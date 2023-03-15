using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NinjaSpawner : MonoBehaviour
{
    public Collider spawnArea;
    public GameObject[] ninjaPrefabArr;
    public float spawnDelay = 1f;
    public float spawnDelayVariance = 0.5f;
    public float maxAngle = 20f;
    public float force = 10f;
    public float forceVariance = 5f;
    public float lifespan = 5f;
    public GameObject particlePrefab;

    // Update is called once per frame
    void OnEnable()
    {
        StartCoroutine(SpawnNinja());
    }

    void OnDisable()
    {
        StopAllCoroutines();
    }

    private IEnumerator SpawnNinja()
    {
        while (true)
        {
            GameObject ninja = new GameObject();
            ninja.transform.SetParent(transform);
            ninja.tag = "Sliceable";

            GameObject ps = Instantiate(particlePrefab, ninja.transform);
            ps.transform.parent = ninja.transform;
            ps.transform.localPosition = Vector3.zero;

            GameObject ninjaPrefab = ninjaPrefabArr[Random.Range(0, ninjaPrefabArr.Length)];
            ninja.AddComponent<MeshFilter>().mesh = ninjaPrefab.GetComponent<MeshFilter>().sharedMesh;
            ninja.AddComponent<MeshRenderer>().material = ninjaPrefab.GetComponent<MeshRenderer>().sharedMaterial;
            ninja.AddComponent<MeshCollider>().convex = true;

            ninja.transform.localScale = ninjaPrefab.transform.localScale;

            // Set the spawn area
            Vector3 spawnPos = new Vector3(Random.Range(spawnArea.bounds.min.x, spawnArea.bounds.max.x), Random.Range(spawnArea.bounds.min.y, spawnArea.bounds.max.y), Random.Range(spawnArea.bounds.min.z, spawnArea.bounds.max.z));
            ninja.transform.position = spawnPos;

            // Set the lifespan
            ninja.AddComponent<NinjaMesh>();

            // Set the angle
            float angle = Random.Range(-maxAngle, maxAngle);
            ninja.transform.rotation = Quaternion.Euler(0, 0, angle);

            // Set the force
            float spawnForce = force + Random.Range(-forceVariance, forceVariance);
            Rigidbody rb = ninja.AddComponent<Rigidbody>();
            rb.AddForce(ninja.transform.up * spawnForce, ForceMode.Impulse);
            rb.AddTorque(new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)) * spawnForce, ForceMode.Impulse);

            yield return new WaitForSeconds(spawnDelay + Random.Range(-spawnDelayVariance, spawnDelayVariance));
        }
    }
}
