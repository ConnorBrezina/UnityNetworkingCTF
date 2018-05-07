using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class CTFGameManager : NetworkBehaviour {

    public int m_numPlayers = 2;
    public float m_gameTime = 5.0f;
	public Text m_timerText = null;
	public Text m_ScoreText = null;
    public GameObject[] m_flag = null;
	private float time= 35.0f;

    public enum CTF_GameState
    {
        GS_WaitingForPlayers,
        GS_Ready,
        GS_InGame,
    }

    [SyncVar]
    CTF_GameState m_gameState = CTF_GameState.GS_WaitingForPlayers;

    public bool SpawnFlag()
    {
		Vector3 spawnPoint;
		ObjectSpawner.RandomPoint(new Vector3(0, 0, 0), 20.0f, out spawnPoint);
		GameObject flag = Instantiate(m_flag[0], spawnPoint, new Quaternion());
        NetworkServer.Spawn(flag);
        return true;
    }
	public bool P2()
	{
		Vector3 spawnPoint;
		ObjectSpawner.RandomPoint(new Vector3(0, 0, 0), 20.0f, out spawnPoint);
		GameObject p2 = Instantiate(m_flag[1], spawnPoint, new Quaternion());
		NetworkServer.Spawn(p2);
		return true;
	}
	public bool P1()
	{
		Vector3 spawnPoint;
		ObjectSpawner.RandomPoint(new Vector3(0, 0, 0), 20.0f, out spawnPoint);
		GameObject p1 = Instantiate(m_flag[2], spawnPoint, new Quaternion());
		NetworkServer.Spawn(p1);
		return true;
	}

    bool IsNumPlayersReached()
    {
        return CTFNetworkManager.singleton.numPlayers == m_numPlayers;
    }

	// Use this for initialization
	void Start () {
    }

	// Update is called once per frame
	void Update ()
    {
		if (m_gameState == CTF_GameState.GS_InGame) {
			time -= Time.deltaTime;
			m_timerText.text = "Time Left: " + time;
			//m_ScoreText.text = "Your Score: " + PlayerController.score;
		}
		if (time <= 0) {
			m_timerText.text = "GameOver!";
			if (PlayerController.ScoreP1 > PlayerController.ScoreP2) {
				m_ScoreText.text = "Winners Score: " + PlayerController.ScoreP1;
			}
			if (PlayerController.ScoreP1 < PlayerController.ScoreP2) {
				m_ScoreText.text = "Winners Score: " + PlayerController.ScoreP2;
			}
			else
				m_ScoreText.text = "You Lose";
		}

	    if(isServer)
        {
            if(m_gameState == CTF_GameState.GS_WaitingForPlayers && IsNumPlayersReached())
            {
                m_gameState = CTF_GameState.GS_Ready;
            }
        }

        UpdateGameState();
	}

    public void UpdateGameState()
    {
        if (m_gameState == CTF_GameState.GS_Ready)
        {
            //call whatever needs to be called
            if (isServer)
            {
                SpawnFlag();
				P1 ();
				P2 ();
                //change state to ingame
                m_gameState = CTF_GameState.GS_InGame;
            }
        }
        
    }
}
