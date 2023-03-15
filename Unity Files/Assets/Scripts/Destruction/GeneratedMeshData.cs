using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Connoreaster
{
    public class GeneratedMeshData
    {
        List<Vector3> vertices = new List<Vector3>();
        List<Vector3> normals = new List<Vector3>();
        // List<Vector2> uv = new List<Vector2>();
        List<List<int>> submeshIndices = new List<List<int>>();

        public void AddTriangle(MeshTriangleData _triangle)
        {
            int currentVerticeCount = vertices.Count;

            vertices.AddRange(_triangle.vertices);
            normals.AddRange(_triangle.normals);

            if (submeshIndices.Count < _triangle.subMeshIndex + 1)
            {
                for (int i = submeshIndices.Count; i < _triangle.subMeshIndex + 1; i++)
                {
                    submeshIndices.Add(new List<int>());
                }
            }

            for (int i = 0; i < 3; i++)
            {
                submeshIndices[_triangle.subMeshIndex].Add(currentVerticeCount + i);
            }
        }

        public void AddTriangle(Vector3[] _vertices, Vector3[] _normals, int _submeshIndex, Vector4[] _tangents = null)
        {
            int currentVerticeCount = vertices.Count;

            vertices.AddRange(_vertices);
            normals.AddRange(_normals);

            if (submeshIndices.Count < _submeshIndex + 1)
            {
                for (int i = submeshIndices.Count; i < _submeshIndex + 1; i++)
                {
                    submeshIndices.Add(new List<int>());
                }
            }

            for (int i = 0; i < 3; i++)
            {
                submeshIndices[_submeshIndex].Add(currentVerticeCount + i);
            }
        }

        public Mesh GetGeneratedMesh()
        {
            Mesh mesh = new Mesh();
            mesh.SetVertices(vertices);
            mesh.SetNormals(normals);

            mesh.subMeshCount = submeshIndices.Count;
            for (int i = 0; i < submeshIndices.Count; i++)
            {
                mesh.SetTriangles(submeshIndices[i], i);
            }

            RecalculateUVs(mesh);

            return mesh;
        }

        private void RecalculateUVs(Mesh mesh)
        {
            Bounds bounds = mesh.bounds;
            Vector3[] _vertices = mesh.vertices;
            Vector2[] _uvs = new Vector2[_vertices.Length];

            for (int i = 0; i < _vertices.Length; i++)
            {
                Vector3 v = _vertices[i];
                _uvs[i] = new Vector2((v.x - bounds.min.x) / bounds.size.x, (v.y - bounds.min.y) / bounds.size.y);
            }

            mesh.uv = _uvs;
        }
    }
}
