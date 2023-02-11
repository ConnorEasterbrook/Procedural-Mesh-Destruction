using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Connoreaster
{
    public class DismemberHelper
    {
        public void RemoveComponents(GameObject limb)
        {
            if (limb.tag == "Limb")
            {
                limb.tag = "Sliceable";

                Debug.Log("Removing components");
                GameObject newLimb = GameObject.Instantiate(limb, limb.transform.position, limb.transform.rotation);
                newLimb.GetComponent<Rigidbody>().isKinematic = false;
                newLimb.GetComponent<Rigidbody>().useGravity = true;
                MonoBehaviour.Destroy(newLimb.GetComponent<CharacterJoint>());

                MonoBehaviour.Destroy(limb.gameObject);

                var list = newLimb.GetComponentsInChildren<Transform>();
                foreach (Transform child in list)
                {
                    child.GetComponent<Rigidbody>().isKinematic = false;
                    child.GetComponent<Rigidbody>().useGravity = true;
                }
            }

            MonoBehaviour.Destroy(limb.GetComponent<CharacterJoint>());
            limb.GetComponent<Rigidbody>().isKinematic = false;
            limb.GetComponent<Rigidbody>().useGravity = true;
        }
    }
}
