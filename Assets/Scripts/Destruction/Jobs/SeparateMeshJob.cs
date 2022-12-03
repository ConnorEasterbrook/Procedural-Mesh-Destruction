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

using UnityEngine;
using Unity.Collections;
using Unity.Jobs;

namespace Connoreaster
{
    public struct SeparateMeshJob : IJobFor
    {
        [ReadOnly] private NativeArray<int> meshSubMeshTriangles;

        [ReadOnly] private int triangleIndexA;
        [ReadOnly] private int triangleIndexB;
        [ReadOnly] private int triangleIndexC;

        [ReadOnly] private NativeArray<Vector3> meshVerts;
        [ReadOnly] private NativeArray<Vector3> meshNormals;
        [ReadOnly] private NativeArray<Vector2> meshUVS;

        [NativeDisableParallelForRestriction] public NativeArray<Vector3> triVerts;
        [NativeDisableParallelForRestriction] public NativeArray<Vector3> triNorms;
        [NativeDisableParallelForRestriction] public NativeArray<Vector2> triUVs;

        [ReadOnly] private Plane plane;
        [NativeDisableParallelForRestriction] public NativeArray<bool> triangleLeftSide;

        public SeparateMeshJob(int i, Mesh mesh, int[] subMeshTris, Plane plane)
        {
            meshSubMeshTriangles = new NativeArray<int>(subMeshTris, Allocator.TempJob);

            triangleIndexA = 0;
            triangleIndexB = 0;
            triangleIndexC = 0;

            meshVerts = new NativeArray<Vector3>(mesh.vertices, Allocator.TempJob);
            meshNormals = new NativeArray<Vector3>(mesh.normals, Allocator.TempJob);
            meshUVS = new NativeArray<Vector2>(mesh.uv, Allocator.TempJob);

            triVerts = new NativeArray<Vector3>(subMeshTris.Length, Allocator.TempJob);
            triNorms = new NativeArray<Vector3>(subMeshTris.Length, Allocator.TempJob);
            triUVs = new NativeArray<Vector2>(subMeshTris.Length, Allocator.TempJob);

            this.plane = plane;
            triangleLeftSide = new NativeArray<bool>(subMeshTris.Length, Allocator.TempJob);
        }

        public void Execute(int index)
        {
            int iterator = index * 3;

            triangleIndexA = meshSubMeshTriangles[iterator];
            triangleIndexB = meshSubMeshTriangles[iterator + 1];
            triangleIndexC = meshSubMeshTriangles[iterator + 2];

            triVerts[iterator] = meshVerts[triangleIndexA];
            triVerts[iterator + 1] = meshVerts[triangleIndexB];
            triVerts[iterator + 2] = meshVerts[triangleIndexC];

            triNorms[iterator] = meshNormals[triangleIndexA];
            triNorms[iterator + 1] = meshNormals[triangleIndexB];
            triNorms[iterator + 2] = meshNormals[triangleIndexC];

            triUVs[iterator] = meshUVS[triangleIndexA];
            triUVs[iterator + 1] = meshUVS[triangleIndexB];
            triUVs[iterator + 2] = meshUVS[triangleIndexC];

            triangleLeftSide[iterator] = plane.GetSide(meshVerts[triangleIndexA]);
            triangleLeftSide[iterator + 1] = plane.GetSide(meshVerts[triangleIndexB]);
            triangleLeftSide[iterator + 2] = plane.GetSide(meshVerts[triangleIndexC]);
        }

        public void Dispose()
        {
            meshSubMeshTriangles.Dispose();

            meshVerts.Dispose();
            meshNormals.Dispose();
            meshUVS.Dispose();

            triVerts.Dispose();
            triNorms.Dispose();
            triUVs.Dispose();

            triangleLeftSide.Dispose();
        }
    }
}
