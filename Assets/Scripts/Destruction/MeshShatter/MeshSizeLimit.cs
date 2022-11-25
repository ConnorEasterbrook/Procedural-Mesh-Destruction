using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshSizeLimit : MonoBehaviour
{
    public float minShapeSize = 0.0175f;
    public bool isPlane = false;

    // Update is called once per frame
    void Update()
    {
        Mesh gameObjectMesh = GetComponent<MeshFilter>().mesh;

        if (!isPlane)
        {
            if (gameObjectMesh.bounds.size.x * gameObjectMesh.bounds.size.y * gameObjectMesh.bounds.size.z < minShapeSize)
            {
                Destroy(gameObject);
            }
        }
        else
        {
            if (gameObjectMesh.bounds.size.x * gameObjectMesh.bounds.size.z < minShapeSize * 100)
            {
                Destroy(gameObject);
            }
        }
    }
}
