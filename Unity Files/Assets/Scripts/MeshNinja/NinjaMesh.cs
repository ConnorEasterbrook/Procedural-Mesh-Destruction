using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class NinjaMesh : MonoBehaviour
{
    private bool _enabled = true;
    // private Collider _collider;

    // private void Update()
    // {
    //     _collider.enabled = false;
    // }

    public async void OnEnable()
    {
        // _collider = GetComponent<Collider>();

        await Task.Delay(5000);
        if (_enabled)
        {
            Destroy(gameObject);
        }
    }
    public void OnDisable()
    {
        _enabled = false;
    }
}
