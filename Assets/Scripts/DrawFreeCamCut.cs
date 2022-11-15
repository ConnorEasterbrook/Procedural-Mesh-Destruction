using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawFreeCamCut : MonoBehaviour
{
    private Camera mainCam;
    private Vector3 _POINTA;
    private Vector3 _POINTB;
    private LineRenderer _LINE;

    private void Awake() 
    {
        mainCam = Camera.main; // Get the main camera

        _LINE = GetComponent<LineRenderer>(); // Get the line renderer
        _LINE.startWidth = 0.1f; // Set the line width
        _LINE.endWidth = 0.1f; // Set the line width
    }

    // Update is called once per frame
    void Update()
    {
        MouseInput();
    }

    /// <summary>
    /// Track the mouse position and input
    /// </summary>
    private void MouseInput()
    {
        Vector3 mousePos = Input.mousePosition; // Get the mouse position
        mousePos.z = -mainCam.transform.position.z; // Set the z position to the camera's z position

        if (Input.GetMouseButtonDown(0)) // If the left mouse button is pressed
        {
            _POINTA = mainCam.ScreenToWorldPoint(mousePos); // Set the first point to the mouse position
        }

        if (Input.GetMouseButton(0)) // If the left mouse button is held down
        {
            _POINTB = mainCam.ScreenToWorldPoint(mousePos); // Set the second point to the mouse position

            _LINE.SetPosition(0, _POINTA); // Set the first point of the line
            _LINE.SetPosition(1, _POINTB); // Set the second point of the line

            _LINE.startColor = Color.red; // Set the line color
            _LINE.endColor = Color.red; // Set the line color
        }

        if (Input.GetMouseButtonUp(0)) // If the left mouse button is released
        {
            _POINTB = mainCam.ScreenToWorldPoint(mousePos); // Set the second point to the mouse position

            _LINE.SetPosition(0, _POINTA); // Set the first point of the line
            _LINE.SetPosition(1, _POINTB); // Set the second point of the line

            Slice(); // Call the slice method
        }
    }

    /// <summary>
    /// Slice the object using a created plane
    /// </summary>
    private void Slice() 
    {
        Vector3 planePoint = (_POINTA + _POINTB) / 2; // Get the center point of the line
        Vector3 planeNormal = Vector3.Cross((_POINTA - _POINTB), _POINTA - mainCam.transform.position).normalized; // Get the normal of the plane
        Quaternion planeRotation = Quaternion.FromToRotation(Vector3.up, planeNormal); // Get the rotation of the plane

        // Collider[] colliders = Physics.OverlapSphere(planePoint, new Vector3(100f, 0.01f, 100f), planeRotation); // Get all colliders within the plane

        // foreach (Collider hit in colliders)
        // {
        //     MeshFilter meshFilter = hit.GetComponentInChildren<MeshFilter>(); // Get the mesh filter

        //     if (meshFilter != null)
        //     {
        //         // CALL CUTTING SCRIPT HERE
        //     }
        // }
    }
}
