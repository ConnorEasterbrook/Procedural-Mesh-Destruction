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
    public class MeshCutCalculations : MonoBehaviour
    {
        private static Mesh sentGameObjectMesh;
        private Plane slicePlane;
        private GeneratedMeshData mesh1;
        private GeneratedMeshData mesh2;
        private List<Vector3> newVertices;
        private MeshTriangleData triangle;
        private GameObject secondMeshGO;

        public void CallScript(GameObject _sentGameObject, Plane _slicePlane)
        {
            sentGameObjectMesh = _sentGameObject.GetComponent<MeshFilter>().mesh;
            slicePlane = _slicePlane;
            mesh1 = new GeneratedMeshData(); // Create a new mesh data object for the first mesh
            mesh2 = new GeneratedMeshData(); // Create a new mesh data object for the second mesh
            newVertices = new List<Vector3>(); // Create a new list of vertices for the new mesh caused by slicing

            SeparateMeshes(mesh1, mesh2); // Separate the meshes
            BeginFill();
            CreateFirstMesh(_sentGameObject);
        }

        /// <summary>
        /// Iterates through the triangles of all the submeshes of the original mesh and splits them into two meshes
        /// </summary>
        private void SeparateMeshes(GeneratedMeshData mesh1, GeneratedMeshData mesh2)
        {
            // Iterate through all the submeshes
            for (int i = 0; i < sentGameObjectMesh.subMeshCount; i++)
            {
                int[] hitGameObjectSubMeshTriangles = sentGameObjectMesh.GetTriangles(i); // Get the triangles of the submesh

                // Iterate through the submesh indices as triangles to determine which mesh to assign them to
                for (int j = 0; j < hitGameObjectSubMeshTriangles.Length; j += 3)
                {
                    int triangleIndexA = hitGameObjectSubMeshTriangles[j]; // Get the first index of the triangle
                    int triangleIndexB = hitGameObjectSubMeshTriangles[j + 1]; // Get the second index of the triangle
                    int triangleIndexC = hitGameObjectSubMeshTriangles[j + 2]; // Get the third index of the triangle

                    // Get the data of the triangle
                    triangle = GetTriangle
                    (
                        GetVerticesToAdd(triangleIndexA, triangleIndexB, triangleIndexC),
                        GetNormalsToAdd(triangleIndexA, triangleIndexB, triangleIndexC),
                        GetUVsToAdd(triangleIndexA, triangleIndexB, triangleIndexC),
                        i
                    );

                    // Check what side the submesh triangle is on the slicePlane and if it has been sliced through
                    bool triangleALeftSide = slicePlane.GetSide(sentGameObjectMesh.vertices[triangleIndexA]); // Check if the first vertex of the triangle is on the left side of the plane
                    bool triangleBLeftSide = slicePlane.GetSide(sentGameObjectMesh.vertices[triangleIndexB]); // Check if the second vertex of the triangle is on the left side of the plane
                    bool triangleCLeftSide = slicePlane.GetSide(sentGameObjectMesh.vertices[triangleIndexC]); // Check if the third vertex of the triangle is on the left side of the plane
                    switch (triangleALeftSide)
                    {
                        // All three vertices are on one side of the plane
                        case true when triangleBLeftSide && triangleCLeftSide:
                            mesh1.AddTriangle(triangle);
                            break;

                        // All three vertices are on the other side of the plane.
                        case false when !triangleBLeftSide && !triangleCLeftSide:
                            mesh2.AddTriangle(triangle);
                            break;

                        // A triangle was cut through so now we need to calculate new triangles   
                        default:
                            CutTriangle(triangleALeftSide, triangleBLeftSide, triangleCLeftSide);
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Gets the triangle data and stores it in a MeshTriangleData struct
        /// </summary>
        private static MeshTriangleData GetTriangle(Vector3[] vertices, Vector3[] normals, Vector2[] uvs, int submeshIndex)
        {
            MeshTriangleData triangle = new MeshTriangleData(vertices, normals, uvs, submeshIndex);
            return triangle;
        }

        /// <summary>
        /// Gets the vertices of the triangle
        /// </summary>
        private static Vector3[] GetVerticesToAdd(int triangleIndexA, int triangleIndexB, int triangleIndexC)
        {
            Vector3 vertexA = sentGameObjectMesh.vertices[triangleIndexA];
            Vector3 vertexB = sentGameObjectMesh.vertices[triangleIndexB];
            Vector3 vertexC = sentGameObjectMesh.vertices[triangleIndexC];

            Vector3[] verticesToAdd = { vertexA, vertexB, vertexC };
            return verticesToAdd;
        }

        /// <summary>
        /// Gets the normals of the triangle
        /// </summary>
        private static Vector3[] GetNormalsToAdd(int triangleIndexA, int triangleIndexB, int triangleIndexC)
        {
            Vector3 normalA = sentGameObjectMesh.normals[triangleIndexA];
            Vector3 normalB = sentGameObjectMesh.normals[triangleIndexB];
            Vector3 normalC = sentGameObjectMesh.normals[triangleIndexC];

            Vector3[] normalsToAdd = { normalA, normalB, normalC };
            return normalsToAdd;
        }

        /// <summary>
        /// Gets the UVs of the triangle
        /// </summary>
        private static Vector2[] GetUVsToAdd(int triangleIndexA, int triangleIndexB, int triangleIndexC)
        {
            Vector2 uvA = sentGameObjectMesh.uv[triangleIndexA];
            Vector2 uvB = sentGameObjectMesh.uv[triangleIndexB];
            Vector2 uvC = sentGameObjectMesh.uv[triangleIndexC];

            Vector2[] uvsToAdd = { uvA, uvB, uvC };
            return uvsToAdd;
        }

        /// <summary>
        /// Adds additional vertices to cut triangles to make them whole again
        /// </summary>
        private void CutTriangle(bool triangleALeftSide, bool triangleBLeftSide, bool triangleCLeftSide)
        {
            List<bool> belongsToMesh1 = new List<bool>(); // Stores whether the vertices belong to mesh1 (true) or mesh2 (false)
            belongsToMesh1.Add(triangleALeftSide); // Add the first vertex to the list
            belongsToMesh1.Add(triangleBLeftSide); // Add the second vertex to the list
            belongsToMesh1.Add(triangleCLeftSide); // Add the third vertex to the list

            MeshTriangleData mesh1Triangle = new MeshTriangleData(new Vector3[2], new Vector3[2], new Vector2[2], triangle.SubmeshIndex); // Stores the vertices of the first triangle
            MeshTriangleData mesh2Triangle = new MeshTriangleData(new Vector3[2], new Vector3[2], new Vector2[2], triangle.SubmeshIndex); // Stores the vertices of the second triangle

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
                        mesh1Triangle.Vertices[0] = triangle.Vertices[i]; // Make the first vertex of the triangle match the current vertex iteration
                        mesh1Triangle.Vertices[1] = mesh1Triangle.Vertices[0]; // Add the second vertex of the triangle to match the first

                        mesh1Triangle.Normals[0] = triangle.Normals[i]; // Make the first normal of the triangle match the current normal iteration
                        mesh1Triangle.Normals[1] = mesh1Triangle.Normals[0]; // Add the second normal of the triangle to match the first

                        mesh1Triangle.UVs[0] = triangle.UVs[i]; // Make the first UV of the triangle match the current UV iteration
                        mesh1Triangle.UVs[1] = mesh1Triangle.UVs[0]; // Add the second UV of the triangle to match the first

                        mesh1Side = true; // Set the mesh1Side to true so that we can increase the iteration and place the second vertex correctly.
                    }
                    else
                    {
                        mesh1Triangle.Vertices[1] = triangle.Vertices[i]; // Make the second vertex of the triangle match the current vertex iteration
                        mesh1Triangle.Normals[1] = triangle.Normals[i]; // Make the second normal of the triangle match the current normal iteration
                        mesh1Triangle.UVs[1] = triangle.UVs[i]; // Make the second UV of the triangle match the current UV iteration
                    }
                }
                else
                {
                    if (!mesh2Side)
                    {
                        mesh2Triangle.Vertices[0] = triangle.Vertices[i]; // Make the first vertex of the triangle match the current vertex iteration
                        mesh2Triangle.Vertices[1] = mesh2Triangle.Vertices[0]; // Add the second vertex of the triangle to match the first

                        mesh2Triangle.Normals[0] = triangle.Normals[i]; // Make the first normal of the triangle match the current normal iteration
                        mesh2Triangle.Normals[1] = mesh2Triangle.Normals[0]; // Add the second normal of the triangle to match the first

                        mesh2Triangle.UVs[0] = triangle.UVs[i]; // Make the first UV of the triangle match the current UV iteration
                        mesh2Triangle.UVs[1] = mesh2Triangle.UVs[0]; // Add the second UV of the triangle to match the first

                        mesh2Side = true; // Set the mesh2Side to true so that we can increase the iteration and place the second vertex correctly.
                    }
                    else
                    {
                        mesh2Triangle.Vertices[1] = triangle.Vertices[i]; // Make the second vertex of the triangle match the current vertex iteration
                        mesh2Triangle.Normals[1] = triangle.Normals[i]; // Make the second normal of the triangle match the current normal iteration
                        mesh2Triangle.UVs[1] = triangle.UVs[i]; // Make the second UV of the triangle match the current UV iteration
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
            slicePlane.Raycast(new Ray(mesh1Triangle.Vertices[0], (mesh2Triangle.Vertices[0] - mesh1Triangle.Vertices[0]).normalized), out distance); // Get the distance from the first vertex to the plane
            normalizedDistance = distance / (mesh2Triangle.Vertices[0] - mesh1Triangle.Vertices[0]).magnitude; // Get the normalized distance from the first vertex to the plane
            Vector3 mesh1Vert = Vector3.Lerp(mesh1Triangle.Vertices[0], mesh2Triangle.Vertices[0], normalizedDistance); // Get the vertex on the plane
            Vector3 mesh1Normal = Vector3.Lerp(mesh1Triangle.Normals[0], mesh2Triangle.Normals[0], normalizedDistance); // Get the normal on the plane
            Vector2 mesh1UV = Vector2.Lerp(mesh1Triangle.UVs[0], mesh2Triangle.UVs[0], normalizedDistance); // Get the UV on the plane
            newVertices.Add(mesh1Vert); // Add the vertex to the list of vertices

            slicePlane.Raycast(new Ray(mesh1Triangle.Vertices[1], (mesh2Triangle.Vertices[1] - mesh1Triangle.Vertices[1]).normalized), out distance); // Get the distance from the second vertex to the plane
            normalizedDistance = distance / (mesh2Triangle.Vertices[1] - mesh1Triangle.Vertices[1]).magnitude; // Get the normalized distance from the second vertex to the plane
            Vector3 mesh2Vert = Vector3.Lerp(mesh1Triangle.Vertices[1], mesh2Triangle.Vertices[1], normalizedDistance); // Get the vertex on the plane
            Vector3 mesh2Normal = Vector3.Lerp(mesh1Triangle.Normals[1], mesh2Triangle.Normals[1], normalizedDistance); // Get the normal on the plane
            Vector2 mesh2UV = Vector2.Lerp(mesh1Triangle.UVs[1], mesh2Triangle.UVs[1], normalizedDistance); // Get the UV on the plane
            newVertices.Add(mesh2Vert); // Add the vertex to the list of vertices

            bool isEven = false;
            AddToMesh(mesh1, mesh1Triangle, mesh1Vert, mesh2Vert, mesh1Normal, mesh2Normal, mesh1UV, mesh2UV, isEven); // Add the triangle to the mesh
            AddToMesh(mesh2, mesh2Triangle, mesh1Vert, mesh2Vert, mesh1Normal, mesh2Normal, mesh1UV, mesh2UV, isEven); // Add the triangle to the mesh

            isEven = true;
            AddToMesh(mesh1, mesh1Triangle, mesh2Vert, mesh1Vert, mesh2Normal, mesh1Normal, mesh2UV, mesh1UV, isEven); // Add the triangle to the mesh
            AddToMesh(mesh2, mesh2Triangle, mesh2Vert, mesh1Vert, mesh2Normal, mesh1Normal, mesh2UV, mesh1UV, isEven); // Add the triangle to the mesh
        }

        /// <summary>
        /// An obtuse function to cut down on code size. Assigns the triangle data to the correct mesh
        /// </summary>
        private void AddToMesh(GeneratedMeshData mesh, MeshTriangleData meshTriangle, Vector3 meshVert, Vector3 _meshVert, Vector3 meshNormal, Vector3 _meshNormal, Vector2 meshUV, Vector2 _meshUV, bool isEven)
        {
            // Triangle test variables
            Vector3[] updatedVertices; // Create a new array of vertices for the triangle
            Vector3[] updatedNormals; // Create a new array of normals for the triangle
            Vector2[] updatedUVs; // Create a new array of UVs for the triangle
            MeshTriangleData testTriangle; // Create a new triangle with the updated data

            // Conditional to determine which calculations to use
            if (isEven)
            {
                updatedVertices = new Vector3[] { meshTriangle.Vertices[0], meshTriangle.Vertices[1], meshVert };
                updatedNormals = new Vector3[] { meshTriangle.Normals[0], meshTriangle.Normals[1], meshNormal };
                updatedUVs = new Vector2[] { meshTriangle.UVs[0], meshTriangle.UVs[1], meshUV };
                testTriangle = new MeshTriangleData(updatedVertices, updatedNormals, updatedUVs, triangle.SubmeshIndex);
            }
            else
            {
                updatedVertices = new Vector3[] { meshTriangle.Vertices[0], meshVert, _meshVert };
                updatedNormals = new Vector3[] { meshTriangle.Normals[0], meshNormal, _meshNormal };
                updatedUVs = new Vector2[] { meshTriangle.UVs[0], meshUV, _meshUV };
                testTriangle = new MeshTriangleData(updatedVertices, updatedNormals, updatedUVs, triangle.SubmeshIndex);
            }

            // If our vertices are not the same, then we can add the triangle to the mesh
            if (updatedVertices[0] != updatedVertices[1] && updatedVertices[0] != updatedVertices[2])
            {
                // If the Dot Cross product is negative then the triangle needs to be flipped
                if (Vector3.Dot(Vector3.Cross(updatedVertices[1] - updatedVertices[0], updatedVertices[2] - updatedVertices[0]), updatedNormals[0]) < 0)
                {
                    FlipTriangle(testTriangle); // Flip the triangle
                }

                mesh.AddTriangle(testTriangle); // Add the triangle to the mesh
            }
        }

        /// <summary>
        /// Flips the triangle by swapping the first and third vertices
        /// </summary>
        private static void FlipTriangle(MeshTriangleData testTriangle)
        {
            Vector3 temp = testTriangle.Vertices[2]; // Store the third vertex
            testTriangle.Vertices[2] = testTriangle.Vertices[0]; // Set the third vertex to the first vertex
            testTriangle.Vertices[0] = temp; // Set the first vertex to the previous third vertex position

            temp = testTriangle.Normals[2]; // Store the third normal
            testTriangle.Normals[2] = testTriangle.Normals[0]; // Set the third normal to the first normal
            testTriangle.Normals[0] = temp; // Set the first normal to the previous third normal position
        }

        /// <summary>
        /// Begin the process of filling in the cross section of the sliced mesh
        /// </summary>
        public void BeginFill()
        {
            List<Vector3> vertices = new List<Vector3>(); // Create a list of vertices
            List<Vector3> polygon = new List<Vector3>(); // Create a list of vertices for the polygon

            // Loop through all the vertices in the mesh
            for (int i = 0; i < newVertices.Count; i++)
            {
                // If the vertex is not in the list of vertices
                if (!vertices.Contains(newVertices[i]))
                {
                    polygon.Clear(); // Clear the polygon list
                    polygon.Add(newVertices[i]); // Add the vertex to the polygon list
                    polygon.Add(newVertices[i + 1]); // Add the next vertex to the polygon list

                    vertices.Add(newVertices[i]); // Add the vertex to the list of vertices
                    vertices.Add(newVertices[i + 1]); // Add the next vertex to the list of vertices

                    EvaluatePairs(vertices, polygon); // Evaluate the pairs of vertices
                    Fill(polygon); // Fill the polygon
                }
            }
        }

        /// <summary>
        /// Evaluate the pairs of vertices
        /// </summary>
        public void EvaluatePairs(List<Vector3> vertices, List<Vector3> polygon)
        {
            bool isDone = false; // Create a boolean to determine if the loop is done
            while (!isDone)
            {
                isDone = true; // Set the boolean to true so that it only loops once if no pairs are found

                // Loop through all the vertices in the mesh
                for (int i = 0; i < newVertices.Count; i += 2)
                {
                    // If the first vertex is in the polygon and the second vertex is not. Else if the second vertex is in the polygon and the first vertex is not
                    if (newVertices[i] == polygon[polygon.Count - 1] && !vertices.Contains(newVertices[i + 1]))
                    {
                        isDone = false;
                        polygon.Add(newVertices[i + 1]); // Add the next vertex to the polygon list
                        vertices.Add(newVertices[i + 1]); // Add the next vertex to the list of vertices
                    }
                    else if (newVertices[i + 1] == polygon[polygon.Count - 1] && !vertices.Contains(newVertices[i]))
                    {
                        isDone = false;
                        polygon.Add(newVertices[i]); // Add the next vertex to the polygon list
                        vertices.Add(newVertices[i]); // Add the next vertex to the list of vertices
                    }
                }
            }
        }

        /// <summary>
        /// Fill the polygon
        /// </summary>
        private void Fill(List<Vector3> vertices)
        {
            Vector3 centerPosition = Vector3.zero; // Create a new vector for the center position

            // Loop through all the vertices in the polygon
            for (int i = 0; i < vertices.Count; i++)
            {
                centerPosition += vertices[i];
            }

            centerPosition /= vertices.Count; // Set the center position to the average of all the vertices

            // Create an upward axis used to determine the orientation of the plane
            Vector3 up = new Vector3()
            {
                x = slicePlane.normal.x,
                y = slicePlane.normal.y,
                z = slicePlane.normal.z
            };

            Vector3 left = Vector3.Cross(slicePlane.normal, up); // Create a left axis used to determine the orientation of the plane

            Vector3 displacement = Vector3.zero; // Create a new vector for the displacement
            Vector2 uv1 = Vector2.zero; // Create a new vector for the first UV
            Vector2 uv2 = Vector2.zero; // Create a new vector for the second UV

            // Loop through all the vertices in the polygon
            for (int i = 0; i < vertices.Count; i++)
            {
                displacement = vertices[i] - centerPosition; // Set the displacement to the difference between the vertex and the center position

                // Set the UVs to the dot product of the displacement and the left and up axes
                uv1 = new Vector2()
                {
                    x = .5f + Vector3.Dot(displacement, left),
                    y = .5f + Vector3.Dot(displacement, up)
                };

                displacement = vertices[(i + 1) % vertices.Count] - centerPosition; // Set the displacement to the difference between the next vertex and the center position

                // Set the UVs to the dot product of the displacement and the left and up axes
                uv2 = new Vector2()
                {
                    x = .5f + Vector3.Dot(displacement, left),
                    y = .5f + Vector3.Dot(displacement, up)
                };

                bool isMesh1 = true;
                CheckFillFlip(mesh1, vertices, centerPosition, uv1, uv2, isMesh1, i); // Check if the fill should be flipped for the first mesh

                isMesh1 = false;
                CheckFillFlip(mesh2, vertices, centerPosition, uv1, uv2, isMesh1, i); // Check if the fill should be flipped for the second mesh
            }
        }

        /// <summary>
        /// Check if the fill should be flipped
        /// </summary>
        private void CheckFillFlip(GeneratedMeshData mesh, List<Vector3> vertices, Vector3 centerPosition, Vector2 uv1, Vector2 uv2, bool isMesh1, int index)
        {
            Vector3[] _vertices = { vertices[index], vertices[(index + 1) % vertices.Count], centerPosition }; // Create an array of vertices and set the first two to the current and next vertex and the third to the center position
            Vector3[] _normals;
            Vector2[] _uvs = { uv1, uv2, new(0.5f, 0.5f) }; // Create an array of UVs and set the first two to the UVs and the third to the center UV

            if (isMesh1)
            {
                _normals = new[] { -slicePlane.normal, -slicePlane.normal, -slicePlane.normal }; // Create an array of normals and set all three to the negative normal of the slice plane
            }
            else
            {
                _normals = new[] { slicePlane.normal, slicePlane.normal, slicePlane.normal }; // Set all three normals to the positive normal of the slice plane
            }

            MeshTriangleData fillTriangle = new MeshTriangleData(_vertices, _normals, _uvs, sentGameObjectMesh.subMeshCount + 1); // Create a new triangle using the previous arrays

            // If the Dot Cross product is negative then the triangle needs to be flipped
            if (Vector3.Dot(Vector3.Cross(_vertices[1] - _vertices[0], _vertices[2] - _vertices[0]), _normals[0]) < 0)
            {
                FlipTriangle(fillTriangle); // Flip the triangle
            }

            mesh.AddTriangle(fillTriangle); // Add the triangle to the second mesh
        }

        /// <summary>
        /// Create the new meshes after the slicing
        /// </summary>
        private void CreateFirstMesh(GameObject hitGameObject)
        {
            Mesh completeMesh1 = mesh1.GetGeneratedMesh();

            // Remove all current colliders on the object to avoid duplicates and update the mesh bounds
            Collider[] originalCols = hitGameObject.GetComponents<Collider>();
            foreach (Collider col in originalCols)
            {
                Destroy(col);
            }

            hitGameObject.GetComponent<MeshFilter>().mesh = completeMesh1; // Set the first object's mesh to the first mesh
            var collider = hitGameObject.AddComponent<MeshCollider>(); // Add a mesh collider to the first object
            collider.sharedMesh = completeMesh1; // Set the collider's mesh to the first mesh
            collider.convex = true; // Set the collider to convex since convex colliders are faster

            Material[] mats = new Material[completeMesh1.subMeshCount]; // Create a new array of materials

            // Loop through all the materials in the original mesh
            for (int i = 0; i < completeMesh1.subMeshCount; i++)
            {
                mats[i] = hitGameObject.GetComponent<MeshRenderer>().material; // Set the material to the original material
            }
            hitGameObject.GetComponent<MeshRenderer>().materials = mats; // Set the materials to the new array of materials

            CreateSecondMesh(hitGameObject, mats);
        }

        private void CreateSecondMesh(GameObject hitGameObject, Material[] mats)
        {
            Mesh completeMesh2 = mesh2.GetGeneratedMesh();

            secondMeshGO = new GameObject(); // Create a new game object for the second mesh
            secondMeshGO.tag = "Sliceable"; // Set the tag to sliceable
            secondMeshGO.transform.position = hitGameObject.transform.position + (Vector3.up * .05f); // Set the position of the second mesh to the position of the first mesh plus a small offset
            secondMeshGO.transform.rotation = hitGameObject.transform.rotation; // Set the rotation of the second mesh to the rotation of the first mesh
            secondMeshGO.transform.localScale = hitGameObject.transform.localScale; // Set the scale of the second mesh to the scale of the first mesh
            secondMeshGO.AddComponent<MeshRenderer>(); // Add a mesh renderer to the second mesh

            mats = new Material[completeMesh2.subMeshCount]; // Create a new array of materials

            // Loop through all the materials in the original mesh
            for (int i = 0; i < completeMesh2.subMeshCount; i++)
            {
                mats[i] = hitGameObject.GetComponent<MeshRenderer>().material; // Set the material to the original material
            }
            secondMeshGO.GetComponent<MeshRenderer>().materials = mats; // Set the materials to the new array of materials
            secondMeshGO.AddComponent<MeshFilter>().mesh = completeMesh2; // Add a mesh filter to the second mesh and set the mesh to the second mesh

            secondMeshGO.AddComponent<MeshCollider>().sharedMesh = completeMesh2; // Add a mesh collider to the second mesh and set the mesh to the second mesh
            var cols = secondMeshGO.GetComponents<MeshCollider>(); // Get all the mesh colliders on the second mesh and set them to convex since convex colliders are faster
            foreach (var col in cols)
            {
                col.convex = true;
            }

            AddRigidBody(secondMeshGO); // Add a rigidbody to the second mesh
        }

        private void AddRigidBody(GameObject secondMesh)
        {
            Rigidbody rightRigidbody = secondMesh.AddComponent<Rigidbody>(); // Add a rigidbody to the second mesh
            rightRigidbody.AddRelativeForce(-slicePlane.normal * 250f); // Add a force to the second mesh in the opposite direction of the slice plane for effect
        }
        public void AddShatter()
        {
            secondMeshGO.AddComponent<MeshShatter>(); // Add a mesh shatter script to the second mesh
            secondMeshGO.AddComponent<MeshSizeLimit>().isPlane = GetComponent<MeshSizeLimit>().isPlane; // Add a mesh size limit script to the second mesh
        }
    }
}
