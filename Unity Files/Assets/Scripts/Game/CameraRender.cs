using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace Connoreaster
{
    public class CameraRender : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            RenderSettings.fog = true;
            RenderSettings.fogDensity = 0.175f;
        }
    }
}
