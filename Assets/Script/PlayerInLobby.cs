using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;

public class PlayerInLobby : Photon.MonoBehaviour {
	
	public int playerID;
	public bool canStart;

	void Start(){
		DontDestroyOnLoad (this.gameObject);
	}

	public void CheckCanStart(){
		photonView.RPC ("Check", PhotonTargets.All);
		//Check();
	}

	[PunRPC]
	void Check(){
		canStart = true;
	}

	public void GenerateToken(){
		PhotonNetwork.Instantiate ("Player/PlayerToken", GameManager.instance.spots[playerID].position, Quaternion.identity, 0);
	}

	[PunRPC]
	void PhotonGenerateToken(){
		PhotonNetwork.Instantiate ("Player/PlayerToken", GameManager.instance.spots[playerID].position, Quaternion.identity, 0);
	}
}
