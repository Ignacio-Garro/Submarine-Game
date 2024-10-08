

using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.XR;

public class Voxelization : MonoBehaviour
{
    public GameObject mesh; // Malla que quieres voxelizar
    public int resolution = 32; // Resolución de la textura 3D (por ejemplo, 32x32x32)
    public Texture3D voxelTexture;
    public float height = 1f;
    public float width = 1f;
    public float depth = 1f;
    public bool saveTexture = false;
    public string assetPath = "Assets/3D_Textures/MyTexture2DArray.asset";


    void Start()
    {
        mesh.transform.position = Vector3.zero;
        Bounds bounds = mesh.GetComponent<Renderer>().bounds;
        voxelTexture = GenerateVoxelTexture(mesh, resolution);
        height = bounds.max.y - bounds.min.y ;
        width = bounds.max.x - bounds.min.x ;
        depth = bounds.max.z - bounds.min.z ;
        if (saveTexture) SaveTexture2DArray("Assets/3D_Textures");

    }

    // Función para generar la textura 3D
    Texture3D GenerateVoxelTexture(GameObject mesh, int resolution)
    {
        // Calcular los límites de la malla en espacio local
        Bounds bounds = mesh.GetComponent<Renderer>().bounds;

        // Crear la textura 3D con la resolución especificada
        Texture3D texture = new Texture3D(resolution, resolution, resolution, TextureFormat.RFloat, false);

        // Array para almacenar los datos de la textura (1.0f si el punto está dentro, 0.0f si está fuera)
        float[] voxelData = new float[resolution * resolution * resolution];

        // Recorrer cada voxel en la textura 3D
        for (int x = 0; x < resolution; x++)
        {
            for (int y = 0; y < resolution; y++)
            {
                for (int z = 0; z < resolution; z++)
                {
                    // Calcular la posición del voxel en el espacio local de la malla
                    Vector3 localPos = new Vector3(
                        Mathf.Lerp(bounds.min.x, bounds.max.x, (float)x / (resolution - 1)),
                        Mathf.Lerp(bounds.min.y, bounds.max.y, (float)y / (resolution - 1)),
                        Mathf.Lerp(bounds.min.z, bounds.max.z, (float)z / (resolution - 1))
                    );


                    Vector3 worldPos = mesh.transform.TransformPoint(localPos);

                    bool inside = IsPointInsideMesh(localPos, mesh);

                    // Convertir de nuevo la posición mundial al espacio local de la malla
                    Vector3 meshLocalPos = mesh.transform.InverseTransformPoint(worldPos);

                    // Verificar si el punto está dentro de la malla
                    

                 

                    // Asignar el valor correspondiente al voxel (1.0 si está dentro, 0.0 si está fuera)
                    voxelData[x + y * resolution + z * resolution * resolution] = inside ? 1.0f : 0.0f;
                }
            }
        }

        // Asignar los datos a la textura 3D
        texture.SetPixelData(voxelData, 0);
        texture.Apply();

        return texture;
    }

    // Función para comprobar si un punto está dentro de la malla
    bool IsPointInsideMesh(Vector3 point, GameObject mesh)
    {
        Vector3 closestPoint = mesh.GetComponent<Collider>().ClosestPoint(point);
        // Si el número de intersecciones es impar, el punto está dentro de la malla
        return closestPoint == point;
    }

    // Función para verificar si un rayo intersecta un triángulo (usamos el algoritmo Möller-Trumbore)
    bool RayTriangleIntersection(Ray ray, Vector3 v0, Vector3 v1, Vector3 v2)
    {
        Vector3 edge1 = v1 - v0;
        Vector3 edge2 = v2 - v0;
        Vector3 h = Vector3.Cross(ray.direction, edge2);
        float a = Vector3.Dot(edge1, h);

        if (a > -0.0001f && a < 0.0001f) return false;  // El rayo es paralelo al triángulo

        float f = 1.0f / a;
        Vector3 s = ray.origin - v0;
        float u = f * Vector3.Dot(s, h);

        if (u < 0.0f || u > 1.0f) return false;

        Vector3 q = Vector3.Cross(s, edge1);
        float v = f * Vector3.Dot(ray.direction, q);

        if (v < 0.0f || u + v > 1.0f) return false;

        float t = f * Vector3.Dot(edge2, q);

        // Si t > 0, hay una intersección
        return t > 0.0001f;
    }

    public void SaveTexture2DArray(string path)
    {
        // Crear un Texture2DArray
        Texture2DArray textureArray = new Texture2DArray(
            voxelTexture.width,
            voxelTexture.height,
            voxelTexture.depth, // La cantidad de capas en el Texture2DArray corresponde a la profundidad de la textura 3D
            TextureFormat.RGBA32,
            false
        );

        // Obtener los píxeles de la textura 3D
        Color[] colors = voxelTexture.GetPixels();
        Texture2D texture2D = new Texture2D(voxelTexture.width, voxelTexture.height);

        // Rellenar el Texture2DArray con las capas de la textura 3D
        for (int z = 0; z < voxelTexture.depth; z++)
        {
            for (int y = 0; y < voxelTexture.height; y++)
            {
                for (int x = 0; x < voxelTexture.width; x++)
                {
                    texture2D.SetPixel(x, y, colors[x + y * voxelTexture.width + z * voxelTexture.width * voxelTexture.height]);
                }
            }

            // Aplicar los píxeles al Texture2D y copiarlos a la capa correspondiente en el Texture2DArray
            texture2D.Apply();
            Graphics.CopyTexture(texture2D, 0, 0, textureArray, z, 0);
        }

        // Guardar el Texture2DArray como un asset
        //AssetDatabase.CreateAsset(textureArray, assetPath);
        //AssetDatabase.SaveAssets();

        Debug.Log("Texture2DArray guardado como asset en " + assetPath);
    }
}

