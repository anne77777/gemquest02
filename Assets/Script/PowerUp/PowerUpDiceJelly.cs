using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(menuName = "Power Up/Dice Jelly")]
public class PowerUpDiceJelly : PowerUp {

	public override void Activate(Player player, int amount = 6){
		player._DicePowerUpValue = amount;
		player._UseDicePowerUp = true;
	}
}
