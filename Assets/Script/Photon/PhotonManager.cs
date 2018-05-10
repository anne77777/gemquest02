using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon;

public class PhotonManager : Photon.MonoBehaviour {
	
	[SerializeField]private Text connectText;
	PlayerSpawnSpot[] spots;
	[SerializeField]bool[] spotOccupied;

	[SerializeField]int playercount;
	//bool connecting = false;

	List<string> message = new List<string>();

	void Start(){
		//PhotonNetwork.offlineMode = true; //single or multiplayer room
		PhotonNetwork.ConnectUsingSettings ("ver1.0");
		PhotonNetwork.automaticallySyncScene = true;
		spots = GameObject.FindObjectsOfType<PlayerSpawnSpot> ();
		spotOccupied = new bool[spots.Length];
		for (int i = 0; i < spotOccupied.Length; i++) {
			spotOccupied [i] = false;
		}
		//PhotonNetwork.player.name = PlayerPrefs.GetString ("UserName", "Turdina");
	}

	void Connect(){/*
		if(PhotonNetwork.offlineMode){
			PhotonNetwork.offlineMode = true;//pretend that we've succesfully connected
			OnJoinedLobby();
		}else{*/
			PhotonNetwork.ConnectUsingSettings ("ver1.0");
		//}
	}

	void Update(){
		connectText.text = PhotonNetwork.connectionStateDetailed.ToString ();

		if(playercount >= 1 && playercount <= 4){
			bool allchecked = true;
			PlayerInLobby[] players = GameObject.FindObjectsOfType<PlayerInLobby> ();
			for (int i = 0; i < players.Length; i++) {
				if(!players[i].canStart){
					allchecked = false;	
					break;
				}
			}

			//Debug.Log ("allchecked : " + allchecked);
			if(allchecked){
				PhotonNetwork.LoadLevel ("gametest");
			}
		}
	}

	public virtual void OnConnectedToMaster(){
		Debug.Log ("OnConnectedToMaster()");
	}

	public virtual void OnJoinedLobby(){
		Debug.Log ("OnJoinedLobby()");
		PhotonNetwork.JoinOrCreateRoom ("Room 1", null, null);		

		//connecting = false;

	}

	public virtual void OnJoinedRoom(){
		Debug.Log ("OnJoinedRoom()");

		if(playercount < 4){
			SpawnPlayer ();	
			photonView.RPC ("AddPlayerCount", PhotonTargets.All);	
		}
	}

	[PunRPC]
	void AddPlayerCount(){
		PlayerInLobby[] players = GameObject.FindObjectsOfType<PlayerInLobby> ();
		playercount = (int)players.Length;
		Debug.Log ("playercount : " + playercount);
	}

	/*
	void OnPhotonRandomJoinFailed(){}
	*/
	/*
	void OnDestroy(){
		PlayerPrefs.SetString ("UserName", PhotonNetwork.player.name);
		Hashtable ht = PhotonNetwork.player.CustomProperties;
		ht["Anne"] = 2;
		PhotonNetwork.player.SetCustomProperties (ht);
	}*/

	int num;
	public void SpawnPlayer(){
		photonView.RPC ("CheckOccupation", PhotonTargets.All);
		GameObject p = PhotonNetwork.Instantiate ("Player/PlayerTokenImage", spots[num].transform.position, spots[num].transform.rotation, 0) as GameObject;
		Vector3 trans1 = p.GetComponentInChildren<Image> ().transform.position;
		Vector3 trans2 = p.GetComponentInChildren<Button> ().transform.position;
		p.GetComponentInChildren<Image> ().transform.position = spots [num].transform.position;
		p.GetComponentInChildren<Button> ().transform.position = new Vector3(150,100,0);//spots [num].transform.position - (trans1 - trans2);
		p.GetComponent<PlayerInLobby> ().playerID = playercount;
		//photonView.RPC ("AddPlayerInLobby", PhotonTargets.All, p.GetComponent<PlayerInLobby> ().canStart);
	}

	[PunRPC]
	void CheckOccupation(){
		do {
			num = Random.Range (0, spots.Length);
		} while (spotOccupied [num]);
		spotOccupied [num] = true;
	}

}
