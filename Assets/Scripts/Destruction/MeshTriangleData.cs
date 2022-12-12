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
    public class MeshTriangleData
    {
        private int submeshIndex;

        public List<Vector3> vertices { get; set; } = new();
        public List<Vector3> normals { get; set; } = new();
        public int subMeshIndex { get => submeshIndex; private set => subMeshIndex = value; }

        public MeshTriangleData(Vector3[] _vertices, Vector3[] _normals, int _submeshIndex)
        {
            Clear();

            vertices.AddRange(_vertices);
            normals.AddRange(_normals);

            submeshIndex = _submeshIndex;
        }

        private void Clear()
        {
            vertices.Clear();
            normals.Clear();

            submeshIndex = 0;
        }
    }
}
