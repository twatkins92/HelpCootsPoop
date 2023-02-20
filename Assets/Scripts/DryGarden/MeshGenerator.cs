using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class MeshGenerator : MonoBehaviour
{
    public int resolution = 10;
    public Material material;
    public float width = 1f;
    public float length = 1f;

    void Awake()
    {
        // Create the mesh
        Mesh mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        // Add the MeshRenderer component and assign the material
        GetComponent<MeshRenderer>().material = material;

        // Calculate the number of vertices
        int numVertices = (resolution + 1) * (resolution + 1);

        // Calculate the number of quads
        int numQuads = resolution * resolution;

        // Create the arrays to hold the vertices, normals, and UVs
        Vector3[] vertices = new Vector3[numVertices];
        Vector3[] normals = new Vector3[numVertices];
        Vector2[] uvs = new Vector2[numVertices];

        // Calculate the spacing between the vertices
        float spacing = 1f / resolution;
        float halfWidth = width * 0.5f;
        float halfLength = length * 0.5f;

        // Generate the vertices, normals, and UVs
        for (int i = 0; i <= resolution; i++)
        {
            for (int j = 0; j <= resolution; j++)
            {
                int index = i * (resolution + 1) + j;
                vertices[index] = new Vector3(
                    (i * spacing - 0.5f) * width,
                    0f,
                    (j * spacing - 0.5f) * length
                );
                normals[index] = Vector3.up;
                uvs[index] = new Vector2(i * spacing, j * spacing);
            }
        }

        // Create the array to hold the triangles
        int[] triangles = new int[numQuads * 6];

        // Generate the triangles
        for (int i = 0; i < resolution; i++)
        {
            for (int j = 0; j < resolution; j++)
            {
                int quadIndex = i * resolution + j;
                int vertexIndex = i * (resolution + 1) + j;

                // First triangle
                triangles[quadIndex * 6] = vertexIndex;
                triangles[quadIndex * 6 + 1] = vertexIndex + 1;
                triangles[quadIndex * 6 + 2] = vertexIndex + resolution + 1;

                // Second triangle
                triangles[quadIndex * 6 + 3] = vertexIndex + 1;
                triangles[quadIndex * 6 + 4] = vertexIndex + resolution + 2;
                triangles[quadIndex * 6 + 5] = vertexIndex + resolution + 1;
            }
        }

        // Assign the arrays to the mesh
        mesh.vertices = vertices;
        mesh.normals = normals;
        mesh.uv = uvs;
        mesh.triangles = triangles;

        GetComponent<KDT>().Init();
    }
}
