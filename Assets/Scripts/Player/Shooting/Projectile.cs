using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    IEnumerator Start()
    {
        yield return new WaitForSeconds(5.0f);
        Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Shatter")
        {
        foreach (var shard in Physics.OverlapSphere(transform.position, 5.0f))
        {
            var rb = shard.GetComponent<Rigidbody>();

            rb.isKinematic = false;
            rb.AddExplosionForce(5.0f, transform.position, 5.0f);

            // shard.gameObject.AddComponent<DestroyIn>().Time = 3.0f;
        }
        }
    }
}
