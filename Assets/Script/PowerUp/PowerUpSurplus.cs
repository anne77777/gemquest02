using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(menuName = "Power Up/Surplus")]
public class PowerUpSurplus : PowerUp {

	public override void Activate(Player originalPlayer, Player targetPlayer){
		int numRed = (targetPlayer.GemRed.Amount > 1) ? (targetPlayer.GemRed.Amount - 1) : 0;
		targetPlayer.GemRed.Amount -= numRed;
		originalPlayer.GemRed.Amount += numRed;

		int numGreen = (targetPlayer.GemGreen.Amount > 1) ? (targetPlayer.GemGreen.Amount - 1) : 0;
		targetPlayer.GemGreen.Amount -= numGreen;
		originalPlayer.GemGreen.Amount += numGreen;

		int numBlue = (targetPlayer.GemBlue.Amount > 1) ? (targetPlayer.GemBlue.Amount - 1) : 0;
		targetPlayer.GemBlue.Amount -= numBlue;
		originalPlayer.GemBlue.Amount += numBlue;

		int numYellow = (targetPlayer.GemYellow.Amount > 1) ? (targetPlayer.GemYellow.Amount - 1) : 0;
		targetPlayer.GemYellow.Amount -= numYellow;
		originalPlayer.GemYellow.Amount += numYellow;

		int numWhite = (targetPlayer.GemWhite.Amount > 1) ? (targetPlayer.GemWhite.Amount - 1) : 0;
		targetPlayer.GemWhite.Amount -= numWhite;
		originalPlayer.GemWhite.Amount += numWhite;
	}
}
