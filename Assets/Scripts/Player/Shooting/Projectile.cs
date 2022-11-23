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
}
