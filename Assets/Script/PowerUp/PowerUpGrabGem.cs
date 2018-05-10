using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(menuName = "Power Up/Grab Gem")]
public class PowerUpGrabGem : PowerUp {

	public override void Activate(Player originalPlayer, Player targetPlayer, GemType originalGem, GemType targetGem){
		switch(originalGem){
		case GemType.Red:
			originalPlayer.GemRed.Amount--;
			targetPlayer.GemRed.Amount++;
			break;
		case GemType.Green:
			originalPlayer.GemGreen.Amount--;
			targetPlayer.GemGreen.Amount++;
			break;
		case GemType.Blue:
			originalPlayer.GemBlue.Amount--;
			targetPlayer.GemBlue.Amount++;
			break;
		case GemType.Yellow:
			originalPlayer.GemYellow.Amount--;
			targetPlayer.GemYellow.Amount++;
			break;
		case GemType.White:
			originalPlayer.GemWhite.Amount--;
			targetPlayer.GemWhite.Amount++;
			break;
		}

		switch(targetGem){
		case GemType.Red:
			originalPlayer.GemRed.Amount++;
			targetPlayer.GemRed.Amount--;
			break;
		case GemType.Green:
			originalPlayer.GemGreen.Amount++;
			targetPlayer.GemGreen.Amount--;
			break;
		case GemType.Blue:
			originalPlayer.GemBlue.Amount++;
			targetPlayer.GemBlue.Amount--;
			break;
		case GemType.Yellow:
			originalPlayer.GemYellow.Amount++;
			targetPlayer.GemYellow.Amount--;
			break;
		case GemType.White:
			originalPlayer.GemWhite.Amount++;
			targetPlayer.GemWhite.Amount--;
			break;
		}
	}
}
