using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum WayType{
	None,
	UP,
	DOWN,
	LEFT,
	RIGHT,
	ORIGIN,
	GOAL
}

public enum TileType{
	NONE,
	ORIGIN,
	GOAL,
	PORTAL,
	TREASURE,
	POWERUP,
	EVENT,
	GEM
}

public class Tile : MonoBehaviour {
	
	[Header("Tile Logic")]
	public TileType ThisTileType;
	public bool Occupied;
	[Header("Way Info")]
	public WayType[] AvailableWays;
	[Header("Tile Icon")]
	public GameObject TileIcon;
	[Header("Gem Info")]
	public GemType Gem;
	[Header("Portal Relation")]
	public Tile Destination;
	[Header("Goal Direction")]
	public WayType GoalWay;

	void Start(){
		if(ThisTileType == TileType.POWERUP){
			TileIcon.GetComponent<SpriteRenderer>().sprite = GameManager.instance.PowerUp;
		}
	}

	void OnTriggerEnter(Collider other){
		//Debug.Log ("Player entered tile : " + this.gameObject.name);
		Player p = other.gameObject.GetComponentInParent<Player>();
		//Debug.Log ("p first : " + p._PortalFirst);
		//Debug.Log ("p second : " + p._PortalSecond);
		if (p != null && other.gameObject.name.Contains("Base")) {
			//Debug.Log ("player has the component");
			switch (ThisTileType) {
			case TileType.ORIGIN:
				if (AvailableWays.Length == 1) {
					p.CurrentTile = this;
					p.CurrentWay = AvailableWays [0];
					p.SetOriginalWay ();
					p.SetDetectorPosition ();
				} else {
					Debug.Log ("Suspicious way count of the origin tile!");
				}
				break;

			case TileType.GOAL:
				Debug.Log ("End game!");
				break;

			case TileType.PORTAL:
				//Debug.Log ("p first : " + p._PortalFirst);
				//Debug.Log ("p second : " + p._PortalSecond);
				/*if(!p._PortalFirst && !p._PortalSecond){
					if (Destination != null) {
						p.SetPlayerPositionTile (Destination);
						p.OriginalWay = WayType.None;
						p.CurrentTile = Destination;
						p._PortalFirst = true;
					}
				} else if(p._PortalFirst && !p._PortalSecond){
					p._PortalFirst = false;
					p._PortalSecond = true;
				} else {
					Debug.Log ("Portal destination is none!");
				}*/

				if (!p._PortalFirst && !p._PortalSecond && p.CurrentTile == this) {
					p.ChoosePortalPanel.SetActive (true);
					p.DestinationPortal = this.Destination;
				}

				break;

			case TileType.GEM:
				Debug.Log ("name of the entered object : " + other.gameObject.name);
				if (other.gameObject.name.Contains ("Base")) {
					Debug.Log ("should be only the base getting");
					switch (Gem) {
					case GemType.Red:
						p.GemRed.Amount++;
						break;
					case GemType.Blue:
						p.GemBlue.Amount++;
						break;
					case GemType.Green:
						p.GemGreen.Amount++;
						break;
					case GemType.Yellow:
						p.GemYellow.Amount++;
						break;
					case GemType.White:
						p.GemWhite.Amount++;
						break;
					}
				}
				p.UpdatePlayerUI ();
				break;

			case TileType.POWERUP:
				Debug.Log ("POWER UP GET!");
				int index = 0;
				for (int i = 0; i < p.PowerUps.Count; i++) {
					if (p.PowerUps [i] != null) {
						index++;
					}
				}

				PowerUp powerup = GameManager.instance.allPowerUps [Random.Range (0, GameManager.instance.allPowerUps.Length)];
				if (index >= 3) {
					p.GetPowerUp = powerup;
					p._SelectingPowerUp = true;
					p.SelectPowerUp ();
				} else {
					for (int i = 0; i < p.PowerUps.Count; i++) {
						if (p.PowerUps [i] == null) {
							p.PowerUps [i] = powerup;
						}
					}
				}
				//p.PowerUps.Add (new PowerUp ());
				break;

			case TileType.EVENT:
				Debug.Log ("Event happens!");
				break;

			case TileType.TREASURE:
				Debug.Log ("Opened a treasure box!");
				break;

			default:
				//Debug.Log ("Nothing in this tile.");
				break;
			}
		}/* else {
			Debug.Log ("don't have player component");
		}*/
	}

	void OnTriggerExit(Collider other){
		//Debug.Log ("XXXXXXXXXXXXXXXXXXXXXXXXXXXXX");
		Player p = other.gameObject.GetComponentInParent<Player>();
		//Debug.Log ("dsghrtjgfsssssssssssssssssssssssssssssss");
		if (p != null) {
			switch (ThisTileType){
			case TileType.PORTAL:
				if(p._PortalSecond){
					//Debug.Log ("++++++++++++++++++++++++++");
					p._PortalSecond = false;
					p._PortalDesicion = false;
				}else if(p._PortalFirst && !p._PortalSecond){
					p._PortalFirst = false;
					p._PortalSecond = true;
				}
				break;
			}
		} else {
			Debug.Log ("don't have player component");
		}
	}
}
