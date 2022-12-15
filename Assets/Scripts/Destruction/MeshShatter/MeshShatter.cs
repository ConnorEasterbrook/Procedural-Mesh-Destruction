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
        [Range(1, 8)] public int shatterIterations = 4;
        private static Mesh gameObjectMesh;
        public DebugController debugController;

        private GameObject[] shatterGOList;
        private float _FJStrength = 100;

        void OnCollisionEnter(Collision collision)
        {
            if (gameObject.tag == "Weapon")
            {
                if (collision.gameObject.tag == "Sliceable" || collision.gameObject.tag == "Limb")
                {
                    ContactPoint contact = collision.GetContact(0);
                    shatterGOList = new GameObject[shatterIterations];

                    for (int i = 0; i < shatterIterations; i++)
                    {
                        Cut(collision.gameObject, i, contact);
                    }

                    gameObject.tag = "Untagged";
                }
            }
        }

        /// <summary>
        /// Cut the mesh using a plane
        /// </summary>
        public void Cut(GameObject hitObject, int iteration, ContactPoint _CP)
        {
            Plane slicePlane = new Plane(); // Create a new plane
            gameObjectMesh = hitObject.GetComponent<MeshFilter>().mesh; // Get the mesh of the game object

            Vector3 _CPPos = hitObject.transform.InverseTransformPoint(_CP.point);

            if (iteration == 0)
            {
                slicePlane = new Plane(UnityEngine.Random.onUnitSphere, _CPPos);
            }
            else
            {
                slicePlane = new Plane(UnityEngine.Random.onUnitSphere, new Vector3
                (
                    gameObjectMesh.bounds.min.x + gameObjectMesh.bounds.size.x / 2,
                    UnityEngine.Random.Range(gameObjectMesh.bounds.min.y, gameObjectMesh.bounds.max.y),
                    gameObjectMesh.bounds.min.z + gameObjectMesh.bounds.size.z / 2
                ));
            }

            MeshCutCalculations calc = new MeshCutCalculations(); // Create a new mesh cut calculations object
            calc.CallScript(hitObject, slicePlane); // Call the mesh cut calculations script
            GameObject newGameObject = calc.secondMeshGO; // Get the second game object from the mesh cut calculations script
            shatterGOList[iteration] = newGameObject;

            if (iteration == 0)
            {
                FixedJoint _FJ = newGameObject.AddComponent<FixedJoint>();
                _FJ.connectedBody = hitObject.GetComponent<Rigidbody>();
                _FJ.breakForce = _FJStrength;
                _FJ.breakTorque = _FJStrength;
                _FJ.massScale = 0.1f;
            }
            else
            {
                FixedJoint _FJ = newGameObject.AddComponent<FixedJoint>();
                _FJ.connectedBody = shatterGOList[iteration - 1].GetComponent<Rigidbody>();
                _FJ.breakForce = _FJStrength;
                _FJ.breakTorque = _FJStrength;
                _FJ.massScale = 0.1f;
            }

        }
    }
}