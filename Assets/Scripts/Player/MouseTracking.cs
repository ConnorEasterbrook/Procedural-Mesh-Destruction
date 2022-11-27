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
        private Vector3 mouseDownPos; // Set the mouse down position
        public FirstPersonController player; // Set the player object

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
                        mouseTrackingObject.transform.localRotation = Quaternion.Euler(0, 0, -45); 
                        player.Animation("bottomLeftAttackUp");

                    }
                    else if (Input.mousePosition.x < mouseDownPos.x && swipeDistanceX < -trackSensitivity)
                    {
                        mouseTrackingObject.transform.localRotation = Quaternion.Euler(0, 0, 45); 
                        player.Animation("bottomRightAttackUp");
                    }
                    else
                    {
                        mouseTrackingObject.transform.localRotation = Quaternion.Euler(0, 0, 0);
                        player.Animation("bottomAttackUp");
                    }
                }
                else if (Input.mousePosition.y < mouseDownPos.y && swipeDistanceY < -trackSensitivity)
                {
                    if (Input.mousePosition.x > mouseDownPos.x && swipeDistanceX > trackSensitivity)
                    {
                        mouseTrackingObject.transform.localRotation = Quaternion.Euler(0, 0, -135); 
                        player.Animation("topLeftAttackDown");
                    }
                    else if (Input.mousePosition.x < mouseDownPos.x && swipeDistanceX < -trackSensitivity)
                    {
                        mouseTrackingObject.transform.localRotation = Quaternion.Euler(0, 0, 135); 
                        player.Animation("topRightAttackDown");
                    }
                    else
                    {
                        mouseTrackingObject.transform.localRotation = Quaternion.Euler(0, 0, 180); 
                        player.Animation("topAttackDown");
                    }
                }
                else
                {
                    if (Input.mousePosition.x > mouseDownPos.x && swipeDistanceX > trackSensitivity)
                    {
                        mouseTrackingObject.transform.localRotation = Quaternion.Euler(0, 0, -90); // Rotate the object to the right
                        player.Animation("rightAttackLeft");
                    }
                    else if (Input.mousePosition.x < mouseDownPos.x && swipeDistanceX < -trackSensitivity)
                    {
                        mouseTrackingObject.transform.localRotation = Quaternion.Euler(0, 0, 90); // Rotate the object to the left
                        player.Animation("leftAttackRight");
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
