using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StickToMouse : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            transform.position = hit.point;
        }
    }
}
