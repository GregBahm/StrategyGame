using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Linq;

public class DebugHUD : MonoBehaviour
{
    private Vector2 dataWindowScrollPos = Vector2.zero;

    private const int dataScrollHeight = 300;
    private const int width = 150;

    protected void OnGUI()
    {
        ServerGUI();

        DataGUI();

        ClientGUI();
    }

    private static void ServerGUI()
    {
        if (UnetGameplayServer.Instance.isServer)
        {
            GUILayout.BeginVertical(GUI.skin.box, GUILayout.Width(width));
            GUILayout.Label("SERVER:");

            GUI.enabled = !UnetGameplayServer.Instance.GameStarted;
            if (GUILayout.Button("Start Game"))
            {
                UnetGameplayServer.Instance.StartGame();
            }

            GUI.enabled = true;
            GUILayout.Label(string.Format("Players: {0}", UnetGameplayServer.Instance.GameStarted ? UnetGameplayServer.Instance.PlayerCount : NetworkServer.connections.Count(x => x != null)));
            GUILayout.Label(string.Format("Current Round: {0}", UnetGameplayServer.Instance.CurrentRound));
            GUILayout.Label(string.Format("Posted Player Moves: {0}", UnetGameplayServer.Instance.PostedPlayerMoves));
            GUILayout.HorizontalSlider(UnetGameplayServer.Instance.PostedPlayerMoves, 0, UnetGameplayServer.Instance.PlayerCount);

            GUI.enabled = UnetGameplayServer.Instance.GameStarted;
            if (GUILayout.Button("Publish Round"))
            {
                UnetGameplayServer.Instance.PublishRound();
            }

            GUILayout.EndVertical();
        }
    }

    private void DataGUI()
    {
        if (UnetGameplayServer.Instance.GameStarted)
        {
            GUILayout.BeginVertical(GUI.skin.box, GUILayout.Width(width));
            GUILayout.Label("DATA:");

            dataWindowScrollPos = GUILayout.BeginScrollView(dataWindowScrollPos, GUILayout.MaxHeight(300), GUILayout.MinHeight(0));

            for (int i = 0; i < UnetGameplayServer.Instance.Moves.Count; i++)
            {
                GUILayout.Label(string.Format("Move {0}", i));

                if (UnetGameplayServer.Instance.Moves[i] == null)
                {
                    GUILayout.Label("\t MISSING");
                    continue;
                }

                for (int j = 0; j < UnetGameplayServer.Instance.Moves[i].Length; j++)
                {
                    GUILayout.Label(string.Format("  {0} - {1}", j, UnetGameplayServer.Instance.Moves[i][j]));
                }
            }

            if (UnetGameplayServer.Instance.isServer)
            {
                GUILayout.Label("Pre Published Data");
                for (int j = 0; j < UnetGameplayServer.Instance.PrePublishedData.Length; j++)
                {
                    GUILayout.Label(string.Format("  {0} - {1}", j, UnetGameplayServer.Instance.PrePublishedData[j]));
                }
            }

            GUILayout.EndScrollView();

            GUILayout.EndVertical();
        }
    }

    private string moveString;
    private void ClientGUI()
    {
        if (UnetGameplayServer.Instance.isClient && UnetPlayerPrefab.LocalPlayer != null)
        {
            GUILayout.BeginVertical(GUI.skin.box, GUILayout.Width(width));
            GUILayout.Label("CLIENT:");

            GUILayout.Label(string.Format("Player ID: {0}", UnetPlayerPrefab.LocalPlayer.PlayerID));

            GUI.enabled = UnetGameplayServer.Instance.GameStarted;

            GUILayout.Label("Move:");
            moveString = GUILayout.TextField(moveString);

            if (GUILayout.Button("Post Move"))
            {
                UnetPlayerPrefab.LocalPlayer.CmdPostMove(UnetGameplayServer.Instance.CurrentRound, moveString);
                moveString = "";
            }

            GUILayout.EndVertical();
        }
    }
}
