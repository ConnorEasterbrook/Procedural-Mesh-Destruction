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

public class DismemberEnemy : MonoBehaviour
{
    private Animator _animator;
    private List<Rigidbody> _rigidbodies = new List<Rigidbody>();
    public Rigidbody core_rb;
    public GameObject head;

    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponent<Animator>();
        _rigidbodies = new List<Rigidbody>(GetComponentsInChildren<Rigidbody>());
        _rigidbodies.Remove(GetComponent<Rigidbody>());

        DeactivateRagdoll();
    }

    // Update is called once per frame
    void Update()
    {
        if (head != null)
        {
            if (head.transform.localScale == Vector3.zero)
            {
                _animator.enabled = false;

                ActivateRagdoll();
            }
        }
    }

    private void ActivateRagdoll()
    {
        foreach (Rigidbody rb in _rigidbodies)
        {
            if (rb != null)
            {
                rb.isKinematic = false;
                rb.useGravity = true;
            }
            else
            {
                _rigidbodies.Remove(rb);
            }
        }
    }

    private void DeactivateRagdoll()
    {
        foreach (Rigidbody rb in _rigidbodies)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }
    }
}
