using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class UnetPlayerPrefab : NetworkBehaviour
{
    public static UnetPlayerPrefab LocalPlayer { get; private set; }

    [SyncVar]
    private int playerID = -1;
    public int PlayerID { get { return playerID; } set { Debug.Assert(isServer); playerID = value; } }

    protected void Start()
    {
        if (hasAuthority)
        {
            LocalPlayer = this;
        }
    }

    [Command]
    public void CmdPostMove(int round, string move)
    {
        Debug.Assert(isServer, "Must Run on Server");

        UnetGameplayServer.Instance.PostPlayerMove(playerID, round, move);
    }
}
