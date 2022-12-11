/**
 * Copyright 2022 Connor Easterbrook
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Connoreaster
{
    public class GeneratedMeshData
    {
        List<Vector3> vertices = new List<Vector3>();
        List<Vector3> normals = new List<Vector3>();
        List<Vector2> uv = new List<Vector2>();
        List<List<int>> submeshIndices = new List<List<int>>();

        public void AddTriangle(MeshTriangleData _triangle)
        {
            int currentVerticeCount = vertices.Count;

            vertices.AddRange(_triangle.vertices);
            normals.AddRange(_triangle.normals);
            uv.AddRange(_triangle.UVs);

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

        public void AddTriangle(Vector3[] _vertices, Vector3[] _normals, Vector2[] _uvs, int _submeshIndex, Vector4[] _tangents = null)
        {
            int currentVerticeCount = vertices.Count;

            vertices.AddRange(_vertices);
            normals.AddRange(_normals);
            uv.AddRange(_uvs);

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
            mesh.SetUVs(0, uv);
            mesh.SetUVs(1, uv);

            mesh.subMeshCount = submeshIndices.Count;
            for (int i = 0; i < submeshIndices.Count; i++)
            {
                mesh.SetTriangles(submeshIndices[i], i);
            }

            return mesh;
        }
    }
}
