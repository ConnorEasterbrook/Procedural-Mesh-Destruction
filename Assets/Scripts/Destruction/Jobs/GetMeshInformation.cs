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
using System.Linq;
using UnityEngine;

namespace Connoreaster
{
    public class GetMeshInformation
    {
        public List<Vector3> vertices;
        public List<int> triangles;
        public List<int> subTriangles;
        public List<Vector3> normals;
        public List<Vector2> uv;
        public string _name;

        public GetMeshInformation()
        {
            subTriangles = new List<int>();
        }

        public GetMeshInformation(Mesh mesh)
        {
            vertices = mesh.vertices.ToList();
            triangles = mesh.triangles.ToList();
            normals = mesh.normals.ToList();
            uv = mesh.uv.ToList();
            _name = mesh.name;
            subTriangles = new List<int>();

            if (mesh.subMeshCount > 1)
            {
                subTriangles.AddRange(mesh.GetTriangles(1));
            }
        }

        // public Mesh GetMesh()
        // {

        //     Mesh mesh = new Mesh();
        //     mesh.vertices = vertices.ToArray();

        //     if (normals.Count > 0)
        //     {
        //         mesh.normals = normals.ToArray();
        //     }

        //     if (uv.Count == mesh.vertices.Length)
        //     {
        //         mesh.uv = uv.ToArray();
        //     }

        //     mesh.name = _name;

        //     mesh.subMeshCount = triangles.Length;
        //     for (int i = 0; i < triangles.Length; i++)
        //     {
        //         mesh.SetTriangles(triangles[i].ToArray(), i);
        //     }

        //     return mesh;
        // }
    }
}
