using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Connoreaster
{
    public class MeshFillCalculations
    {
        private MeshCutCalculations calcScript;
        private List<Vector3> newVertices;
        private Plane slicePlane;

        public MeshFillCalculations(MeshCutCalculations _calcScript, List<Vector3> _newVertices, Plane _slicePlane)
        {
            calcScript = _calcScript;
            newVertices = _newVertices;
            slicePlane = _slicePlane;
        }

        public void CallScript()
        {
            BeginFill();
        }

        /// <summary>
        /// Begin the process of filling in the cross section of the sliced mesh
        /// </summary>
        public void BeginFill()
        {
            List<Vector3> vertices = new List<Vector3>(); // Create a list of vertices
            List<Vector3> polygonVertices = new List<Vector3>(); // Create a list of vertices for the polygon

            // Loop through all the vertices in the mesh
            for (int i = 0; i < newVertices.Count; i++)
            {
                // If the vertex is not in the list of vertices
                if (!vertices.Contains(newVertices[i]))
                {
                    polygonVertices.Clear(); // Clear the polygon list
                    polygonVertices.Add(newVertices[i]); // Add the vertex to the polygon list
                    polygonVertices.Add(newVertices[i + 1]); // Add the next vertex to the polygon list

                    vertices.Add(newVertices[i]); // Add the vertex to the list of vertices. This is used to prevent duplicate vertices
                    vertices.Add(newVertices[i + 1]);

                    EvaluatePairs(vertices, polygonVertices); // Evaluate the pairs of vertices

                    Vector3 centerPosition = Vector3.zero; // Create a new vector for the center position

                    // Loop through all the vertices in the polygon
                    for (int j = 0; j < polygonVertices.Count; j++)
                    {
                        centerPosition += polygonVertices[j];
                    }

                    centerPosition /= polygonVertices.Count; // Set the center position to the average of all the vertices

                    // Loop through all the vertices in the polygon
                    for (int j = 0; j < polygonVertices.Count; j++)
                    {
                        bool isMesh1 = true;
                        CheckFaceDir(calcScript.mesh1, polygonVertices, centerPosition, isMesh1, j); // Check if the fill should be flipped for the first mesh

                        isMesh1 = false;
                        CheckFaceDir(calcScript.mesh2, polygonVertices, centerPosition, isMesh1, j); // Check if the fill should be flipped for the second mesh
                    }
                }
            }
        }

        /// <summary>
        /// Evaluate the pairs of vertices
        /// </summary>
        public void EvaluatePairs(List<Vector3> vertices, List<Vector3> polygonVertices)
        {
            bool isDone = false; // Create a boolean to determine if the loop is done
            while (!isDone)
            {
                isDone = true; // Set the boolean to true so that it only loops once if no pairs are found

                // Loop through all the vertices in the mesh
                for (int i = 0; i < newVertices.Count; i += 2)
                {
                    // If the first vertex is in the polygon and the second vertex is not. Else if the second vertex is in the polygon and the first vertex is not
                    if (newVertices[i] == polygonVertices[polygonVertices.Count - 1] && !vertices.Contains(newVertices[i + 1]))
                    {
                        isDone = false;
                        polygonVertices.Add(newVertices[i + 1]); // Add the next vertex to the polygon list
                        vertices.Add(newVertices[i + 1]); // Add the next vertex to the list of vertices
                    }
                    else if (newVertices[i + 1] == polygonVertices[polygonVertices.Count - 1] && !vertices.Contains(newVertices[i]))
                    {
                        isDone = false;
                        polygonVertices.Add(newVertices[i]); // Add the next vertex to the polygon list
                        vertices.Add(newVertices[i]); // Add the next vertex to the list of vertices
                    }
                }
            }
        }

        /// <summary>
        /// Check if the fill should be flipped
        /// </summary>
        private void CheckFaceDir(GeneratedMeshData mesh, List<Vector3> polygonVertices, Vector3 centerPosition, bool isMesh1, int index)
        {
            Vector3[] _vertices = { polygonVertices[index], polygonVertices[(index + 1) % polygonVertices.Count], centerPosition }; // Create an array of vertices and set the first two to the current and next vertex and the third to the center position
            Vector3[] _normals;

            if (isMesh1)
            {
                _normals = new[] { -slicePlane.normal, -slicePlane.normal, -slicePlane.normal }; // Create an array of normals and set all three to the negative normal of the slice plane
            }
            else
            {
                _normals = new[] { slicePlane.normal, slicePlane.normal, slicePlane.normal }; // Set all three normals to the positive normal of the slice plane
            }

            MeshTriangleData fillTriangle = new MeshTriangleData(_vertices, _normals, calcScript.sentGameObjectMesh.subMeshCount + 1); // Create a new triangle using the previous arrays

            // If the Dot Cross product is negative then the triangle needs to be flipped
            if (Vector3.Dot(Vector3.Cross(_vertices[1] - _vertices[0], _vertices[2] - _vertices[0]), _normals[0]) < 0)
            {
                Vector3 temp = fillTriangle.vertices[2]; // Store the third vertex
                fillTriangle.vertices[2] = fillTriangle.vertices[0]; // Set the third vertex to the first vertex
                fillTriangle.vertices[0] = temp; // Set the first vertex to the previous third vertex position

                temp = fillTriangle.normals[2]; // Store the third normal
                fillTriangle.normals[2] = fillTriangle.normals[0]; // Set the third normal to the first normal
                fillTriangle.normals[0] = temp; // Set the first normal to the previous third normal position
            }

            mesh.AddTriangle(fillTriangle); // Add the triangle to the second mesh
        }
    }
}
