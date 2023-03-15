using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Connoreaster
{
    public class Shoot : MonoBehaviour
    {
        public Rigidbody projectile;
        public Transform shootLocation;
        public float speed = 20;
        public DebugController debugController;

        void Update()
        {
            // shootLocation.rotation = Quaternion.Euler(Camera.main.transform.rotation.x, 0, 0);

            if (Input.GetMouseButtonDown(0))
            {
                Rigidbody instantiatedProjectile = Instantiate(projectile, shootLocation.position, shootLocation.rotation) as Rigidbody;
                instantiatedProjectile.gameObject.AddComponent<MeshShatter>().debugController = debugController;

                instantiatedProjectile.velocity = speed * Camera.main.transform.forward;
            }
        }
    }
}
