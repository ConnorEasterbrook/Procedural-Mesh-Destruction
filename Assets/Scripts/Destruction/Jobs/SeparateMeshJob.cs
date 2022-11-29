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
using Unity.Collections;
using Unity.Jobs;

namespace Connoreaster
{
    public struct SeparateMeshJob : IJobFor
    {
        // [ReadOnly] public NativeArray<int> triangles;
        // [ReadOnly] public NativeArray<Vector3> vertices;
        // [ReadOnly] public NativeArray<Vector3> normals;
        // [ReadOnly] public NativeArray<Vector2> uv;
        // public NativeMultiHashMap<int, Vector3> vertsToAddMap;
        // public NativeMultiHashMap<int, Vector3> normsToAddMap;
        // public NativeMultiHashMap<int, Vector2> uvsToAddMap;

        [ReadOnly] public NativeArray<int> triangles;
        [ReadOnly] public NativeArray<Vector3> vertices;
        [ReadOnly] public NativeArray<Vector3> normals;
        [ReadOnly] public NativeArray<Vector2> uv;
        [NativeDisableParallelForRestriction] public NativeArray<Vector3> triPoints;
        [NativeDisableParallelForRestriction] public NativeArray<Vector3> triNorms;
        [NativeDisableParallelForRestriction] public NativeArray<Vector2> triUVs;

        public void Execute(int index)
        {
            int i = index * 3;

            int triangleIndexA = triangles[i]; // Get the first index of the triangle
            int triangleIndexB = triangles[i + 1]; // Get the second index of the triangle
            int triangleIndexC = triangles[i + 2]; // Get the third index of the triangle

            triPoints[i] = vertices[triangleIndexA];
            triPoints[i + 1] = vertices[triangleIndexB];
            triPoints[i + 2] = vertices[triangleIndexC];

            triNorms[i] = normals[triangleIndexA];
            triNorms[i + 1] = normals[triangleIndexB];
            triNorms[i + 2] = normals[triangleIndexC];

            triUVs[i] = uv[triangleIndexA];
            triUVs[i + 1] = uv[triangleIndexB];
            triUVs[i + 2] = uv[triangleIndexC];

            // Vector3 vertexA = vertices[triangleIndexA];
            // Vector3 vertexB = vertices[triangleIndexB];
            // Vector3 vertexC = vertices[triangleIndexC];

            // Vector3 normalA = normals[triangleIndexA];
            // Vector3 normalB = normals[triangleIndexB];
            // Vector3 normalC = normals[triangleIndexC];

            // Vector2 uvA = uv[triangleIndexA];
            // Vector2 uvB = uv[triangleIndexB];
            // Vector2 uvC = uv[triangleIndexC];

            // vertsToAddMap.Add(index, vertexA);
            // vertsToAddMap.Add(index, vertexB);
            // vertsToAddMap.Add(index, vertexC);

            // normsToAddMap.Add(index, normalA);
            // normsToAddMap.Add(index, normalB);
            // normsToAddMap.Add(index, normalC);

            // uvsToAddMap.Add(index, uvA);
            // uvsToAddMap.Add(index, uvB);
            // uvsToAddMap.Add(index, uvC);
        }
    }
}
