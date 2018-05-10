using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(menuName = "Power Up/Back To Start Tile")]
public class PowerUpBackToStartTile : PowerUp {

	public override void Activate(Player player){
		player.gameObject.transform.position = player.StartTile.transform.position;
		player.CurrentTile = player.StartTile;
	}
}
