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
    public class DismemberHelper
    {
        public void RemoveComponents(GameObject limb)
        {
            if (limb.tag == "Limb")
            {
                GameObject newLimb = GameObject.Instantiate(limb, limb.transform.position, limb.transform.rotation);
                newLimb.GetComponent<Rigidbody>().isKinematic = false;
                newLimb.GetComponent<Rigidbody>().useGravity = true;
                MonoBehaviour.Destroy(newLimb.GetComponent<DismemberLimb>());
                MonoBehaviour.Destroy(newLimb.GetComponent<CharacterJoint>());

                MonoBehaviour.Destroy(limb.gameObject);

                var list = newLimb.GetComponentsInChildren<Transform>();
                foreach (Transform child in list)
                {
                    child.GetComponent<Rigidbody>().isKinematic = false;
                    child.GetComponent<Rigidbody>().useGravity = true;
                    // MonoBehaviour.Destroy(child.GetComponent<DismemberLimb>());
                    // MonoBehaviour.Destroy(child.GetComponent<CharacterJoint>());
                }
            }

            limb.tag = "Sliceable";
            MonoBehaviour.Destroy(limb.GetComponent<DismemberLimb>());
            MonoBehaviour.Destroy(limb.GetComponent<CharacterJoint>());
            limb.GetComponent<Rigidbody>().isKinematic = false;
            limb.GetComponent<Rigidbody>().useGravity = true;
        }
    }
}
