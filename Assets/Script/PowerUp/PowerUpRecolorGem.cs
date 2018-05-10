using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(menuName = "Power Up/Recolor Gem")]
public class PowerUpRecolorGem : PowerUp {

	public override void Activate(Player originalPlayer, Player targetPlayer, GemType originalGem, GemType targetGem){
		targetPlayer = originalPlayer;

		switch(originalGem){
		case GemType.Red:
			originalPlayer.GemRed.Amount--;
			break;
		case GemType.Green:
			originalPlayer.GemGreen.Amount--;
			break;
		case GemType.Blue:
			originalPlayer.GemBlue.Amount--;
			break;
		case GemType.Yellow:
			originalPlayer.GemYellow.Amount--;
			break;
		case GemType.White:
			originalPlayer.GemWhite.Amount--;
			break;
		}

		switch(targetGem){
		case GemType.Red:
			targetPlayer.GemRed.Amount++;
			break;
		case GemType.Green:
			targetPlayer.GemGreen.Amount++;
			break;
		case GemType.Blue:
			targetPlayer.GemBlue.Amount++;
			break;
		case GemType.Yellow:
			targetPlayer.GemYellow.Amount++;
			break;
		case GemType.White:
			targetPlayer.GemWhite.Amount++;
			break;
		}
	}
}
