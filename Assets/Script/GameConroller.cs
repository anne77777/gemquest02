using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;

public class GameController	: Photon.MonoBehaviour {
	public GameObject Dice;

	[Header("Tile Resources")]
	public Sprite Origin;
	public Sprite PowerUp;

	[Header("Tile Transform")]
	public Transform[] spots;

	void OnDisable(){
		Debug.Log ("Disable GameManager;");
	}

	void Awake(){

		PlayerInLobby[] players = GameObject.FindObjectsOfType<PlayerInLobby> ();
		Debug.Log ("PhotonNetwokr.playerList.Count : " + PhotonNetwork.playerList.Length);

		for (int i = 0; i < PhotonNetwork.playerList.Length; i++) {
			Debug.Log ("Photon player list : " + i);
			if(PhotonNetwork.playerList[i].isLocal){
				if(PhotonNetwork.playerList[i].IsLocal){
					GenerateToken (i);					
				}
			}

			if(PhotonNetwork.playerList[i].IsMasterClient){
				Debug.Log("PhotonNetwork.playerList[i].IsMasterClient : " + i);
			}
		}
		/*
		for (int i = 0; i < players.Length; i++) {
			if(players[i].GetComponent<PhotonView>().isMine){
				players [i].GenerateToken ();
			}

			PhotonNetwork.Destroy (players [i].gameObject);
		}*/

		Player[] p = GameObject.FindObjectsOfType<Player> ();
		Debug.Log ("plyaer length : " + p.Length);
		for (int i = 0; i < p.Length; i++) {
			TurnManager.instance.AllPlayers [i] = p [i];
		}
		//TurnManager.instance.Initialize ();
		//TurnManager.instance.enabled = true;
	}

	void Start(){
		//PhotonNetwork.Instantiate ("Cube", Vector3.zero, Quaternion.identity, 0);
	}

	public void GenerateToken(int index){
		Debug.Log ("GenerateToken!!!");
		Debug.Log ("index : " + index);
		PhotonNetwork.Instantiate ("Player/PlayerToken", GameManager.instance.spots[index].position, Quaternion.identity, 0);
	}
	/*
	[PunRPC]
	void PhotonGenerateToken(){
		PhotonNetwork.Instantiate ("Player/PlayerToken", GameManager.instance.spots[playerID].position, Quaternion.identity, 0);
	}*/
}
