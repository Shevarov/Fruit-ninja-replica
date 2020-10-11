using System.Collections.Generic;
using UnityEngine;

public class Cutter : MonoBehaviour
{
    private static bool currentlyCutting;
    private static Mesh originalMesh;

    public static void CutGameobject(GameObject originalGameObject, Vector3 contactPoint, Vector3 direction, bool addRigidbody = false)
    {
        if (currentlyCutting)
            return;

        currentlyCutting = true;

        originalMesh = originalGameObject.GetComponent<MeshFilter>().mesh;

        var plane = new Plane(originalGameObject.transform.InverseTransformDirection(direction),originalGameObject.transform.InverseTransformPoint(contactPoint));
        
        var addedVertices = new List<Vector3>();

        var leftMesh = new GeneratedMesh();
        var rightMesh = new GeneratedMesh();

        int[] submeshIndices;
        int triangleIndexA, triangleIndexB, triangleIndexC;

        for (int i = 0; i < originalMesh.subMeshCount; i++)
        {
            submeshIndices = originalMesh.GetTriangles(i);

            for (int j = 0; j < submeshIndices.Length; j += 3)
            {
                triangleIndexA = submeshIndices[j];
                triangleIndexB = submeshIndices[j + 1];
                triangleIndexC = submeshIndices[j + 2];

                MeshTriangle currentTriangle = GetTriangle(triangleIndexA, triangleIndexB, triangleIndexC, i);

                bool[] pointsOnLeftSide = new bool [3];

                 pointsOnLeftSide[0] = plane.GetSide(originalMesh.vertices[triangleIndexA]);
                 pointsOnLeftSide[1] = plane.GetSide(originalMesh.vertices[triangleIndexB]);
                 pointsOnLeftSide[2] = plane.GetSide(originalMesh.vertices[triangleIndexC]);

                if (pointsOnLeftSide[0] && pointsOnLeftSide[1] && pointsOnLeftSide[2])
                {
                    leftMesh.AddTriangle(currentTriangle);
                }
                else if (!pointsOnLeftSide[0] && !pointsOnLeftSide[1] && !pointsOnLeftSide[2])
                {
                    rightMesh.AddTriangle(currentTriangle);
                }
                else
                {
                    CutTriangle(plane,currentTriangle,pointsOnLeftSide,leftMesh,rightMesh,addedVertices);
                }

            }

            FillCut(addedVertices, plane, leftMesh, rightMesh);

            GenerateCutterGameobject(originalGameObject,leftMesh,addRigidbody,Vector3.up);
            GenerateCutterGameobject(originalGameObject, rightMesh,addRigidbody,Vector3.left);
            
            currentlyCutting = false;
        }


}

    private static MeshTriangle GetTriangle(int triangleIndexA, int triangleIndexB, int triangleIndexC, int submeshIndex)
    {
        Vector3[] vertices = {
            originalMesh.vertices[triangleIndexA],
            originalMesh.vertices[triangleIndexB],
            originalMesh.vertices[triangleIndexC],
        };

        Vector3[] normals = {
            originalMesh.normals[triangleIndexA],
            originalMesh.normals[triangleIndexB],
            originalMesh.normals[triangleIndexC],
        };

        Vector2[] uvs = {
            originalMesh.uv[triangleIndexA],
            originalMesh.uv[triangleIndexB],
            originalMesh.uv[triangleIndexC],
        };

        return new MeshTriangle(vertices,normals,uvs,submeshIndex);
    }

    private static void GenerateCutterGameobject(GameObject originalGameObject, GeneratedMesh mesh, bool addRigidbody, Vector3 direction) 
    {
        if (mesh.Vertices.Count > 3)
        {
            GameObject gameObject = new GameObject(originalGameObject.name, typeof(MeshFilter), typeof(MeshRenderer));
            gameObject.transform.localRotation = originalGameObject.transform.localRotation;
            gameObject.transform.localScale = originalGameObject.transform.localScale;
            gameObject.transform.position = originalGameObject.transform.position;

            gameObject.GetComponent<MeshFilter>().mesh = mesh.GenerateMesh();

            gameObject.GetComponent<MeshRenderer>().material = originalGameObject.GetComponent<MeshRenderer>().material;
            
            if (addRigidbody)
            {
                gameObject.AddComponent<Rigidbody>();
                gameObject.GetComponent<Rigidbody>().AddTorque(direction*200f);
            }

            gameObject.AddComponent<MeshCollider>();
            gameObject.GetComponent<MeshCollider>().convex = true;

        }
    }

    private static void CutTriangle(Plane plane, MeshTriangle triangle, bool [] pointsOnLeftSide,GeneratedMesh leftMesh, GeneratedMesh rightMesh, List<Vector3> addedVertices)
    {
        MeshTriangle leftMeshTriangle = new MeshTriangle(new Vector3[2],new Vector3[2],new Vector2[2],triangle.SubmeshIndices);
        MeshTriangle rightMeshTriangle = new MeshTriangle(new Vector3[2], new Vector3[2], new Vector2[2], triangle.SubmeshIndices);

        bool left = false;
        bool right = false;

        for (int i = 0; i < 3; i++)
        {
            if (pointsOnLeftSide[i])
            {
                if (!left)
                {
                    left = true;

                    leftMeshTriangle.Vertices[0] = leftMeshTriangle.Vertices[1] = triangle.Vertices[i];
                    leftMeshTriangle.Uvs[0] = leftMeshTriangle.Uvs[1] = triangle.Uvs[i];
                    leftMeshTriangle.Normals[0] = leftMeshTriangle.Normals[1] = triangle.Normals[i];
                }
                else
                {
                    leftMeshTriangle.Vertices[1] = triangle.Vertices[i];
                    leftMeshTriangle.Uvs[1] = triangle.Uvs[i];
                    leftMeshTriangle.Normals[1] = triangle.Normals[i];
                }
            }
            else
            {
                if (!right)
                {
                    right = true;

                    rightMeshTriangle.Vertices[0] = rightMeshTriangle.Vertices[1] = triangle.Vertices[i];
                    rightMeshTriangle.Uvs[0] = rightMeshTriangle.Uvs[1] = triangle.Uvs[i];
                    rightMeshTriangle.Normals[0] = rightMeshTriangle.Normals[1] = triangle.Normals[i];
                }
                else
                {
                    rightMeshTriangle.Vertices[1] = triangle.Vertices[i];
                    rightMeshTriangle.Uvs[1] = triangle.Uvs[i];
                    rightMeshTriangle.Normals[1] = triangle.Normals[i];
                }
            }
        }


        // можно переписать черех цикл и сократить количество кода в 2 раза и вынести в новую функцию добавление новой вершины
        float normalizedDistance;
        float distance;
        plane.Raycast(new Ray(leftMeshTriangle.Vertices[0],(rightMeshTriangle.Vertices[0]-leftMeshTriangle.Vertices[0]).normalized),out distance);

        normalizedDistance = distance / (rightMeshTriangle.Vertices[0] - leftMeshTriangle.Vertices[0]).magnitude; 
        Vector3 vertLeft = Vector3.Lerp(leftMeshTriangle.Vertices[0], rightMeshTriangle.Vertices[0], normalizedDistance);
        addedVertices.Add(vertLeft);

        Vector3 normalLeft = Vector3.Lerp(leftMeshTriangle.Normals[0], rightMeshTriangle.Normals[0], normalizedDistance);
        Vector3 uvLeft = Vector3.Lerp(leftMeshTriangle.Uvs[0], rightMeshTriangle.Uvs[0], normalizedDistance);

        plane.Raycast(new Ray(leftMeshTriangle.Vertices[1], (rightMeshTriangle.Vertices[1] - leftMeshTriangle.Vertices[1]).normalized), out distance);

        normalizedDistance = distance / (rightMeshTriangle.Vertices[1] - leftMeshTriangle.Vertices[1]).magnitude;
        Vector3 vertRight = Vector3.Lerp(leftMeshTriangle.Vertices[1], rightMeshTriangle.Vertices[1], normalizedDistance);
        addedVertices.Add(vertRight);

        Vector3 normalRight = Vector3.Lerp(leftMeshTriangle.Normals[1], rightMeshTriangle.Normals[1], normalizedDistance);
        Vector3 uvRight = Vector3.Lerp(leftMeshTriangle.Uvs[1], rightMeshTriangle.Uvs[1], normalizedDistance);

        // до сюда

        // можно сократить код в 4 раза и вынести в новую функцию 
        Vector3[] updatedVertices = new Vector3[] { leftMeshTriangle.Vertices[0],vertLeft,vertRight};
        Vector3[] updatedNormals = new Vector3[] { leftMeshTriangle.Normals[0], normalLeft, normalRight };
        Vector2[] updatedUvs = new Vector2[] { leftMeshTriangle.Uvs[0], uvLeft, uvRight };

        MeshTriangle currentTriangle = new MeshTriangle(updatedVertices, updatedNormals, updatedUvs,triangle.SubmeshIndices);

        if (updatedVertices[0] != updatedVertices[1] && updatedVertices[0] != updatedVertices[2])
        {
            if (Vector3.Dot(Vector3.Cross(updatedVertices[1] - updatedVertices[0], updatedVertices[2] - updatedVertices[0]), updatedNormals[0]) < 0)
            {
                FlipTriangle(currentTriangle);
            }
            leftMesh.AddTriangle(currentTriangle);
        }

         updatedVertices = new Vector3[] { leftMeshTriangle.Vertices[0], leftMeshTriangle.Vertices[1], vertRight };
         updatedNormals = new Vector3[] { leftMeshTriangle.Normals[0], leftMeshTriangle.Normals[1], normalRight };
         updatedUvs = new Vector2[] { leftMeshTriangle.Uvs[0], leftMeshTriangle.Uvs[1], uvRight };

         currentTriangle = new MeshTriangle(updatedVertices, updatedNormals, updatedUvs, triangle.SubmeshIndices);

        if (updatedVertices[0] != updatedVertices[1] && updatedVertices[0] != updatedVertices[2])
        {
            if (Vector3.Dot(Vector3.Cross(updatedVertices[1] - updatedVertices[0], updatedVertices[2] - updatedVertices[0]), updatedNormals[0]) < 0)
            {
                FlipTriangle(currentTriangle);
            }
            leftMesh.AddTriangle(currentTriangle);
        }

        updatedVertices = new Vector3[] { rightMeshTriangle.Vertices[0], vertLeft, vertRight };
        updatedNormals = new Vector3[] { rightMeshTriangle.Normals[0], normalLeft, normalRight };
        updatedUvs = new Vector2[] { rightMeshTriangle.Uvs[0], uvLeft, uvRight };

        currentTriangle = new MeshTriangle(updatedVertices, updatedNormals, updatedUvs, triangle.SubmeshIndices);

        if (updatedVertices[0] != updatedVertices[1] && updatedVertices[0] != updatedVertices[2])
        {
            if (Vector3.Dot(Vector3.Cross(updatedVertices[1] - updatedVertices[0], updatedVertices[2] - updatedVertices[0]), updatedNormals[0]) < 0)
            {
                FlipTriangle(currentTriangle);
            }
            rightMesh.AddTriangle(currentTriangle);
        }

        updatedVertices = new Vector3[] { rightMeshTriangle.Vertices[0], rightMeshTriangle.Vertices[1], vertRight };
        updatedNormals = new Vector3[] { rightMeshTriangle.Normals[0], rightMeshTriangle.Normals[1], normalRight };
        updatedUvs = new Vector2[] { rightMeshTriangle.Uvs[0], rightMeshTriangle.Uvs[1], uvRight };

        currentTriangle = new MeshTriangle(updatedVertices, updatedNormals, updatedUvs, triangle.SubmeshIndices);

        if (updatedVertices[0] != updatedVertices[1] && updatedVertices[0] != updatedVertices[2])
        {
            if (Vector3.Dot(Vector3.Cross(updatedVertices[1] - updatedVertices[0], updatedVertices[2] - updatedVertices[0]), updatedNormals[0]) < 0)
            {
                FlipTriangle(currentTriangle);
            }
            rightMesh.AddTriangle(currentTriangle);
        }
        // до сюда
    }

    private static void FlipTriangle(MeshTriangle triangle)
    {
        Vector3 lastVertex = triangle.Vertices[triangle.Vertices.Count - 1];
        triangle.Vertices[triangle.Vertices.Count - 1] = triangle.Vertices[0];
        triangle.Vertices[0] = lastVertex;

        Vector3 lastNormal = triangle.Normals[triangle.Normals.Count - 1];
        triangle.Normals[triangle.Normals.Count - 1] = lastNormal;
        triangle.Normals[0] = lastNormal;

        Vector2 lastUv = triangle.Uvs[triangle.Uvs.Count - 1];
        triangle.Uvs[triangle.Uvs.Count - 1] = triangle.Uvs[0];
        triangle.Uvs[0] = lastUv;
    }

    private static void FillCut(List<Vector3> addedVertices, Plane plane, GeneratedMesh leftMesh, GeneratedMesh rightMesh)
    {
        List<Vector3> vertices = new List<Vector3>();
        List<Vector3> polygon = new List<Vector3>();

        for (int i = 0; i < addedVertices.Count; i++)
        {
            if (!vertices.Contains(addedVertices[i]))
            {
                polygon.Clear();
                polygon.Add(addedVertices[i]);
                polygon.Add(addedVertices[i+1]);

                vertices.Add(addedVertices[i]);
                vertices.Add(addedVertices[i + 1]);

                EvaluatePairs(addedVertices, vertices, polygon);
                Fill(polygon,plane,leftMesh,rightMesh);
            }
        }

    }

    private static void EvaluatePairs(List<Vector3> addedVertices, List<Vector3> vertices, List<Vector3> polygon)
    {
        bool isDone = false;
        while (!isDone)
        {
            isDone = true;
            for (int i = 0; i < addedVertices.Count; i+=2)
            {
                if (addedVertices[i] == polygon[polygon.Count - 1] && !vertices.Contains(addedVertices[i + 1]))
                {
                    isDone = false;
                    polygon.Add(addedVertices[i+1]);
                    vertices.Add(addedVertices[i+1]);
                }
                else if (addedVertices[i+1] == polygon[polygon.Count - 1] && !vertices.Contains(addedVertices[i]))
                {
                    isDone = false;
                    polygon.Add(addedVertices[i]);
                    vertices.Add(addedVertices[i]);
                }
            }
        }
    }

    private static void Fill(List<Vector3> _vertices, Plane plane, GeneratedMesh leftMesh, GeneratedMesh rightMesh)
    {
        Vector3 centerPosition = Vector3.zero;

        for (int i = 0; i < _vertices.Count; i++)
        {
            centerPosition += _vertices[i];
        }
        centerPosition /= _vertices.Count;

        Vector3 up = plane.normal;

        Vector3 left = Vector3.Cross(plane.normal,plane.normal);

        Vector3 displacement = Vector3.zero;
        Vector2 uv1, uv2;

        for (int i = 0; i < _vertices.Count; i++)
        {
            displacement = _vertices[i] - centerPosition;
            uv1 = new Vector2(0.5f+Vector3.Dot(displacement,left),0.5f+Vector3.Dot(displacement,up));
            displacement = _vertices[(i + 1) % _vertices.Count] - centerPosition;
            uv2 = new Vector2(0.5f + Vector3.Dot(displacement, left), 0.5f + Vector3.Dot(displacement, up));

            Vector3[] vertices = new Vector3[] { _vertices[i], _vertices[(i + 1) % _vertices.Count], centerPosition };
            Vector3[] normals = new Vector3[] {-plane.normal, -plane.normal, -plane.normal };
            Vector2[] uvs = new Vector2[] { uv1,uv2,new Vector2(0.5f,0.5f)};

            MeshTriangle currenTriangle = new MeshTriangle(vertices,normals,uvs,originalMesh.subMeshCount-1);

            if (Vector3.Dot(Vector3.Cross(vertices[1] - vertices[0], vertices[2] - vertices[0]), normals[0]) < 0)
            {
                FlipTriangle(currenTriangle);
            }
            leftMesh.AddTriangle(currenTriangle);

            normals = new Vector3[] { plane.normal, plane.normal, plane.normal };
            currenTriangle = new MeshTriangle(vertices, normals, uvs, originalMesh.subMeshCount - 1);

            if (Vector3.Dot(Vector3.Cross(vertices[1] - vertices[0], vertices[2] - vertices[0]), normals[0]) < 0)
            {
                FlipTriangle(currenTriangle);
            }
            rightMesh.AddTriangle(currenTriangle);
        }
    }
}

