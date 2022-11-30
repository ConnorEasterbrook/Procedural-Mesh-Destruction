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
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using Unity.Collections;
using Unity.Jobs;

namespace Connoreaster
{
    public struct TriangleJob : IJob
    {
        private int triangleIndexA;
        private int triangleIndexB;
        private int triangleIndexC;
        private NativeArray<Vector3> _vertices;
        private NativeArray<Vector3> _normals;
        private NativeArray<Vector2> _uvs;
        public NativeArray<Vector3> vertices;
        public NativeArray<Vector3> normals;
        public NativeArray<Vector2> uvs;

        public TriangleJob(Mesh mesh, int A, int B, int C)
        {
            triangleIndexA = A;
            triangleIndexB = B;
            triangleIndexC = C;

            _vertices = new NativeArray<Vector3>(mesh.vertices, Allocator.TempJob);
            _normals = new NativeArray<Vector3>(mesh.normals, Allocator.TempJob);
            _uvs = new NativeArray<Vector2>(mesh.uv, Allocator.TempJob);

            vertices = new NativeArray<Vector3>(3, Allocator.TempJob);
            normals = new NativeArray<Vector3>(3, Allocator.TempJob);
            uvs = new NativeArray<Vector2>(3, Allocator.TempJob);
        }

        public void Execute()
        {
            vertices[0] = _vertices[triangleIndexA];
            vertices[1] = _vertices[triangleIndexB];
            vertices[2] = _vertices[triangleIndexC];

            normals[0] = _normals[triangleIndexA];
            normals[1] = _normals[triangleIndexB];
            normals[2] = _normals[triangleIndexC];

            uvs[0] = _uvs[triangleIndexA];
            uvs[1] = _uvs[triangleIndexB];
            uvs[2] = _uvs[triangleIndexC];
        }

        public void Dispose()
        {
            _vertices.Dispose();
            _normals.Dispose();
            _uvs.Dispose();

            vertices.Dispose();
            normals.Dispose();
            uvs.Dispose();
        }
    }
}
