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
