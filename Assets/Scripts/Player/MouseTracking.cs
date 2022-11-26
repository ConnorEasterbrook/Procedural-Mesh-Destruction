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
    public class MouseTracking : MonoBehaviour
    {
        [Header("Track Movement")]
        public bool trackMouseMovement = false; // Set to true to track mouse movement
        public GameObject mouseTrackingObject; // Set the object to track the mouse movement
        public float trackSensitivity = 50f; // Set the sensitivity of the mouse tracking
        public FPCharSlice FPSliceScript; // Set the FPSlice script
        private Vector3 mouseDownPos; // Set the mouse down position

        [Header("Ray Point")]
        public bool rayPoint = false; // Set to true to raycast a point
        public GameObject rayPointObject; // Set the object to raycast a point

        // Update is called once per frame
        void Update()
        {
            if (trackMouseMovement)
            {
                TrackMouseMovement();
            }

            if (rayPoint)
            {
                RayPoint();
            }
        }

        private void TrackMouseMovement()
        {
            // Set the mouse down position on mouse down
            if (Input.GetMouseButtonDown(0))
            {
                mouseDownPos = Input.mousePosition;
            }

            // On mouse up calculate the direction of the mouse movement
            if (Input.GetMouseButtonUp(0))
            {
                float swipeDistanceX = (Input.mousePosition - mouseDownPos).x; // Calculate the distance of the mouse movement on the x axis
                float swipeDistanceY = (Input.mousePosition - mouseDownPos).y; // Calculate the distance of the mouse movement on the y axis

                if (Input.mousePosition.y > mouseDownPos.y && swipeDistanceY > trackSensitivity)
                {
                    if (Input.mousePosition.x > mouseDownPos.x && swipeDistanceX > trackSensitivity)
                    {
                        mouseTrackingObject.transform.localRotation = Quaternion.Euler(0, 0, -45); // Rotate the object to the top right

                        // Vector3 rotation = new Vector3(Camera.main.transform.rotation.eulerAngles.x, Camera.main.transform.rotation.eulerAngles.y, -45); // Set the rotation of the mouse tracking object
                        // FPSliceScript.Slice(mouseTrackingObject, rotation); // Cut the object
                    }
                    else if (Input.mousePosition.x < mouseDownPos.x && swipeDistanceX < -trackSensitivity)
                    {
                        mouseTrackingObject.transform.localRotation = Quaternion.Euler(0, 0, 45); // Rotate the object to the top left
                    }
                    else
                    {
                        mouseTrackingObject.transform.localRotation = Quaternion.Euler(0, 0, 0); // Rotate the object to the top
                    }
                }
                else if (Input.mousePosition.y < mouseDownPos.y && swipeDistanceY < -trackSensitivity)
                {
                    if (Input.mousePosition.x > mouseDownPos.x && swipeDistanceX > trackSensitivity)
                    {
                        mouseTrackingObject.transform.localRotation = Quaternion.Euler(0, 0, -135); // Rotate the object to the bottom right
                    }
                    else if (Input.mousePosition.x < mouseDownPos.x && swipeDistanceX < -trackSensitivity)
                    {
                        mouseTrackingObject.transform.localRotation = Quaternion.Euler(0, 0, 135); // Rotate the object to the bottom left
                    }
                    else
                    {
                        mouseTrackingObject.transform.localRotation = Quaternion.Euler(0, 0, 180); // Rotate the object to the bottom
                    }
                }
                else
                {
                    if (Input.mousePosition.x > mouseDownPos.x && swipeDistanceX > trackSensitivity)
                    {
                        mouseTrackingObject.transform.localRotation = Quaternion.Euler(0, 0, -90); // Rotate the object to the right
                    }
                    else if (Input.mousePosition.x < mouseDownPos.x && swipeDistanceX < -trackSensitivity)
                    {
                        mouseTrackingObject.transform.localRotation = Quaternion.Euler(0, 0, 90); // Rotate the object to the left
                    }
                }


            }
        }

        private void RayPoint()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                rayPointObject.transform.position = hit.point;
            }
        }
    }
}
