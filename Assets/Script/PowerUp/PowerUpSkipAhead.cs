using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(menuName = "Power Up/Skip Ahead")]
public class PowerUpSkipAhead : PowerUp {

	public override void Activate(Player originalPlayer, Player targetPlayer){
		if (targetPlayer.NextTile != null) {
			originalPlayer.gameObject.transform.position = targetPlayer.NextTile.transform.position;
			originalPlayer.CurrentTile = targetPlayer.NextTile;
		} else {
			Debug.Log ("targetplayer's next tile is unsettled!");
		}
	}
}
