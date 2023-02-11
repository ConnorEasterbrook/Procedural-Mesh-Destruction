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
