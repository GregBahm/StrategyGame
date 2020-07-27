using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.WSA;

public class RemoteConnector : MonoBehaviour
{
    private string IP = "10.0.0.164";

    private bool connected;

    public void Connect()
    {
        if (HolographicRemoting.ConnectionState != HolographicStreamerConnectionState.Connected)
        {
            HolographicRemoting.Connect(IP, 99999, RemoteDeviceVersion.V2);
        }
    }

    private void Start()
    {
        Connect();
    }

    void Update()
    {
        Shader.SetGlobalMatrix("_DisplayVolume", transform.worldToLocalMatrix);

        if (!connected && HolographicRemoting.ConnectionState == HolographicStreamerConnectionState.Connected)
        {
            connected = true;

            StartCoroutine(LoadDevice("WindowsMR"));
        }
    }

    IEnumerator LoadDevice(string newDevice)
    {
        XRSettings.LoadDeviceByName(newDevice);
        yield return null;
        XRSettings.enabled = true;
    }
}
