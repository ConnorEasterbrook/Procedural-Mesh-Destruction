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
                slicePlane = new Plane(Random.onUnitSphere, _CPPos);
            }
            else
            {
                slicePlane = new Plane(Random.onUnitSphere, new Vector3
                (
                    gameObjectMesh.bounds.min.x + gameObjectMesh.bounds.size.x / 2,
                    Random.Range(gameObjectMesh.bounds.min.y, gameObjectMesh.bounds.max.y),
                    gameObjectMesh.bounds.min.z + gameObjectMesh.bounds.size.z / 2
                ));
            }

            MeshCutCalculations calc = new MeshCutCalculations(); // Create a new mesh cut calculations object
            calc.CallScript(hitObject, slicePlane); // Call the mesh cut calculations script

            Mesh mesh2 = calc.mesh2.GetGeneratedMesh();
            if (iteration == 0)
            {
                slicePlane = new Plane(Random.onUnitSphere, _CPPos);
            }
            else
            {
                slicePlane = new Plane(Random.onUnitSphere, new Vector3
                (
                    mesh2.bounds.min.x + mesh2.bounds.size.x / 2,
                    Random.Range(mesh2.bounds.min.y, mesh2.bounds.max.y),
                    mesh2.bounds.min.z + mesh2.bounds.size.z / 2
                ));
            }

            calc.CallScript(calc.secondMeshGO, slicePlane); // Call the mesh cut calculations script (again
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