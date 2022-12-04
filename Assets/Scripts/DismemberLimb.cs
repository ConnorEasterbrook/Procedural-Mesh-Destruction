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

public class DismemberLimb : MonoBehaviour
{
    public DismemberLimb[] childLimbs;
    private GameObject limbPrefab;
    public GameObject woundPrefab;
    public bool noDismember;
    public GameObject limbBin;
    public GameObject replacementLimb;

    // Start is called before the first frame update
    void Start()
    {
        childLimbs = GetComponentsInChildren<DismemberLimb>();

        limbPrefab = gameObject;

        if (woundPrefab != null)
        {
            woundPrefab.SetActive(false);
        }
    }

    void OnEnable()
    {
        Physics.IgnoreCollision(GetComponent<Collider>(), GetComponentInParent<Collider>(), true);
        Physics.IgnoreCollision(GetComponent<Collider>(), GetComponentInChildren<Collider>(), true);
    }

    public void GetHit()
    {
        if (childLimbs.Length > 0 && !noDismember)
        {
            // foreach (DismemberLimb limb in childLimbs)
            // {
            //     if (limb != null)
            //     {
            //         limb.CalculateHit();
            //     }
            // }

            CalculateHit();
        }
    }

    public void CalculateHit()
    {
        // transform.localScale = Vector3.zero;

        if (woundPrefab != null)
        {
            woundPrefab.SetActive(true);
        }

        GameObject newLimb = Instantiate(limbPrefab, transform.position, transform.rotation, limbBin.transform);
        newLimb.transform.localScale = Vector3.one;
        newLimb.GetComponent<DismemberLimb>().noDismember = true;
        Destroy(newLimb.GetComponentInChildren<CharacterJoint>());

        foreach (Rigidbody rb in newLimb.GetComponentsInChildren<Rigidbody>())
        {
            rb.isKinematic = false;
            rb.useGravity = true;
        }

        // foreach (DismemberLimb limb in newLimb.GetComponentsInChildren<DismemberLimb>())
        // {
        //     limb.noDismember = true;
        // }

        // m.GetChild(0).gameObject, transform.GetChild(0).position, transform.GetChild(0).rotation, limbBin.transform);
        // Destroy(newLimbChild.GetComponent<CharacterJoint>());
        // GameObject newLimb = Instantiate(replacementLimb, transform.position, transform.rotation, limbBin.transform);
        // Destroy(newLimb.GetComponent<CharacterJoint>()

        transform.localScale = Vector3.zero;
        Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Weapon")
        {
            Debug.Log("Hit");
            GetHit();
        }
    }
}
