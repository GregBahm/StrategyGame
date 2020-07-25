using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalShaderManager : MonoBehaviour
{
    void Update()
    {
        Shader.SetGlobalMatrix("_DisplayVolume", transform.worldToLocalMatrix);
    }
}
