﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using BattleScenario;
using UnityEngine.UI;

public class BattleStateHandler : MonoBehaviour {

	private IBattleState currentBattleState;

	public Image fireballPrefab;
	public Image coldballPrefab;

    public Image timerDisplay;

	[System.NonSerialized]
	public Canvas canvas;

	public FireMonster enemy;
    public float ServerCountdownTime;

    public int highestVotedAction;

    [RPC]
    public void FinaliseHighestVotedAction(int highestVotedAction)
    {
        this.highestVotedAction = highestVotedAction;

        System.Array.Clear(Voting.ChosenOption, 0, Voting.ChosenOption.Length);
    }

    [RPC]
    void SetCountdownTime(float time)
    {
        ServerCountdownTime = time;
    }

    void OnPlayerConnected(NetworkPlayer player)
    {
        networkView.RPC("SetCountdownTime", player, ServerCountdownTime);
    }

    public void SyncCountdownTime()
    {
        for (int playerIndex = 0; playerIndex < Network.connections.Length; ++playerIndex) {
            var player = Network.connections[playerIndex];
            int pingMilisec = Network.GetAveragePing(player);
            float pingSec = pingMilisec * (1.0f / 1000f);
            networkView.RPC("SetCountdownTime", player, ServerCountdownTime + pingSec);
        }
    }

    public enum PlayerAction : int
    {
		DEFAULT,
		FIREBALL,
		COLDBALL,
		DEFEND
	}

	void Awake () {
		this.canvas = GetComponentInParent<Canvas> ();
        this.currentBattleState = new CountdownState(this);
	}

	void Update () {
        if (Network.isClient || Network.isServer) {
            this.currentBattleState = this.currentBattleState.UpdateState(this);
            this.currentBattleState.Update(this);
        }
	}
}
