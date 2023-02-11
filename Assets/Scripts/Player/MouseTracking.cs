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
                        player.Animation("leftAttackRight");
                    }
                    else if (Input.mousePosition.x < mouseDownPos.x && swipeDistanceX < -trackSensitivity)
                    {
                        mouseTrackingObject.transform.localRotation = Quaternion.Euler(0, 0, 90); // Rotate the object to the left
                        player.Animation("rightAttackLeft");
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
