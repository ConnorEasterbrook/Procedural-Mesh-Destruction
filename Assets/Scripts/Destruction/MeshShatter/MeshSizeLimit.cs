using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshSizeLimit : MonoBehaviour
{
    [Range (0.5f, 2.5f)] public float minShapeSize = 1.75f; // Set the minimum size of the shape
    public bool isPlane = false;

    // Update is called once per frame
    void Update()
    {
        Mesh gameObjectMesh = GetComponent<MeshFilter>().mesh; // Get the mesh of the game object

        // Check if the object is a plane or not to shatter properly
        if (!isPlane)
        {
            if (gameObjectMesh.bounds.size.x * gameObjectMesh.bounds.size.y * gameObjectMesh.bounds.size.z < minShapeSize / 100)
            {
                Destroy(gameObject);
            }
        }
        else
        {
            if (gameObjectMesh.bounds.size.x * gameObjectMesh.bounds.size.z < minShapeSize)
            {
                Destroy(gameObject);
            }
        }
    }
}
