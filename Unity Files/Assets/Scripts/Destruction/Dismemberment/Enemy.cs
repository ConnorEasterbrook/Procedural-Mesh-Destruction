using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public GameObject particlePrefab;
    public float health = 100f;

    private void Awake()
    {
        gameObject.tag = "Untagged";
    }

    private void Update()
    {
        if (health <= 0)
        {
            gameObject.tag = "Limb";
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Weapon")
        {
            Splatter();
            UpdateLimbHealth();
        }
    }

    public void UpdateLimbHealth()
    {
        health -= 50f;
    }

    private void Splatter()
    {
        Debug.Log("Splatter");
        GameObject ps = Instantiate(particlePrefab, transform);
    }
}
