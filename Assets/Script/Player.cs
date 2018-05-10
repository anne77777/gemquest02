using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon;

public class Player : Photon.MonoBehaviour {

	public static float TileSize = 1.0f;

	public bool testportal = false;

	public Text diceNum;
	public Text diceTimer;
	public Die dice;
	public Quaternion selectedRot;
	public bool isrolling;

	[Header("Player Basics")]
	public Sprite PlayerIcon;
	public int PlayerID = -1;
	public GameObject PlayerToken;
	[Header("Player Selection")]
	public Player SelectedPlayer;
	[Header("Gem Selection")]
	public GemType SelfGemType;
	public GemType TargetGemType;
	[Header("Script Logic")]
	public bool _SelectedWay = false;
	[Space]
	public bool _RolledDice = false;
	public bool _FinishRolling = false;
	public bool _TurnExecuted = false;
	[Space]
	public bool _PortalDesicion = false;
	public Tile DestinationPortal;
	public bool _PortalFirst = false;
	public bool _PortalSecond = false;
	[Space]
	public bool _AskUsingPowerUp = false;
	public bool _WhetherUsePowerUp = false;
	public bool _PowerUpAffected = false;
	[Space]
	public bool _SelectedPlayer = false;
	public bool _SelfGemType = false;
	public bool _TargetGemType = false;
	public bool _SelectedPlayerAsked = false;
	public bool _SelfGemTypeAsked = false;
	public bool _TargetGemTypeAsked = false;
	[Space]
	public bool _SelectedTile = false;
	public Tile SelectedTile;
	[Space]
	public bool _SelectingPowerUp = false;
	public bool _RemovePowerUp = false;
	[Space]
	public bool _UseDicePowerUp = false;
	public bool _SelectDiceValue = false;
	public bool _SelectDiceValueAsked = false;
	public int _DicePowerUpValue = 0;
	[Header("Dice")]
	public Rigidbody Dice;
	public GameObject DiceRollingPanel;//*
	[Header("Tile Logic")]
	public GameObject Detector;
	public Tile StartTile;
	public Tile CurrentTile;
	public Tile NextTile;
	public int CurrentMoves = 0;
	public WayType OriginalWay;
	public WayType CurrentWay;
	[Header("Gems")]
	public Gem GemRed = new Gem (GemType.Red);
	public Gem GemBlue = new Gem (GemType.Blue);
	public Gem GemYellow = new Gem (GemType.Yellow);
	public Gem GemGreen = new Gem (GemType.Green);
	public Gem GemWhite = new Gem (GemType.White);
	[Header("Power-Ups")]
	public PowerUp CurrentPowerUp;
	public List<PowerUp> PowerUps;
	[Space]
	public float UsePowerUpWaitTime;
	public GameObject UsePowerUpPanel;//*
	public Transform UsePowerUpHolder;//*
	public Transform DisplayPowerUpHolder;
	[Space]
	public PowerUp GetPowerUp;
	public GameObject SelectPowerUpPanel;
	public Transform SelectPowerUpHolder;
	[Header("Way Choosing")]
	public GameObject Up;
	public GameObject Down;
	public GameObject Left;
	public GameObject Right;
	[Header("Portal Choosing")]
	public GameObject ChoosePortalPanel;
	[Header("UI")]
	public Image[] PowerUpImages;
	public Text[] KeyAmountTexts;

	PhotonView photonview;

	public void InitializePlayerPun(){
		photonView.RPC ("InitializePlayer", PhotonTargets.All);
	}

	[PunRPC]
	void InitializePlayer(){
		photonview = GetComponent<PhotonView> ();

		if(StartTile != null){
			CurrentTile = StartTile;
		}else{
			Debug.Log("Something wrong with the starting tile!");	
		}

		UsePowerUpWaitTime = GameManager.instance.UsePowerUpWaitTime;

		DiceRollingPanel = GameManager.instance.DiceRollingPanel;
		UsePowerUpPanel = GameManager.instance.UsePowerUpPanel;
		UsePowerUpHolder = GameManager.instance.UsePowerUpHolder;
		SelectPowerUpPanel = GameManager.instance.SelectPowerUpPanel;
		SelectPowerUpHolder = GameManager.instance.SelectPowerUpHolder;
		ChoosePortalPanel = GameManager.instance.ChoosePortalPanel;

		//LoadDisplayPowerUp();
		InactiveWayArrows ();
		UsePowerUpPanel.SetActive (false);
		ChoosePortalPanel.SetActive (false);
		UpdatePlayerUI ();
		_RolledDice = false;
	}

	void OnEnable(){
		Debug.Log ("Enabled!");
	}

	bool active = false;
	void Update(){
		//ChooseTile ("PortalTile");
		if(photonview != null && !photonview.isMine){
			return;
		}

		if(TurnManager.instance.CurrentPlayer == null || TurnManager.instance.CurrentPlayer != this){
			return;
		}

		if(Input.GetKeyDown(KeyCode.A)){
			if (!active) {
				Debug.Log ("Active : Input.GetKeyDown(KeyCode.A)");
				//GameManager.instance.UsePowerUpPanel.SetActive (true);
				//GameManager.instance.SelectPowerUpPanel.SetActive (true);
				ChoosePortalPanel.SetActive (true);
				active = true;
			} else {
				Debug.Log ("Inactive : Input.GetKeyDown(KeyCode.A)");
				active = false;
				//GameManager.instance.UsePowerUpPanel.SetActive (false);
				//GameManager.instance.SelectPowerUpPanel.SetActive (false);
				ChoosePortalPanel.SetActive (false);
			}
		}

		if(Input.GetKeyDown(KeyCode.P)){
			Player[] p = GameObject.FindObjectsOfType<Player> ();
			Player p1 = null;
			for (int i = 0; i < p.Length; i++) {
				if(p[i] != this){
					p1 = p [i];
				}
			}

			PowerUps [0].Activate (this, p1);
			UpdatePlayerUI ();
		}

		if(_SelectingPowerUp){
			return;
		}

		if(_WhetherUsePowerUp){
			ExitUsingPowerUp ();
			StartCoroutine(ActivatePowerUp ());
		}

		if (_FinishRolling) {
			if(_AskUsingPowerUp){			

				if (!_TurnExecuted && (_WhetherUsePowerUp ? _PowerUpAffected : true)) {
					StartTurn ();
					/*
					if(_WhetherUsePowerUp){
						ActivatePowerUp ();
					}

					if(_PowerUpAffected){
						StartTurn ();						
					}*/
				}
			}

			dice.transform.rotation = selectedRot;
		}

		if(!_RolledDice){
			DiceRollingPanel.SetActive (true);
		}
	}

	public void StartTurn(){
		_TurnExecuted = true;
		SetCurrentWay ();
		SetDetectorPosition ();
		StartCoroutine (CheckWay());	
	}

	IEnumerator CheckWay(){
		yield return new WaitForSeconds (0.5f);
		if(CurrentMoves == 0){
			Debug.Log ("No moves available!");
		}

		for (int i = 0; i < CurrentMoves; i++) {

			int selectpowerupcountdown = 20;
			while(_SelectingPowerUp){
				selectpowerupcountdown--;
				if(selectpowerupcountdown <= 0){
					EndSelectPowerUp ();
					_SelectingPowerUp = false;
					break;
				}
				yield return new WaitForSeconds (0.1f);
			}

			if(CurrentTile.ThisTileType == TileType.PORTAL){
				int pc = 10;
				while(!_PortalDesicion && pc>0){
					pc--;
					Debug.Log ("p : " + pc);
					yield return new WaitForSeconds (1.0f);
				}
				_PortalDesicion = false;

				yield return new WaitForSeconds (1.0f);
			}

			int ways = 0;
			int index = 0;
			for (int j = 0; j < CurrentTile.AvailableWays.Length; j++) {
				Debug.Log ("Way " + j + " = " +CurrentTile.AvailableWays[j]);
				if(CurrentTile.AvailableWays[j] != OriginalWay){
					if (CurrentTile.AvailableWays [j] == WayType.GOAL) {
						if (GemRed.Amount >= 1 && GemGreen.Amount >= 1 && GemBlue.Amount >= 1 && GemYellow.Amount >= 1 && GemWhite.Amount >= 1) {
							ways++;
						}
					} else if (CurrentTile.AvailableWays [j] != WayType.ORIGIN) {
						index = j;
						ways++;
					}
				}
			}

			if(CurrentTile.ThisTileType == TileType.PORTAL){
				Debug.Log ("PORTAL WAYS : " + ways);
				yield return new WaitForSeconds (1.0f);
			}

			if (ways > 1) {
				Debug.Log ("ways > 1");
				ChooseWay ();

				int waitselectway = 50;
				while(!_SelectedWay && waitselectway > 0){
					waitselectway--;
					yield return new WaitForSeconds (0.1f);
				}

				_SelectedWay = false;

				SetCurrentWay ();
				Vector3 pos = FacePosition (CurrentWay);
				pos += this.transform.position;
				pos.y = Detector.transform.position.y;
				Detector.transform.position = pos;
				yield return new WaitForSeconds (0.5f);

				Move ();

			} else if (ways == 1) {
				Debug.Log ("ways == 1");
				SetCurrentWay ();
				CurrentWay = CurrentTile.AvailableWays[index];
				Vector3 pos = FacePosition (CurrentWay);
				pos += this.transform.position;
				pos.y = Detector.transform.position.y;
				Detector.transform.position = pos;
				yield return new WaitForSeconds (0.5f);

				Move ();
			} else {
				Debug.Log ("Something wrong in " + CurrentTile);
			}
			SetOriginalWay ();
			yield return new WaitForSeconds (1.0f);
		}
		CurrentMoves = 0;
		_FinishRolling = false;
		_TurnExecuted = false;
		TurnManager.instance.NextPlayerTurn ();
		yield return new WaitForSeconds (0.0f);
	}

	Vector3 FacePosition(WayType way){
		switch(way){
		case WayType.UP:
			return new Vector3 (0,0,TileSize);
		case WayType.DOWN:
			return new Vector3 (0,0,-TileSize);
		case WayType.LEFT:
			return new Vector3 (-TileSize, 0, 0);
		case WayType.RIGHT:
			return new Vector3 (TileSize, 0, 0);
		default:
			Debug.Log ("Something wrong in the next tile!");
			return Vector3.zero;
		}
	}

	void Move(){
		while(Vector3.Distance(this.transform.position, NextTile.gameObject.transform.position) > 0.01f){
			this.transform.position = Vector3.Lerp (this.transform.position,
				NextTile.gameObject.transform.position,
				10.0f * Time.deltaTime);
		}
		CurrentTile = NextTile;
		InactiveWayArrows ();
	}

	void SetCurrentWay(){
		if (CurrentWay == WayType.None) {
			List<WayType> ws = new List<WayType> ();

			for (int i = 0; i < CurrentTile.AvailableWays.Length; i++) {
				if (CurrentTile.AvailableWays[i] != OriginalWay) {
					if(CurrentTile.AvailableWays[i] == WayType.GOAL){
						if (GemRed.Amount >= 1 && GemGreen.Amount >= 1 && GemBlue.Amount >= 1 && GemYellow.Amount >= 1 && GemWhite.Amount >= 1) {
							ws.Add (CurrentTile.AvailableWays[i]);
						} else {
							continue;
						}
					}else if(CurrentTile.AvailableWays[i] == WayType.ORIGIN){
						continue;
					}else{
						ws.Add (CurrentTile.AvailableWays[i]);
					}
				}
			}

			CurrentWay = ws [Random.Range (0, ws.Count)];
		} 
	}

	public void SetOriginalWay(){
		switch(CurrentWay){
		case WayType.UP:
			OriginalWay = WayType.DOWN;
			break;
		case WayType.DOWN:
			OriginalWay = WayType.UP;
			break;
		case WayType.LEFT:
			OriginalWay = WayType.RIGHT;
			break;
		case WayType.RIGHT:
			OriginalWay = WayType.LEFT;
			break;
		default:
			Debug.Log ("Original way is none!");
			OriginalWay = WayType.DOWN;
			break;
		}
	}

	public void SetDetectorPosition(){
		Detector.transform.position = FacePosition (CurrentWay);
	}

	void ChooseWay(){
		CurrentWay = WayType.None;
		bool isgoal = false;
		foreach (var item in CurrentTile.AvailableWays) {
			if(item != OriginalWay){
				if(item == WayType.UP){
					Up.SetActive(true);
				}else if(item == WayType.DOWN){
					Down.SetActive (true);
				}else if(item == WayType.LEFT){
					Left.SetActive (true);
				}else if(item == WayType.RIGHT){
					Right.SetActive (true);
				}				
			}

			if(item == WayType.GOAL){
				isgoal = true;
			}
		}

		if(isgoal){
			if (CurrentTile.GoalWay == WayType.UP) {
				Up.SetActive (true);
			} else if (CurrentTile.GoalWay == WayType.DOWN) {
				Down.SetActive (true);
			} else if (CurrentTile.GoalWay == WayType.LEFT) {
				Left.SetActive (true);
			} else if (CurrentTile.GoalWay == WayType.RIGHT) {
				Right.SetActive (true);
			} else {
				Debug.Log ("there's something wrong in your goal way!");
			}
		}
	}

	public void InactiveWayArrows(){
		Up.SetActive (false);
		Down.SetActive (false);
		Left.SetActive (false);
		Right.SetActive (false);
	}

	public void RollDice(){
		_AskUsingPowerUp = true;
		StartCoroutine (RollDiceNumber());
	}

	IEnumerator RollDiceNumber(){		
		_RolledDice = true;
		_FinishRolling = false;

		Dice.AddTorque (new Vector3(Random.Range(-20000.0f, 20000.0f), Random.Range(-20000.0f, 20000.0f), Random.Range(-20000.0f, 20000.0f)));
		yield return new WaitForSeconds (0.8f);
		Dice.angularVelocity = Vector3.zero;

		switch(Random.Range(1, 7)){
		case 1:
			TurnManager.instance.CurrentPlayer.CurrentMoves = 1;
			selectedRot = Quaternion.Euler(new Vector3(-90, 180,0));
			break;

		case 2:
			TurnManager.instance.CurrentPlayer.CurrentMoves = 2;
			selectedRot = Quaternion.Euler(new Vector3(-180, 90,0));
			break;

		case 3:
			TurnManager.instance.CurrentPlayer.CurrentMoves = 3;
			selectedRot = Quaternion.Euler(new Vector3(0, 0,270));
			break;

		case 4:
			TurnManager.instance.CurrentPlayer.CurrentMoves = 4;
			selectedRot = Quaternion.Euler(new Vector3(0,-180,90));
			break;

		case 5:
			TurnManager.instance.CurrentPlayer.CurrentMoves = 5;
			selectedRot = Quaternion.Euler(new Vector3(0, 0,0));
			break;

		case 6:
			TurnManager.instance.CurrentPlayer.CurrentMoves = 6;
			selectedRot = Quaternion.Euler(new Vector3(-270, 90,0));
			break;
		default:
			Debug.Log ("Dice rolled to default. Something is wrong.");
			break;
		}

		Dice.gameObject.transform.rotation = Quaternion.Slerp (Dice.gameObject.transform.rotation, selectedRot, 1.0f);
		Dice.angularVelocity = Vector3.zero;
		yield return new WaitForSeconds (1.5f);
		_FinishRolling = true;
		DiceRollingPanel.SetActive (false);
		_RolledDice = false;
	}

	public void GetStepCount (){
		if (_RolledDice) {
			Debug.Log ("Dice not rolling");
			diceNum.text = "" + dice.value;
			CurrentMoves = dice.value;
		}
	}

	public void UpdatePlayerUI(){
		//Debug.Log ("This functionality is moved to elsewhere.");
		/*
		KeyAmountTexts [0].text = ""+GemRed.Amount;
		KeyAmountTexts [1].text = ""+GemGreen.Amount;
		KeyAmountTexts [2].text = ""+GemYellow.Amount;
		KeyAmountTexts [3].text = ""+GemBlue.Amount;
		KeyAmountTexts [4].text = ""+GemWhite.Amount;*/
	}

	public void SetPlayerPositionTile(Tile tile){
		this.gameObject.transform.position = tile.gameObject.transform.position;
	}

	IEnumerator ActivatePowerUp(){
		int count = (int)(UsePowerUpWaitTime / 0.05f);
		int i = 0;
		while(!PowerUpAllPrepared()){
			PrepareToUsePowerUp ();
			i++;
			if(i >= count){
				break;
			}

			yield return new WaitForSeconds (0.05f);
		}

		Debug.Log ("ActivatePowerUp() function ends.");
		Activate ();
		GameManager.instance.ReloadAllPlayerInfo ();
		yield return new WaitForSeconds (0.0f);
	}

	void Activate(){
		if(CurrentPowerUp == null){
			Debug.Log ("Something wrong : No CurrentPowerUp yet you want to activate it!");
			return;
		}

		if(CurrentPowerUp is PowerUpBackToStartTile){
			CurrentPowerUp.Activate (SelectedPlayer);
			return;
		}

		if(CurrentPowerUp is PowerUpDiceJelly){
			CurrentPowerUp.Activate (this);
			return;
		}

		if(CurrentPowerUp is PowerUpDiceMax){
			CurrentPowerUp.Activate (this);
			return;
		}

		if(CurrentPowerUp is PowerUpGrabGem){
			CurrentPowerUp.Activate (this, SelectedPlayer, SelfGemType, TargetGemType);
			return;
		}

		if(CurrentPowerUp is PowerUpJumpToPortal){
			CurrentPowerUp.Activate (this, SelectedTile);
			return;
		}

		if(CurrentPowerUp is PowerUpRandomPlayerPosition){
			Debug.Log ("I haven't tackle with this yet!");
			return;
		}

		if(CurrentPowerUp is PowerUpRandomPlayerPowerUp){
			CurrentPowerUp.Activate (SelectedPlayer);
			return;
		}

		if(CurrentPowerUp is PowerUpRecolorGem){
			CurrentPowerUp.Activate (SelectedPlayer, SelectedPlayer, SelfGemType, TargetGemType);
			return;
		}

		if(CurrentPowerUp is PowerUpReverseDirection){
			CurrentPowerUp.Activate (SelectedPlayer);
			return;
		}

		if(CurrentPowerUp is PowerUpSkipAhead){
			CurrentPowerUp.Activate (this, SelectedPlayer);
			return;
		}

		if(CurrentPowerUp is PowerUpSurplus){
			CurrentPowerUp.Activate (this, SelectedPlayer);
			return;
		}

		if(CurrentPowerUp is PowerUpSwitchGem){
			CurrentPowerUp.Activate (SelectedPlayer, SelectedPlayer, SelfGemType, TargetGemType);
			return;
		}

		if(CurrentPowerUp is PowerUpSwitchPosition){
			CurrentPowerUp.Activate (this, SelectedPlayer);
			return;
		}

	}

	public void ThisTurnUsePowerUp(){
		_AskUsingPowerUp = true;
		UsePowerUpPanel.SetActive (true);
		LoadChoosePowerUp ();
	}

	void LoadChoosePowerUp(){
		for (int i = 0; i < PowerUps.Count ; i++) {
			UsePowerUpHolder.GetChild (i).GetComponent<DisplayPowerUp> ().ShowPowerUpInfo(PowerUps[i]);
		}
	}

	void LoadDisplayPowerUp(){
		for (int i = 0; i < PowerUps.Count ; i++) {
			DisplayPowerUpHolder.GetChild (i).GetComponentInChildren<DisplayPowerUp> ().ShowPowerUpInfo(PowerUps[i]);
		}
	}

	public void ExitUsingPowerUp(bool use = true){
		UsePowerUpPanel.SetActive (false);
		_AskUsingPowerUp = true;
		_WhetherUsePowerUp = use;
		Debug.Log ("PowerUp : " + _AskUsingPowerUp + " : " +_WhetherUsePowerUp);
	}

	public void PrepareToUsePowerUp(){
		if(CurrentPowerUp is PowerUpBackToStartTile){
			if(!_SelectedPlayer){
				if(_SelectedPlayerAsked){
					return;
				}
				GameManager.instance.LoadSelectPlayerPanel ();
				_SelectedPlayerAsked = true;
			}
		}

		if(CurrentPowerUp is PowerUpDiceJelly){
			if(!_SelectDiceValue){
				if(_SelectDiceValueAsked){
					return;
				}
				GameManager.instance.LoadSelectDicePanel ();
				_SelectDiceValueAsked = true;
			}
		}

		if(CurrentPowerUp is PowerUpDiceMax){
			return;
		}

		if(CurrentPowerUp is PowerUpGrabGem){
			if (!_SelectedPlayer) {
				if(_SelectedPlayerAsked){
					return;
				}
				GameManager.instance.LoadSelectPlayerPanel ();
				_SelectedPlayerAsked = true;
			} else {
				if (!_SelfGemType) {
					if(_SelfGemTypeAsked){
						return;
					}
					GameManager.instance.LoadSelectGemPanel (this);
					_SelfGemTypeAsked = true;
				} else {
					if (!_TargetGemType && SelectedPlayer != null) {
						if(_TargetGemTypeAsked){
							return;
						}
						GameManager.instance.LoadSelectGemPanel (SelectedPlayer, false);
						_TargetGemTypeAsked = true;
					} else {
						Debug.LogError ("Something wrong here.");
					}
				}
			}
		}

		if(CurrentPowerUp is PowerUpJumpToPortal){
			if(_SelectedTile){
				return;
			}
			ChooseTile ("PortalTile");
		}

		if(CurrentPowerUp is PowerUpRandomPlayerPosition){
			Debug.Log ("I haven't tackle with this yet!");
			return;
		}

		if(CurrentPowerUp is PowerUpRandomPlayerPowerUp){
			if(!_SelectedPlayer){
				if(_SelectedPlayerAsked){
					return;
				}
				GameManager.instance.LoadSelectPlayerPanel ();
				_SelectedPlayerAsked = true;
			}
		}

		if(CurrentPowerUp is PowerUpRecolorGem){
			if (!_SelectedPlayer) {
				if(_SelectedPlayerAsked){
					return;
				}
				GameManager.instance.LoadSelectPlayerPanel ();
				_SelectedPlayerAsked = true;
			} else {
				if (!_SelfGemType) {
					if(_SelfGemTypeAsked){
						return;
					}
					GameManager.instance.LoadSelectGemPanel (this);
					_SelfGemTypeAsked = true;
				} else {
					if (!_TargetGemType && SelectedPlayer != null) {
						if(_TargetGemTypeAsked){
							return;
						}
						GameManager.instance.LoadSelectGemPanel (SelectedPlayer, false);
					} else {
						Debug.LogError ("Something wrong here.");
					}
				}
			}
		}

		if(CurrentPowerUp is PowerUpReverseDirection){
			if(!_SelectedPlayer){
				if(_SelectedPlayerAsked){
					return;
				}
				GameManager.instance.LoadSelectPlayerPanel ();
				_SelectedPlayerAsked = true;
			}
		}

		if(CurrentPowerUp is PowerUpSkipAhead){
			Debug.Log ("I still need to tackle with the direction.");
			if(!_SelectedPlayer){
				if(_SelectedPlayerAsked){
					return;
				}
				GameManager.instance.LoadSelectPlayerPanel ();
				_SelectedPlayerAsked = true;
			}
		}

		if(CurrentPowerUp is PowerUpSurplus){
			if(!_SelectedPlayer){
				if(_SelectedPlayerAsked){
					return;
				}
				GameManager.instance.LoadSelectPlayerPanel ();
				_SelectedPlayerAsked = true;
			}
		}

		if(CurrentPowerUp is PowerUpSwitchGem){
			if (!_SelectedPlayer) {
				if(_SelectedPlayerAsked){
					return;
				}
				GameManager.instance.LoadSelectPlayerPanel ();
				_SelectedPlayerAsked = true;
			} else {
				if (!_SelfGemType) {
					if(_SelfGemTypeAsked){
						return;
					}
					GameManager.instance.LoadSelectGemPanel (this);
				} else {
					if (!_TargetGemType && SelectedPlayer != null) {
						if(_TargetGemTypeAsked){
							return;
						}
						GameManager.instance.LoadSelectGemPanel (SelectedPlayer, false);
						_TargetGemTypeAsked = true;
					} else {
						Debug.LogError ("Something wrong here.");
					}
				}
			}
		}

		if(CurrentPowerUp is PowerUpSwitchPosition){
			if(!_SelectedPlayer){
				if(_SelectedPlayerAsked){
					return;
				}
				GameManager.instance.LoadSelectPlayerPanel ();
				_SelectedPlayerAsked = true;
			}
		}
	}

	bool PowerUpAllPrepared(){
		if(CurrentPowerUp == null){
			Debug.LogError ("There's no current powerup ofr this player!");
			return true;
		}

		if(CurrentPowerUp is PowerUpBackToStartTile){
			return _SelectedPlayer;
		}

		if(CurrentPowerUp is PowerUpDiceJelly){
			return _SelectDiceValue;
		}

		if(CurrentPowerUp is PowerUpDiceMax){
			return _SelectDiceValue;
		}

		if(CurrentPowerUp is PowerUpGrabGem){
			return _SelectedPlayer && _SelfGemType && _TargetGemType;
		}

		if(CurrentPowerUp is PowerUpJumpToPortal){
			return _SelectedTile;
		}

		if(CurrentPowerUp is PowerUpRandomPlayerPosition){
			Debug.Log ("I haven't tackle with this yet!");
			return true;
		}

		if(CurrentPowerUp is PowerUpRandomPlayerPowerUp){
			return _SelectedPlayer;
		}

		if(CurrentPowerUp is PowerUpRecolorGem){
			return _SelectedPlayer && _SelfGemType && _TargetGemType;
		}

		if(CurrentPowerUp is PowerUpReverseDirection){
			return _SelectedPlayer;
		}

		if(CurrentPowerUp is PowerUpSkipAhead){
			Debug.Log ("I still need the direction for multi way circumstances!");
			return _SelectedPlayer;
		}

		if(CurrentPowerUp is PowerUpSurplus){
			return _SelectedPlayer;
		}

		if(CurrentPowerUp is PowerUpSwitchGem){
			return _SelectedPlayer && _SelfGemType && _TargetGemType;
		}

		if(CurrentPowerUp is PowerUpSwitchPosition){
			return _SelectedPlayer;
		}

		Debug.LogError ("After all the checking of different powerups, we haven't found anything!");
		return true;
	}

	void ChooseTile(string target = "Tile"){
		RaycastHit[] hits;
		hits = Physics.RaycastAll (Camera.main.ScreenPointToRay (Input.mousePosition));

		for (int i = 0; i < hits.Length; i++) {
			if(hits[i].collider.transform.parent.tag == target){
				if (Input.GetMouseButtonDown (0)) {
					SelectedTile = hits [i].collider.transform.parent.GetComponent<Tile> ();
					_SelectedTile = true;
					Debug.Log ("Selected a " + target + " !");
					GameManager.instance.UnloadPortalTileSelection ();
				}
			}
		}
	}

	public void SelectPowerUp(){
		GameObject prefab = GameManager.instance.selectPowerupPrefab;

		for (int i = 0; i < PowerUps.Count; i++) {
			GameObject go = Instantiate (prefab);
			go.transform.SetParent (SelectPowerUpHolder);
			go.GetComponent<DisplayPowerUp> ().ShowPowerUpInfo (PowerUps [i]);
		}

		SelectPowerUpPanel.SetActive (true);
	}

	public void AddSelectPowerUp(){
		if(_RemovePowerUp){
			PowerUps.Add (GetPowerUp);
			GetPowerUp = null;
		}
	}

	public void EndSelectPowerUp(){
		SelectPowerUpPanel.SetActive (false);
		_SelectingPowerUp = false;
		
		for (int i = 0; i < SelectPowerUpHolder.transform.childCount; i++) {
			Destroy (SelectPowerUpHolder.transform.GetChild(i).gameObject);
		}
	}
}
