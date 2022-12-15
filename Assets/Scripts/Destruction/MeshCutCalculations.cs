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
using System.Linq;
using UnityEngine;
using Unity.Collections;
using Unity.Jobs;

namespace Connoreaster
{
    public class MeshCutCalculations
    {
        public Mesh sentGameObjectMesh;
        private Plane slicePlane;
        public GeneratedMeshData mesh1;
        public GeneratedMeshData mesh2;
        private List<Vector3> newVertices;
        private MeshTriangleData triangle;
        public GameObject secondMeshGO;

        public void CallScript(GameObject _sentGameObject, Plane _slicePlane)
        {
            sentGameObjectMesh = _sentGameObject.GetComponent<MeshFilter>().mesh;
            slicePlane = _slicePlane;
            mesh1 = new GeneratedMeshData(); // Create a new mesh data object for the first mesh
            mesh2 = new GeneratedMeshData(); // Create a new mesh data object for the second mesh
            newVertices = new List<Vector3>(); // Create a new list of vertices for the new mesh caused by slicing

            SeparateMeshes();
            
            MeshFillCalculations meshFillCalculations = new MeshFillCalculations(this, newVertices, slicePlane);
            meshFillCalculations.CallScript();

            MeshCreationCalculations meshCreationCalculations = new MeshCreationCalculations(this, mesh1.GetGeneratedMesh(), mesh2.GetGeneratedMesh(), secondMeshGO);
            secondMeshGO = meshCreationCalculations.CallScript(_sentGameObject);
        }

        private Vector3[][] vertsToAdd;
        private Vector3[][] normsToAdd;
        private bool[][] triangleLeftSide;
        int batchCount = 1;

        /// <summary>
        /// Iterates through the triangles of all the submeshes of the original mesh and splits them into two meshes
        /// </summary>
        private void SeparateMeshes()
        {
            int iterator = 0;

            if (sentGameObjectMesh.triangles.Length < 50)
            {
                batchCount = 128;
            }
            else if (sentGameObjectMesh.triangles.Length < 500)
            {
                batchCount = 64;
            }
            else if (sentGameObjectMesh.triangles.Length < 2000)
            {
                batchCount = 32;
            }
            else if (sentGameObjectMesh.triangles.Length < 10000)
            {
                batchCount = 1;
            }

            // Iterate through all the submeshes
            for (int i = 0; i < sentGameObjectMesh.subMeshCount; i++)
            {
                int[] hitGameObjectSubMeshTriangles = sentGameObjectMesh.GetTriangles(i); // Get the triangles of the submesh

                int subMeshTriangleCount = hitGameObjectSubMeshTriangles.Length / 3; // Get the number of triangles in the submesh

                vertsToAdd = new Vector3[subMeshTriangleCount][];
                normsToAdd = new Vector3[subMeshTriangleCount][];
                triangleLeftSide = new bool[subMeshTriangleCount][];

                SeparateMeshJob job = new SeparateMeshJob(i, sentGameObjectMesh, hitGameObjectSubMeshTriangles, slicePlane);
                JobHandle dependency = new JobHandle();
                JobHandle scheduleDependency = job.Schedule(subMeshTriangleCount, dependency);
                JobHandle scheduleParallelJobHandle = job.ScheduleParallel(subMeshTriangleCount, batchCount, scheduleDependency);
                scheduleParallelJobHandle.Complete();

                // Iterate through the submesh indices as triangles to determine which mesh to assign them to
                for (int j = 0; j < hitGameObjectSubMeshTriangles.Length; j += 3)
                {
                    iterator = j / 3;

                    vertsToAdd[iterator] = new Vector3[3];
                    normsToAdd[iterator] = new Vector3[3];
                    triangleLeftSide[iterator] = new bool[3];

                    vertsToAdd[iterator][0] = job.triVerts[j];
                    vertsToAdd[iterator][1] = job.triVerts[j + 1];
                    vertsToAdd[iterator][2] = job.triVerts[j + 2];

                    normsToAdd[iterator][0] = job.triNorms[j];
                    normsToAdd[iterator][1] = job.triNorms[j + 1];
                    normsToAdd[iterator][2] = job.triNorms[j + 2];

                    triangleLeftSide[iterator][0] = job.triangleLeftSide[j];
                    triangleLeftSide[iterator][1] = job.triangleLeftSide[j + 1];
                    triangleLeftSide[iterator][2] = job.triangleLeftSide[j + 2];

                    triangle = new MeshTriangleData(vertsToAdd[iterator], normsToAdd[iterator], i); // Create a new triangle with the job data

                    switch (triangleLeftSide[iterator][0])
                    {
                        // All three vertices are on one side of the plane
                        case true when triangleLeftSide[iterator][1] && triangleLeftSide[iterator][2]:
                            mesh1.AddTriangle(triangle);
                            break;

                        // All three vertices are on the other side of the plane.
                        case false when !triangleLeftSide[iterator][1] && !triangleLeftSide[iterator][2]:
                            mesh2.AddTriangle(triangle);
                            break;

                        // A triangle was cut through so now we need to calculate new triangles   
                        default:
                            CutTriangle(iterator);
                            break;
                    }
                }

                job.Dispose();
            }
        }

        /// <summary>
        /// Adds additional vertices to cut triangles to make them whole again
        /// </summary>
        private void CutTriangle(int iterator)
        {
            List<bool> belongsToMesh1 = new List<bool>(); // Stores whether the vertices belong to mesh1 (true) or mesh2 (false)
            belongsToMesh1.Add(triangleLeftSide[iterator][0]); // Add the first vertex to the list
            belongsToMesh1.Add(triangleLeftSide[iterator][1]); // Add the second vertex to the list
            belongsToMesh1.Add(triangleLeftSide[iterator][2]); // Add the third vertex to the list

            MeshTriangleData mesh1Triangle = new MeshTriangleData(new Vector3[2], new Vector3[2], triangle.subMeshIndex); // Stores the vertices of the first triangle
            MeshTriangleData mesh2Triangle = new MeshTriangleData(new Vector3[2], new Vector3[2], triangle.subMeshIndex); // Stores the vertices of the second triangle

            FindCorrectSides(mesh1Triangle, mesh2Triangle, belongsToMesh1);
            TestTriangles(mesh1Triangle, mesh2Triangle);
        }

        /// <summary>
        /// Finds the correct sides of the triangle to add to the correct submesh
        /// </summary>
        private void FindCorrectSides(MeshTriangleData mesh1Triangle, MeshTriangleData mesh2Triangle, List<bool> belongsToMesh1)
        {
            // Find the correct sides of the triangle
            bool mesh1Side = false;
            bool mesh2Side = false;

            // Iterate through each triangle vertex to find the mesh it falls on
            for (int i = 0; i < 3; i++)
            {
                // We need to reassign all triangle data in order for it to look correct. The third vertex will be the one already on the correct side
                if (belongsToMesh1[i])
                {
                    if (!mesh1Side)
                    {
                        mesh1Triangle.vertices[0] = triangle.vertices[i]; // Make the first vertex of the triangle match the current vertex iteration
                        mesh1Triangle.vertices[1] = mesh1Triangle.vertices[0]; // Add the second vertex of the triangle to match the first

                        mesh1Triangle.normals[0] = triangle.normals[i]; // Make the first normal of the triangle match the current normal iteration
                        mesh1Triangle.normals[1] = mesh1Triangle.normals[0]; // Add the second normal of the triangle to match the first

                        mesh1Side = true; // Set the mesh1Side to true so that we can increase the iteration and place the second vertex correctly.
                    }
                    else
                    {
                        mesh1Triangle.vertices[1] = triangle.vertices[i]; // Make the second vertex of the triangle match the current vertex iteration
                        mesh1Triangle.normals[1] = triangle.normals[i]; // Make the second normal of the triangle match the current normal iteration
                    }
                }
                else
                {
                    if (!mesh2Side)
                    {
                        mesh2Triangle.vertices[0] = triangle.vertices[i]; // Make the first vertex of the triangle match the current vertex iteration
                        mesh2Triangle.vertices[1] = mesh2Triangle.vertices[0]; // Add the second vertex of the triangle to match the first

                        mesh2Triangle.normals[0] = triangle.normals[i]; // Make the first normal of the triangle match the current normal iteration
                        mesh2Triangle.normals[1] = mesh2Triangle.normals[0]; // Add the second normal of the triangle to match the first

                        mesh2Side = true; // Set the mesh2Side to true so that we can increase the iteration and place the second vertex correctly.
                    }
                    else
                    {
                        mesh2Triangle.vertices[1] = triangle.vertices[i]; // Make the second vertex of the triangle match the current vertex iteration
                        mesh2Triangle.normals[1] = triangle.normals[i]; // Make the second normal of the triangle match the current normal iteration
                    }
                }
            }
        }

        /// <summary>
        /// Tests the triangles to see if they are valid and adds them to the correct submesh
        /// </summary>
        private void TestTriangles(MeshTriangleData mesh1Triangle, MeshTriangleData mesh2Triangle)
        {
            float normalizedDistance;
            float distance;

            // Get the triangle that was split by the slicePlane
            slicePlane.Raycast(new Ray(mesh1Triangle.vertices[0], (mesh2Triangle.vertices[0] - mesh1Triangle.vertices[0]).normalized), out distance); // Get the distance from the first vertex to the plane
            normalizedDistance = distance / (mesh2Triangle.vertices[0] - mesh1Triangle.vertices[0]).magnitude; // Get the normalized distance from the first vertex to the plane
            Vector3 mesh1Vert = Vector3.Lerp(mesh1Triangle.vertices[0], mesh2Triangle.vertices[0], normalizedDistance); // Get the vertex on the plane
            Vector3 mesh1Normal = Vector3.Lerp(mesh1Triangle.normals[0], mesh2Triangle.normals[0], normalizedDistance); // Get the normal on the plane
            newVertices.Add(mesh1Vert); // Add the vertex to the list of vertices

            slicePlane.Raycast(new Ray(mesh1Triangle.vertices[1], (mesh2Triangle.vertices[1] - mesh1Triangle.vertices[1]).normalized), out distance); // Get the distance from the second vertex to the plane
            normalizedDistance = distance / (mesh2Triangle.vertices[1] - mesh1Triangle.vertices[1]).magnitude; // Get the normalized distance from the second vertex to the plane
            Vector3 mesh2Vert = Vector3.Lerp(mesh1Triangle.vertices[1], mesh2Triangle.vertices[1], normalizedDistance); // Get the vertex on the plane
            Vector3 mesh2Normal = Vector3.Lerp(mesh1Triangle.normals[1], mesh2Triangle.normals[1], normalizedDistance); // Get the normal on the plane
            newVertices.Add(mesh2Vert); // Add the vertex to the list of vertices

            bool isEven = false;
            AddToMesh(mesh1, mesh1Triangle, mesh1Vert, mesh2Vert, mesh1Normal, mesh2Normal, isEven); // Add the triangle to the mesh
            AddToMesh(mesh2, mesh2Triangle, mesh1Vert, mesh2Vert, mesh1Normal, mesh2Normal, isEven); // Add the triangle to the mesh

            isEven = true;
            AddToMesh(mesh1, mesh1Triangle, mesh2Vert, mesh1Vert, mesh2Normal, mesh1Normal, isEven); // Add the triangle to the mesh
            AddToMesh(mesh2, mesh2Triangle, mesh2Vert, mesh1Vert, mesh2Normal, mesh1Normal, isEven); // Add the triangle to the mesh
        }

        /// <summary>
        /// An obtuse function to cut down on code size. Assigns the triangle data to the correct mesh
        /// </summary>
        private void AddToMesh(GeneratedMeshData mesh, MeshTriangleData meshTriangle, Vector3 meshVert, Vector3 _meshVert, Vector3 meshNormal, Vector3 _meshNormal, bool isEven)
        {
            // Triangle test variables
            Vector3[] updatedVertices; // Create a new array of vertices for the triangle
            Vector3[] updatedNormals; // Create a new array of normals for the triangle
            MeshTriangleData testTriangle; // Create a new triangle with the updated data

            // Conditional to determine which calculations to use
            if (isEven)
            {
                updatedVertices = new Vector3[] { meshTriangle.vertices[0], meshTriangle.vertices[1], meshVert };
                updatedNormals = new Vector3[] { meshTriangle.normals[0], meshTriangle.normals[1], meshNormal };
                testTriangle = new MeshTriangleData(updatedVertices, updatedNormals, triangle.subMeshIndex);
            }
            else
            {
                updatedVertices = new Vector3[] { meshTriangle.vertices[0], meshVert, _meshVert };
                updatedNormals = new Vector3[] { meshTriangle.normals[0], meshNormal, _meshNormal };
                testTriangle = new MeshTriangleData(updatedVertices, updatedNormals, triangle.subMeshIndex);
            }

            // If our vertices are not the same, then we can add the triangle to the mesh
            if (updatedVertices[0] != updatedVertices[1] && updatedVertices[0] != updatedVertices[2])
            {
                // If the Dot Cross product is negative then the triangle needs to be flipped
                if (Vector3.Dot(Vector3.Cross(updatedVertices[1] - updatedVertices[0], updatedVertices[2] - updatedVertices[0]), updatedNormals[0]) < 0)
                {
                    Vector3 temp = testTriangle.vertices[2]; // Store the third vertex
                    testTriangle.vertices[2] = testTriangle.vertices[0]; // Set the third vertex to the first vertex
                    testTriangle.vertices[0] = temp; // Set the first vertex to the previous third vertex position

                    temp = testTriangle.normals[2]; // Store the third normal
                    testTriangle.normals[2] = testTriangle.normals[0]; // Set the third normal to the first normal
                    testTriangle.normals[0] = temp; // Set the first normal to the previous third normal position
                }

                mesh.AddTriangle(testTriangle); // Add the triangle to the mesh
            }
        }
    }
}
