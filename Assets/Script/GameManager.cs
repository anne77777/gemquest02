using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Photon;

public class GameManager : Photon.MonoBehaviour {

	public static GameManager instance;

	public GameObject testcube;

	[Header("Starting Spots")]
	public Transform[] spots;

	[Header("Player References")]
	public GameObject DiceRollingPanel;
	[Space]
	public GameObject UsePowerUpPanel;
	public Transform UsePowerUpHolder;
	[Space]
	public GameObject SelectPowerUpPanel;
	public Transform SelectPowerUpHolder;
	[Space]
	public GameObject ChoosePortalPanel;

	[Header("Timing")]
	public float UsePowerUpWaitTime;

	[Header("Tile Resources")]
	public Sprite Origin;
	public Sprite PowerUp;

	[Header("Player Info Panel")]
	public GameObject playerInfoPanelPrefab;

	[Header("UI Organizing")]
	public Transform allPlayerInfoPanel;

	[Header("Select Player")]
	public GameObject selectPlayerPanel;
	public Transform selectPlayerHolder;
	public GameObject selectPlayerPrefab;

	[Header("Select Gem")]
	public GameObject selectGemPanel;
	public Transform selectGemHolder;
	public GameObject selectGemPrefab;
	[Space]
	public Sprite gemRed;
	public Sprite gemGreen;
	public Sprite gemBlue;
	public Sprite gemYellow;
	public Sprite gemWhite;
	[Space]
	[Tooltip("For test purpose only!")]public Player testPlayer;

	[Header("Select Dice")]
	public GameObject selectDicePanel;
	public Transform selectDiceHolder;
	public GameObject selectDicePrefab;

	[Header("Select Portal")]
	public Color highlightColor;

	[Header("Select PowerUp")]
	public GameObject selectPowerupPrefab;

	[Header("All PowerUps")]
	public PowerUp[] allPowerUps;

	[Header("Dice")]
	public GameObject DiceObject;

	void Awake(){
		instance = this;
	}

	public List<PlayerInLobby> playerinlobby;
	void Start(){
		
		while(playerinlobby.Count == 0 && playerinlobby.Count != PhotonNetwork.playerList.Length){
			//Debug.Log ("Gonna test!!!");
			playerinlobby.Clear ();
			PlayerInLobby[] pil = GameObject.FindObjectsOfType<PlayerInLobby>();

			for (int i = 0; i < pil.Length; i++) {
				playerinlobby.Add (pil[i]);
			}
			/*
			Debug.Log ("playerinlobby.Count : " + playerinlobby.Count);
			Debug.Log ("PhotonNetwork.playerList.Length : "+PhotonNetwork.playerList.Length);
			Debug.Log ("Not equal!!!");*/
		}

		Debug.Log ("GameManager Initialize!!!");
		Initialize ();
		//photonView.RPC ("Initialize", PhotonTargets.All);
	}


	void Initialize(){
		Object[] go = Resources.LoadAll ("PowerUp", typeof(PowerUp));
		allPowerUps = new PowerUp[go.Length];
		for (int i = 0; i < go.Length; i++) {
			allPowerUps [i] = go [i] as PowerUp;
		}

		PlayerInLobby[] pil2 = GameObject.FindObjectsOfType<PlayerInLobby>();
		Debug.Log ("PlayerInLobby[].Length : " + pil2.Length);
		Debug.Log ("PhotonNetwork.playerList.Length : " + PhotonNetwork.playerList.Length);

		for (int i = 0; i < PhotonNetwork.playerList.Length; i++) {
			if(PhotonNetwork.playerList[i].IsMasterClient){
				for (int j = 0; j < PhotonNetwork.playerList.Length; j++) {
					GenerateToken (j);	
				}							
			}
		}	

		for (int i = 0; i < TurnManager.instance.AllPlayers.Count; i++) {
			if(TurnManager.instance.AllPlayers[i] == null){
				continue;
			}

			GameObject pipp = Instantiate(playerInfoPanelPrefab);
			pipp.transform.SetParent(allPlayerInfoPanel);
			pipp.GetComponent<PlayerInfoPanel> ().player = TurnManager.instance.AllPlayers [i];
			pipp.GetComponent<PlayerInfoPanel> ().UpdatePlayerInfoPanel ();
		}

		PlayerInLobby[] pil = GameObject.FindObjectsOfType<PlayerInLobby>();
		for (int i = 0; i < pil.Length; i++) {
			Destroy (pil[i].gameObject);
		}

		TurnManager.instance.Initialize ();
		//LoadSelectDicePanel ();
		//LoadSelectGemPanel (testPlayer);
		//LoadSelectPlayerPanel ();
		//LoadPortalTileSelection ();
	}

	public Player startplayer;
	public int startindex;

	void GenerateToken(int index){
		Debug.Log ("==============" + index+"==============");
		GameObject go = PhotonNetwork.Instantiate ("Player/PlayerToken", GameManager.instance.spots[index].position, Quaternion.identity, 0);
		TurnManager.instance.AllPlayers [index] = go.GetComponent<Player> ();
		startplayer = go.GetComponent<Player> ();
		startindex = index;
		photonView.RPC ("SetTokenStartTile", PhotonTargets.All);
	}

	[PunRPC]
	void SetTokenStartTile(){
		startplayer.StartTile = spots [startindex].GetComponent<Tile> ();
	}

	public void OrganizeAllPlayerInfoPanel(){
		GameObject[] infopanels = GameObject.FindGameObjectsWithTag ("InfoPanel");
		for (int i = 0; i < infopanels.Length; i++) {
			infopanels [i].transform.SetParent (allPlayerInfoPanel);
		}
	}

	public void LoadSelectPlayerPanel(bool withself= true){
		for (int i = 0; i < TurnManager.instance.AllPlayers.Count; i++) {
			if(!withself && TurnManager.instance.CurrentPlayer == TurnManager.instance.AllPlayers[i]){
				break;
			}

			Player p = TurnManager.instance.AllPlayers [i];

			GameObject go = Instantiate (selectPlayerPrefab);
			go.GetComponent<Image> ().sprite = TurnManager.instance.AllPlayers [i].PlayerIcon;
			go.transform.SetParent (selectPlayerHolder);

			EventTrigger trigger = go.GetComponent<EventTrigger> ();
			EventTrigger.Entry entry = new EventTrigger.Entry ();
			entry.eventID = EventTriggerType.PointerDown;
			entry.callback.AddListener ((data) => {
				ReturnSelectedPlayer(p);
			});
			trigger.triggers.Add(entry);
		}

		selectPlayerPanel.SetActive (true);
	}

	public void ReturnSelectedPlayer(Player player){
		TurnManager.instance.CurrentPlayer.SelectedPlayer = player;
		UnloadSelectPlayerPanel ();
	}

	public void UnloadSelectPlayerPanel(){
		selectPlayerPanel.SetActive (false);
		for (int i = 0; i < selectPlayerPanel.transform.childCount; i++) {
			Destroy (selectPlayerPanel.transform.GetChild(i).gameObject);
		}
	}

	public void LoadSelectGemPanel(Player player, bool self = true){

		//RED
		GameObject gored = Instantiate (selectGemPrefab);
		gored.transform.GetChild(0).gameObject.GetComponent<Image> ().sprite = gemRed;
		gored.transform.SetParent (selectPlayerHolder);

		if (player.GemRed.Amount >= 1) {
			EventTrigger trigger = gored.GetComponent<EventTrigger> ();
			EventTrigger.Entry entry = new EventTrigger.Entry ();
			entry.eventID = EventTriggerType.PointerDown;
			Debug.Log ("+++++++");
			entry.callback.AddListener ((data) => {
				ReturnSelectedGem (GemType.Red, self);
			});
			trigger.triggers.Add (entry);
			Debug.Log ("========");
		} else {
			gored.GetComponent<Image> ().color = Color.grey;
			gored.transform.GetChild(0).gameObject.GetComponent<Image> ().color = Color.grey;
		}

		//GREEN
		GameObject gogreen = Instantiate (selectGemPrefab);
		gogreen.transform.GetChild(0).gameObject.GetComponent<Image> ().sprite = gemGreen;
		gogreen.transform.SetParent (selectPlayerHolder);

		if (player.GemGreen.Amount >= 1) {
			EventTrigger trigger = gogreen.GetComponent<EventTrigger> ();
			EventTrigger.Entry entry = new EventTrigger.Entry ();
			entry.eventID = EventTriggerType.PointerDown;
			Debug.Log ("+++++++");
			entry.callback.AddListener ((data) => {
				ReturnSelectedGem (GemType.Green, self);
			});
			trigger.triggers.Add (entry);
			Debug.Log ("========");
		} else {
			gogreen.GetComponent<Image> ().color = Color.grey;
			gogreen.transform.GetChild(0).gameObject.GetComponent<Image> ().color = Color.grey;
		}

		//Yellow
		GameObject goyellow = Instantiate (selectGemPrefab);
		goyellow.transform.GetChild(0).gameObject.GetComponent<Image> ().sprite = gemYellow;
		goyellow.transform.SetParent (selectPlayerHolder);

		if (player.GemYellow.Amount >= 1) {
			EventTrigger trigger = goyellow.GetComponent<EventTrigger> ();
			EventTrigger.Entry entry = new EventTrigger.Entry ();
			entry.eventID = EventTriggerType.PointerDown;
			Debug.Log ("+++++++");
			entry.callback.AddListener ((data) => {
				ReturnSelectedGem (GemType.Yellow, self);
			});
			trigger.triggers.Add (entry);
			Debug.Log ("========");
		} else {
			goyellow.GetComponent<Image> ().color = Color.grey;
			goyellow.transform.GetChild(0).gameObject.GetComponent<Image> ().color = Color.grey;
		}

		//Blue
		GameObject goblue= Instantiate (selectGemPrefab);
		goblue.transform.GetChild(0).gameObject.GetComponent<Image> ().sprite = gemBlue;
		goblue.transform.SetParent (selectPlayerHolder);

		if (player.GemBlue.Amount >= 1) {
			EventTrigger trigger = goblue.GetComponent<EventTrigger> ();
			EventTrigger.Entry entry = new EventTrigger.Entry ();
			entry.eventID = EventTriggerType.PointerDown;
			Debug.Log ("+++++++");
			entry.callback.AddListener ((data) => {
				ReturnSelectedGem (GemType.Blue, self);
			});
			trigger.triggers.Add (entry);
			Debug.Log ("========");
		} else {
			goblue.GetComponent<Image> ().color = Color.grey;
			goblue.transform.GetChild(0).gameObject.GetComponent<Image> ().color = Color.grey;
		}

		//White
		GameObject gowhite = Instantiate (selectGemPrefab);
		gowhite.transform.GetChild(0).gameObject.GetComponent<Image> ().sprite = gemWhite;
		gowhite.transform.SetParent (selectPlayerHolder);

		if (player.GemWhite.Amount >= 1) {
			EventTrigger trigger = gowhite.GetComponent<EventTrigger> ();
			EventTrigger.Entry entry = new EventTrigger.Entry ();
			entry.eventID = EventTriggerType.PointerDown;
			Debug.Log ("+++++++");
			entry.callback.AddListener ((data) => {
				ReturnSelectedGem (GemType.White, self);
			});
			trigger.triggers.Add (entry);
			Debug.Log ("========");
		} else {
			gowhite.GetComponent<Image> ().color = Color.grey;
			gowhite.transform.GetChild(0).gameObject.GetComponent<Image> ().color = Color.grey;
		}

		selectPlayerPanel.SetActive (true);
	}

	public void ReturnSelectedGem(GemType gemtype, bool self = true){
		Debug.Log ("Selected a gem!!! (start)");
		if (self) {
			testPlayer.SelfGemType = gemtype;
			//TurnManager.instance.CurrentPlayer.SelfGemType = gemtype;
		} else {
			testPlayer.TargetGemType = gemtype;
			//TurnManager.instance.CurrentPlayer.TargetGemType = gemtype;
		}

		Debug.Log ("Selected a gem!!! (end)");
		UnloadSelectGemPanel ();
	}

	public void UnloadSelectGemPanel(){
		selectGemPanel.SetActive (false);
		for (int i = 0; i < selectPlayerPanel.transform.childCount; i++) {
			Destroy (selectPlayerPanel.transform.GetChild(i).gameObject);
		}
	}

	public void LoadSelectDicePanel(){
		for (int i = 1; i <= 6; i++) {

			int amount = i;

			GameObject go = Instantiate (selectDicePrefab);
			go.GetComponentInChildren<Text> ().text = i.ToString();
			go.transform.SetParent (selectPlayerHolder);

			EventTrigger trigger = go.GetComponent<EventTrigger> ();
			EventTrigger.Entry entry = new EventTrigger.Entry ();
			entry.eventID = EventTriggerType.PointerDown;
			entry.callback.AddListener ((data) => {
				ReturnSelectedDice(amount);
			});
			trigger.triggers.Add(entry);
		}

		selectPlayerPanel.SetActive (true);
	}

	public void ReturnSelectedDice(int i){
		TurnManager.instance.CurrentPlayer._DicePowerUpValue = i;
		UnloadSelectPlayerPanel ();
	}

	public void UnloadSelectDicePanel(){
		selectDicePanel.SetActive (false);
		for (int i = 0; i < selectDicePanel.transform.childCount; i++) {
			Destroy (selectDicePanel.transform.GetChild(i).gameObject);
		}
	}

	Color portaloriginalcolor;
	public void LoadPortalTileSelection(){
		GameObject[] portals = GameObject.FindGameObjectsWithTag ("PortalTile");

		if(portals.Length > 0){
			portaloriginalcolor = portals [0].transform.Find ("Cube").GetComponent<MeshRenderer> ().material.color;
		}

		for (int i = 0; i < portals.Length; i++) {
			GameObject go = portals [i].transform.Find ("Cube").gameObject;
			go.GetComponent<MeshRenderer> ().material.color = highlightColor;
		}
	}

	public void UnloadPortalTileSelection(){
		GameObject[] portals = GameObject.FindGameObjectsWithTag ("PortalTile");

		for (int i = 0; i < portals.Length; i++) {
			GameObject go = portals [i].transform.Find ("Cube").gameObject;
			go.GetComponent<MeshRenderer> ().material.color = portaloriginalcolor;
		}
	}

	public void ReloadAllPlayerInfo(){
		Debug.Log ("Gonna reload all players' info!");
	}

	public void DecidePortalTransportation(bool decision){
		Debug.Log ("Iniside the function!!!");
		TurnManager.instance.CurrentPlayer.testportal = decision;
		Debug.Log ("Test portal : " + TurnManager.instance.CurrentPlayer.testportal);
		if (decision) {
			if (TurnManager.instance.CurrentPlayer.DestinationPortal != null) {
				TurnManager.instance.CurrentPlayer.SetPlayerPositionTile (TurnManager.instance.CurrentPlayer.DestinationPortal);
				TurnManager.instance.CurrentPlayer.CurrentTile = TurnManager.instance.CurrentPlayer.DestinationPortal;
				TurnManager.instance.CurrentPlayer.OriginalWay = WayType.None;
			} else {
				Debug.Log ("You wanna transport but the destination portal is null!");
			}
		} else {
			TurnManager.instance.CurrentPlayer.DestinationPortal = null;
		}

		ChoosePortalPanel.SetActive (false);
		TurnManager.instance.CurrentPlayer._PortalDesicion = true;
	}
}
