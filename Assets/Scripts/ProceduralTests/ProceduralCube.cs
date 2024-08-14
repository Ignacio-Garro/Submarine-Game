
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class ProceduralCube : MonoBehaviour
{
    public float curvatureAmount = 0.5f; // Controla la cantidad de curvatura

    void OnValidate()
    {
        // Crear un nuevo Mesh
        Mesh mesh = new Mesh();
        mesh.name = "Curved Cube";

        // Definir los vértices del cubo original
        Vector3[] vertices = new Vector3[]
        {
            new Vector3(-0.5f, -0.5f,  0.5f),  // Vértice 0
            new Vector3( 0.5f, -0.5f,  0.5f),  // Vértice 1
            new Vector3( 0.5f,  0.5f,  0.5f),  // Vértice 2
            new Vector3(-0.5f,  0.5f,  0.5f),  // Vértice 3
            new Vector3(-0.5f, -0.5f, -0.5f),  // Vértice 4
            new Vector3( 0.5f, -0.5f, -0.5f),  // Vértice 5
            new Vector3( 0.5f,  0.5f, -0.5f),  // Vértice 6
            new Vector3(-0.5f,  0.5f, -0.5f)   // Vértice 7
        };

        // Aplicar curvatura a los vértices usando la función seno
        for (int i = 0; i < vertices.Length; i++)
        {
            Vector3 v = vertices[i];
            vertices[i] = v.normalized * (0.5f + curvatureAmount * Mathf.Sin(v.magnitude * Mathf.PI));
        }

        // Definir los triángulos del cubo
        int[] triangles = new int[]
        {
            // Cara frontal
            0, 2, 1,
            0, 3, 2,

            // Cara trasera
            5, 6, 4,
            4, 6, 7,

            // Cara izquierda
            4, 7, 0,
            0, 7, 3,

            // Cara derecha
            1, 2, 5,
            5, 2, 6,

            // Cara superior
            3, 7, 2,
            2, 7, 6,

            // Cara inferior
            4, 0, 1,
            4, 1, 5
        };

        // Asignar vértices y triángulos al Mesh
        mesh.vertices = vertices;
        mesh.triangles = triangles;

        // Recalcular Normales para la iluminación
        mesh.RecalculateNormals();

        // Asignar el Mesh al MeshFilter
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        meshFilter.mesh = mesh;
        meshFilter.sharedMesh = mesh;
    }
}
