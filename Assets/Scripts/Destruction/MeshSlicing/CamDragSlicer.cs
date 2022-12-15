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
    public class CamDragSlicer : MonoBehaviour
    {
        // LINE RENDERER VARIABLES
        public GameObject ball;
        public LayerMask layerMask;
        private Collider _collider;

        // MESH CUTTING VARIABLES
        private Plane slicePlane;
        private Vector3 mousePoint;
        private bool slicing;
        public GameObject tip;
        public GameObject baseOfWeapon;
        private Vector3 enterPos;
        private Vector3 baseEnterPos;
        private Vector3 exitPos;
        private ParticleSystem ps;

        private void Awake()
        {
            _collider = ball.GetComponent<Collider>();
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                _collider.enabled = true;
                slicing = true;
            }
            else if (Input.GetMouseButtonUp(0))
            {
                _collider.enabled = false;
                slicing = false;
            }

            if (slicing)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit, 1000, layerMask))
                {
                    mousePoint = hit.point;
                    ball.transform.position = hit.point;
                }
            }
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

            if (hitGameObject.GetComponent<MeshSizeLimit>() == null)
            {
                ps = hitGameObject.GetComponentInChildren<ParticleSystem>();
                ps.Play();
                MeshCutCalculations calc = new MeshCutCalculations(); // Create a new mesh cut calculations object
                calc.CallScript(hitGameObject, slicePlane); // Call the mesh cut calculations script
            }
        }
    }
}
