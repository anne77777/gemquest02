using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;

public class TurnManager : Photon.MonoBehaviour {

	public static TurnManager instance;

	[Header("Players")]
	public List<Player> AllPlayers;
	public Player CurrentPlayer;

	void Awake(){
		instance = this;
	}

	void Start(){
		Initialize ();
	}

	public void Initialize(){
		photonView.RPC ("InitializePun", PhotonTargets.All);
	}

	[PunRPC]
	void InitializePun(){
		for (int i = 0; i < AllPlayers.Count; i++) {
			if(AllPlayers[i] == null){
				continue;
			}
			AllPlayers [i].InitializePlayerPun ();
		}

		CurrentPlayer = AllPlayers [0];
		GameManager.instance.OrganizeAllPlayerInfoPanel ();
		EnabledPlayer ();
	}

	public void NextPlayerTurn(){
		int index = AllPlayers.IndexOf (CurrentPlayer);

		if (index + 1 >= AllPlayers.Count) {
			index = 0;
		} else {
			index++;
		}

		CurrentPlayer = AllPlayers [index];
		EnabledPlayer ();
	}

	void EnabledPlayer(){
		for (int i = 0; i < AllPlayers.Count; i++) {
			if (AllPlayers [i] == null) {
				continue;
			}

			if (AllPlayers [i] == CurrentPlayer) {				
				AllPlayers [i].DiceRollingPanel.SetActive (true);
				AllPlayers [i].enabled = true;
			} else {
				AllPlayers [i].DiceRollingPanel.SetActive (false);
				AllPlayers [i].enabled = false;
			}
		}

		CurrentPlayer.enabled = true;
	}
}
