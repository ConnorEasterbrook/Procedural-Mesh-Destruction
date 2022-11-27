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
    public class FPCharSlice : MonoBehaviour
    {
        private static Mesh hitGameObjectMesh;
        private Plane slicePlane;
        private GeneratedMeshData mesh1;
        private GeneratedMeshData mesh2;
        private List<Vector3> newVertices;
        private MeshTriangleData triangle;

        public float explodeForce = 250f;
        public bool debugColour = false;



        public GameObject tip;
        public GameObject baseOfWeapon;
        private Vector3 enterPos;
        private Vector3 baseEnterPos;
        private Vector3 exitPos;

        private void Update()
        {
            // if (Input.GetMouseButtonDown(0))
            // {
            //     enterPos = tip.transform.position;
            //     baseEnterPos = baseOfWeapon.transform.position;
            // }
            // if (Input.GetMouseButtonUp(0))
            // {
            //     exitPos = tip.transform.position;
            //     Cut();
            // }
        }

        private void OnTriggerEnter(Collider other)
        {
            enterPos = tip.transform.position;
            baseEnterPos = baseOfWeapon.transform.position;
        }

        private void OnTriggerExit(Collider other)
        {
            exitPos = tip.transform.position;

            //Create a triangle between the tip and base so that we can get the normal
            Vector3 side1 = exitPos - enterPos;
            Vector3 side2 = exitPos - baseEnterPos;

            //Get the point perpendicular to the triangle above which is the normal
            Vector3 normal = Vector3.Cross(side1, side2).normalized;

            //Transform the normal so that it is aligned with the object we are slicing's transform.
            Vector3 transformedNormal = ((Vector3)(other.gameObject.transform.localToWorldMatrix.transpose * normal)).normalized;

            //Get the enter position relative to the object we're cutting's local transform
            Vector3 transformedStartingPoint = other.gameObject.transform.InverseTransformPoint(enterPos);

            slicePlane = new Plane();

            slicePlane.SetNormalAndPosition(
                    transformedNormal,
                    transformedStartingPoint);

            var direction = Vector3.Dot(Vector3.up, transformedNormal);

            //Flip the plane so that we always know which side the positive mesh is on
            if (direction < 0)
            {
                slicePlane = slicePlane.flipped;
            }

            // GameObject[] slices = Slicer.Slice(plane, other.gameObject);
            Cut(other.gameObject);
        }

        /// <summary>
        /// Cut the mesh using a plane
        /// </summary>
        private void Cut(GameObject hitGameObject)
        {
            // Ensure the hit gameObject is sliceable
            if (hitGameObject.tag != "Sliceable")
            {
                return;
            }

            MeshCutCalculations calc = new MeshCutCalculations(); // Create a new mesh cut calculations object
            calc.CallScript(hitGameObject, slicePlane, explodeForce, debugColour); // Call the mesh cut calculations script
        }
    }
}
