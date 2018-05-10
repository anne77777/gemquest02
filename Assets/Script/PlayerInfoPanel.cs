using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInfoPanel : MonoBehaviour {

	[Header("Info Panel")]
	public Player player;
	[Space]
	public Transform DisplayPowerUpHolder;
	[Space]
	public Text amountRed;
	public Text amountGreen;
	public Text amountYellow;
	public Text amountBlue;
	public Text amountWhite;

	public void UpdatePlayerInfoPanel(){
		if(player == null){
			Debug.Log ("No player here!!!");
		}

		for (int i = 0; i < player.PowerUps.Count ; i++) {
			if(player == null){
				continue;
			}
			DisplayPowerUpHolder.GetChild (i).GetComponentInChildren<DisplayPowerUp> ().ShowPowerUpInfo(player.PowerUps[i]);
		}

		amountRed.text= ""+ player.GemRed.Amount;
		amountGreen.text = ""+ player.GemGreen.Amount;
		amountYellow.text = ""+ player.GemYellow.Amount;
		amountBlue.text = ""+ player.GemBlue.Amount;
		amountWhite.text = ""+ player.GemWhite.Amount;
	}
}
