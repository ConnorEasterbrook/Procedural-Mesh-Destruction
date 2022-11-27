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
    public class MeshShatter : MonoBehaviour
    {
        [Range(1, 8)] public float shatterIterations = 4;
        private bool isShattered = false;
        public float explodeForce = 250f;
        public bool debugColour = false;

        private static Mesh gameObjectMesh;
        private Plane slicePlane;
        private GeneratedMeshData mesh1;
        private GeneratedMeshData mesh2;
        private List<Vector3> newVertices;
        private MeshTriangleData triangle;

        // Update is called once per frame
        void Update()
        {
            //     if (Input.GetMouseButtonDown(0))
            //     {
            //         if (!isShattered)
            //         {
            //             for (int i = 0; i < shatterIterations; i++)
            //             {
            //                 Cut();
            //             }

            //             isShattered = true;
            //         }
            //     }
        }

        void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.tag == "Projectile")
            {
                if (!isShattered)
                {
                    for (int i = 0; i < shatterIterations; i++)
                    {
                        Cut();
                    }

                    Destroy(gameObject);
                    Destroy(collision.gameObject);
                    // isShattered = true;
                }
            }
        }

        /// <summary>
        /// Cut the mesh using a plane
        /// </summary>
        public void Cut()
        {
            gameObjectMesh = GetComponent<MeshFilter>().mesh; // Get the mesh of the game object

            // Create a plane at a random point on the mesh
            slicePlane = new Plane(UnityEngine.Random.onUnitSphere, new Vector3
            (
                gameObjectMesh.bounds.min.x + gameObjectMesh.bounds.size.x / 2,
                UnityEngine.Random.Range(gameObjectMesh.bounds.min.y, gameObjectMesh.bounds.max.y), 
                gameObjectMesh.bounds.min.z + gameObjectMesh.bounds.size.z / 2
            ));

            MeshCutCalculations calc = new MeshCutCalculations(); // Create a new mesh cut calculations object
            calc.CallScript(gameObject, slicePlane, explodeForce, debugColour); // Call the mesh cut calculations script
        }
    }
}