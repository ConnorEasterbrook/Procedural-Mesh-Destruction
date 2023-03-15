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

        [NativeDisableParallelForRestriction] public NativeArray<Vector3> triVerts;
        [NativeDisableParallelForRestriction] public NativeArray<Vector3> triNorms;

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

            triVerts = new NativeArray<Vector3>(subMeshTris.Length, Allocator.TempJob);
            triNorms = new NativeArray<Vector3>(subMeshTris.Length, Allocator.TempJob);

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

            triangleLeftSide[iterator] = plane.GetSide(meshVerts[triangleIndexA]);
            triangleLeftSide[iterator + 1] = plane.GetSide(meshVerts[triangleIndexB]);
            triangleLeftSide[iterator + 2] = plane.GetSide(meshVerts[triangleIndexC]);
        }

        public void Dispose()
        {
            meshSubMeshTriangles.Dispose();

            meshVerts.Dispose();
            meshNormals.Dispose();

            triVerts.Dispose();
            triNorms.Dispose();

            triangleLeftSide.Dispose();
        }
    }
}
