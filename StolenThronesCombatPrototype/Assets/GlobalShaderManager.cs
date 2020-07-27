using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.WSA;

public class GlobalShaderManager : MonoBehaviour
{

    void Update()
    {
        Shader.SetGlobalMatrix("_DisplayVolume", transform.worldToLocalMatrix);
    }
}
