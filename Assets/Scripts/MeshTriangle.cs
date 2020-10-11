
using System.Collections.Generic;
using UnityEngine;

public class MeshTriangle 
{
    private List<Vector3> vertices = new List<Vector3>();
    private List<Vector3> normals = new List<Vector3>();
    private List<Vector2> uvs = new List<Vector2>();
    private int submeshIndices;

    public List<Vector3> Vertices { get { return vertices; } set { vertices = value; } }
    public List<Vector3> Normals { get { return normals; } set { normals = value; } }
    public List<Vector2> Uvs { get { return uvs; } set { uvs = value; } }
    public int SubmeshIndices { get { return submeshIndices; } set { submeshIndices = value; } }

    public MeshTriangle(Vector3[] vertices, Vector3[] normals, Vector2[] uvs, int submeshIndices)
    {
        Clear();

        this.vertices.AddRange(vertices);
        this.normals.AddRange(normals);
        this.uvs.AddRange(uvs);

        this.submeshIndices = submeshIndices;
    }

    public void Clear()
    {
        vertices.Clear();
        normals.Clear();
        uvs.Clear();

        submeshIndices = 0;
    }
}
