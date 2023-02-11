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
            CalculateHit();
        }
    }

    public void CalculateHit()
    {
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

        transform.localScale = Vector3.zero;
        Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Weapon")
        {
            GetHit();
        }
    }
}
