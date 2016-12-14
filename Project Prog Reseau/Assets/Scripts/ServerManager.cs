using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System.Collections.Generic;
using UnityEngine.UI;

public class ServerManager : NetworkBehaviour
{

    public SyncListInt PlayerScores = new SyncListInt();

    [SyncVar] public int numberOfPlayers;


    [SyncVar] public bool serverIsPlayer = false;

    Text[] playerScoresDisplay = new Text[2];

    Text waitToPlay;

	// Use this for initialization
	void Awake ()
    {
        playerScoresDisplay[0] = GameObject.Find("ScorePlayer1").GetComponent<Text>();
        playerScoresDisplay[1] = GameObject.Find("ScorePlayer2").GetComponent<Text>();
        waitToPlay = GameObject.Find("waitToPlay").GetComponent<Text>();
    }

    // Update is called once per frame
    void Update ()
    {
        waitToPlay.enabled = numberOfPlayers <= 1;

        if (isServer && Input.GetKeyDown(KeyCode.P))
        {
            Cmd_Cheat();
        }


    }

    void Cmd_Cheat()
    {
        numberOfPlayers++;
    }

    [Command]
    public void Cmd_AddScore(int playerID, int value)
    {

        while (PlayerScores.Count <= playerID)
        {
            PlayerScores.Add(0);
            Rpc_AddToList();
        }

        PlayerScores[playerID] += value;

        Rpc_UpdateDisplays();
    }

    public void AddScore(int playerID, int value)
    {
        if (!isServer) return;
        while (PlayerScores.Count <= playerID)
        {
            PlayerScores.Add(0);
            Rpc_AddToList();
        }

        PlayerScores[playerID] += value;

        Rpc_UpdateDisplays();
    }

    [ClientRpc]
    void Rpc_AddToList()
    {
        PlayerScores.Add(0);
    }

    [ClientRpc]
    void Rpc_UpdateDisplays()
    {
        playerScoresDisplay[0].text = PlayerScores[0].ToString();
        playerScoresDisplay[1].text = PlayerScores[1].ToString();

    }

}
