using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Connoreaster
{
    public class MeshSizeLimit : MonoBehaviour
    {
        private float minShapeSize = 0.05f; // Set the minimum size of the shape
        public bool isPlane = false;

        // Update is called once per frame
        void Update()
        {
            Mesh gameObjectMesh = new Mesh();

            if (gameObject.GetComponent<MeshFilter>() == null)
            {
                Destroy(gameObject);
            }
            else
            {
                gameObjectMesh = GetComponent<MeshFilter>().mesh; // Get the mesh of the game object
            }

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
}
