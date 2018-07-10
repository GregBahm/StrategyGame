using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

public class UnetGameplayServer : NetworkBehaviour
{
#if UNITY_EDITOR
    [UnityEditor.CustomEditor(typeof(UnetGameplayServer))]
    public class Editor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            UnetGameplayServer t = (UnetGameplayServer)target;

            GUI.enabled = Application.isPlaying && !t.gameStarted;
            if (GUILayout.Button("Start Game"))
            {
                t.StartGame();
            }

            GUI.enabled = Application.isPlaying && t.gameStarted;
            if (GUILayout.Button("Publish Round"))
            {
                t.PublishRound();
            }

            GUI.enabled = false;
            UnityEditor.EditorGUILayout.IntField("Current Rount", t.currentRound);
            UnityEditor.EditorGUILayout.IntField("Player Count", t.playerCount);

            UnityEditor.EditorGUILayout.IntSlider("Posted Player Moves", t.PostedPlayerMoves, 0, t.playerCount);

        }
    }
#endif

    public static UnetGameplayServer Instance;

    private List<string[]> moves;
    public List<string[]> Moves { get { return moves; } }

    private string[] prePublishedData = null;
    public string[] PrePublishedData { get { return prePublishedData; } }

    [SyncVar]
    private int currentRound;
    public int CurrentRound { get { return currentRound; } }

    [SyncVar]
    private int playerCount;
    public int PlayerCount { get { return playerCount; } }

    [SyncVar]
    private bool gameStarted;
    public bool GameStarted { get { return gameStarted; } }

    public int PostedPlayerMoves { get { return gameStarted ? PrePublishedData.Count(x => !string.IsNullOrEmpty(x)) : 0; } }

    protected void Awake()
    {
        Instance = this;
    }

    protected void OnEnable()
    {
        moves = new List<string[]>();
        currentRound = 0;
        playerCount = 0;
        gameStarted = false;
    }

    public void StartGame()
    {
        Debug.Assert(isServer, "Must Run on Server");

        var players = NetworkServer.connections.Where(x => x != null).ToArray();
        playerCount = players.Length;

        for (int i = 0; i < playerCount; i++)
        {
            players[i].playerControllers[0].gameObject.GetComponent<UnetPlayerPrefab>().PlayerID = i;
        }

        prePublishedData = new string[playerCount];
        gameStarted = true;
    }

    public void PostPlayerMove(int player, int round, string move)
    {
        Debug.Assert(isServer, "Must Run on Server");

        if (round != currentRound
            || player >= playerCount
            || !gameStarted)
        {
            Debug.LogWarning("Incorrect data.  Ignoring");
            return;
        }

        prePublishedData[player] = move;
        Debug.LogFormat("Got move data for round {0}, player {1}", round, player);
    }

    public void PublishRound()
    {
        RpcPublishRoundData(currentRound, prePublishedData);

        currentRound++;

        prePublishedData = new string[playerCount];
    }

    [ClientRpc]
    protected void RpcPublishRoundData(int round, string[] moveData)
    {
        Debug.Assert(isClient, "Must Run on Client");
        Debug.LogFormat("Got round data for round {0}", round);

        // Pad empty moves for missing data
        for (int i = moves.Count; i <= round; i++)
        {
            moves.Add(null);
        }

        // Set data
        moves[round] = moveData;
    }

}
