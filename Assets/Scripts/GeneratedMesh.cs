
using System.Collections.Generic;
using UnityEngine;

public class GeneratedMesh 
{
    private List<Vector3> vertices = new List<Vector3>();
    private List<Vector3> normals = new List<Vector3>();
    private List<Vector2> uvs = new List<Vector2>();
    private List<List<int>> submeshIndices = new List<List<int>>();

    public List<Vector3> Vertices { get { return vertices; } set { vertices = value; } }
    public List<Vector3> Normals { get { return normals; } set { normals = value; } }
    public List<Vector2> Uvs { get { return uvs; } set { uvs = value; } }
    public List<List<int>> SubmeshIndices { get { return submeshIndices; } set { submeshIndices = value; } }

    public void AddTriangle(MeshTriangle triangle)
    {
        int currentVerticeCount = vertices.Count;
        vertices.AddRange(triangle.Vertices);
        normals.AddRange(triangle.Normals);
        uvs.AddRange(triangle.Uvs);

        if (submeshIndices.Count < triangle.SubmeshIndices + 1)
        {
            for (int i = submeshIndices.Count; i < triangle.SubmeshIndices + 1; i++)
            {
                submeshIndices.Add(new List<int>());
            }
        }

        for (int i = 0; i < 3; i++)
        {
            submeshIndices[triangle.SubmeshIndices].Add(currentVerticeCount + i);
        }
    }

    public Mesh GenerateMesh()
    {
        Mesh mesh = new Mesh
        {
            vertices = vertices.ToArray(),
            normals = normals.ToArray(),
            uv = uvs.ToArray()
        };

        for (int i = 0; i < submeshIndices.Count; i++)
        {
            mesh.SetTriangles(submeshIndices[i].ToArray(),i);
        }

        return mesh;
    }

}
