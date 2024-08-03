using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class ProceduralEnviromentCubes : MonoBehaviour
{


    [SerializeField] bool regenerate = false;
    [SerializeField] int size_x;
    [SerializeField] int size_y;
    [SerializeField] int size_z;
    [SerializeField] float cubeSize = 1f;
    [SerializeField] float umbral = 50f;
    [SerializeField] float perlinScale = 0f;
    [SerializeField] float perlinOrigin = 0f;
    
    List<List<List<float>>> grid = new List<List<List<float>>>();

    List<Vector3> vertices = new List<Vector3>();
    List<int> triangles = new List<int>();

    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    private void OnValidate()
    {
        if (!regenerate) return;
        CreateGeometry();
    }

    void CreateGeometry()
    {
        vertices.Clear();
        triangles.Clear();
        grid.Clear();

        FillGrid();

        for (int i = 0; i < size_x; i++)
        {
            for (int j = 0; j < size_y; j++)
            {
                for (int k = 0; k < size_z; k++)
                {
                    if (grid[i][j][k] >= umbral)
                    {
                        if (i <= 0 || grid[i - 1][j][k] < umbral)
                        {
                            CreateFace(i, j, k, 1);
                        }

                        if (i >= size_x - 1 || grid[i + 1][j][k] < umbral)
                        {
                            CreateFace(i, j, k, 0);
                        }
                        if (k <= 0 || grid[i][j][k - 1] < umbral)
                        {
                            CreateFace(i, j, k, 3);
                        }
                        if (k >= size_z - 1 || grid[i][j][k + 1] < umbral)
                        {
                            CreateFace(i, j, k, 2);
                        }
                        if (j <= 0 || grid[i][j - 1][k] < umbral)
                        {
                            CreateFace(i, j, k, 5);
                        }
                        if (j >= size_y - 1 || grid[i][j + 1][k] < umbral)
                        {
                            CreateFace(i, j, k, 4);
                        }
                    }
                }
            }
        }


        Mesh mesh = new Mesh();
        mesh.name = "Procedural Mesh";
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();

        // Recalcular Normales para la iluminación
        mesh.RecalculateNormals();

        // Asignar el Mesh al MeshFilter
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        meshFilter.mesh = mesh;
        meshFilter.sharedMesh = mesh;
    }

    void CreateCube()
    {
        CreateFace(0, 0, 0, 0);
        CreateFace(0, 0, 0, 1);
        CreateFace(0, 0, 0, 2);
        CreateFace(0, 0, 0, 3);
        CreateFace(0, 0, 0, 4);
        CreateFace(0, 0, 0, 5);
    }

    void FillGrid()
    {
        for (int i = 0; i < size_x; i++)
        {
            List<List<float>> ListA = new List<List<float>>();
            grid.Add(ListA);
            for(int j = 0; j < size_y; j++)
            {
                List<float> ListB = new List<float>();
                ListA.Add(ListB);
                for(int k = 0; k < size_z; k++)
                {
                    ListB.Add(FillFunction(i, j, k));
                }
            }

        }
    }

    float FillFunction(int x, int y, int z)
    {
        return PerlinNoise3D(x, y, z);
    }

    float PerlinNoise3D(int gridX, int gridY, int gridZ)
    {
        float x = gridX * perlinScale + perlinOrigin;
        float y = gridY * perlinScale + perlinOrigin;
        float z = gridZ * perlinScale + perlinOrigin;
        float a = Mathf.PerlinNoise(x, y);
        float b = Mathf.PerlinNoise(y, z);
        float c = Mathf.PerlinNoise(x, z);
        return Mathf.Clamp((a + b + c) * 100 / 3, 0, 100);
    }

    //rigth side => 0
    //left side => 1
    //front side => 2
    //back side => 3
    //up side => 4
    //down side => 5
    void CreateFace(int gridX, int gridY, int gridZ, int side)
    {
        float x = gridX * cubeSize;
        float y = gridY * cubeSize;
        float z = gridZ * cubeSize;
        float cubeHalfSize = cubeSize * 0.5f;
        int currentSize = vertices.Count;
        switch (side)
        {
            case 0:
                vertices.Add(new Vector3(x + cubeHalfSize, y + cubeHalfSize, z + cubeHalfSize));
                vertices.Add(new Vector3(x + cubeHalfSize, y - cubeHalfSize, z + cubeHalfSize));
                vertices.Add(new Vector3(x + cubeHalfSize, y - cubeHalfSize, z - cubeHalfSize));
                vertices.Add(new Vector3(x + cubeHalfSize, y + cubeHalfSize, z - cubeHalfSize));
                triangles.Add(currentSize);
                triangles.Add(currentSize + 1);
                triangles.Add(currentSize + 2);
                triangles.Add(currentSize);
                triangles.Add(currentSize + 2);
                triangles.Add(currentSize + 3);
                break;
            case 1:
                vertices.Add(new Vector3(x - cubeHalfSize, y + cubeHalfSize, z + cubeHalfSize));
                vertices.Add(new Vector3(x - cubeHalfSize, y - cubeHalfSize, z + cubeHalfSize));
                vertices.Add(new Vector3(x - cubeHalfSize, y - cubeHalfSize, z - cubeHalfSize));
                vertices.Add(new Vector3(x - cubeHalfSize, y + cubeHalfSize, z - cubeHalfSize));
                triangles.Add(currentSize + 3);
                triangles.Add(currentSize + 2);
                triangles.Add(currentSize);

                triangles.Add(currentSize + 2);
                triangles.Add(currentSize + 1);
                triangles.Add(currentSize);
                break;
            case 2:
                vertices.Add(new Vector3(x + cubeHalfSize, y + cubeHalfSize, z + cubeHalfSize));
                vertices.Add(new Vector3(x + cubeHalfSize, y - cubeHalfSize, z + cubeHalfSize));
                vertices.Add(new Vector3(x - cubeHalfSize, y - cubeHalfSize, z + cubeHalfSize));
                vertices.Add(new Vector3(x - cubeHalfSize, y + cubeHalfSize, z + cubeHalfSize));
                triangles.Add(currentSize + 2);
                triangles.Add(currentSize + 1);
                triangles.Add(currentSize);
                triangles.Add(currentSize + 3);
                triangles.Add(currentSize + 2);
                triangles.Add(currentSize);
                break;
            case 3:
                vertices.Add(new Vector3(x - cubeHalfSize, y - cubeHalfSize, z - cubeHalfSize));
                vertices.Add(new Vector3(x - cubeHalfSize, y + cubeHalfSize, z - cubeHalfSize));
                vertices.Add(new Vector3(x + cubeHalfSize, y + cubeHalfSize, z - cubeHalfSize));
                vertices.Add(new Vector3(x + cubeHalfSize, y - cubeHalfSize, z - cubeHalfSize));
                triangles.Add(currentSize);
                triangles.Add(currentSize + 1);
                triangles.Add(currentSize + 2);
                triangles.Add(currentSize);
                triangles.Add(currentSize + 2);
                triangles.Add(currentSize + 3);

                break;
            case 4:
                vertices.Add(new Vector3(x + cubeHalfSize, y + cubeHalfSize, z + cubeHalfSize));
                vertices.Add(new Vector3(x + cubeHalfSize, y + cubeHalfSize, z - cubeHalfSize));
                vertices.Add(new Vector3(x - cubeHalfSize, y + cubeHalfSize, z - cubeHalfSize));
                vertices.Add(new Vector3(x - cubeHalfSize, y + cubeHalfSize, z + cubeHalfSize));
                triangles.Add(currentSize);
                triangles.Add(currentSize + 1);
                triangles.Add(currentSize + 2);
                triangles.Add(currentSize);
                triangles.Add(currentSize + 2);
                triangles.Add(currentSize + 3);
                break;
            case 5:
                vertices.Add(new Vector3(x + cubeHalfSize, y - cubeHalfSize, z + cubeHalfSize));
                vertices.Add(new Vector3(x + cubeHalfSize, y - cubeHalfSize, z - cubeHalfSize));
                vertices.Add(new Vector3(x - cubeHalfSize, y - cubeHalfSize, z - cubeHalfSize));
                vertices.Add(new Vector3(x - cubeHalfSize, y - cubeHalfSize, z + cubeHalfSize));
                triangles.Add(currentSize + 2);
                triangles.Add(currentSize + 1);
                triangles.Add(currentSize);
                triangles.Add(currentSize + 3);
                triangles.Add(currentSize + 2);
                triangles.Add(currentSize);
                break;
            default:
                break;
        }
    }

}
