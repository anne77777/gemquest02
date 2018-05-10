using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(menuName = "Power Up/Dice Max")]
public class PowerUpDiceMax : PowerUp {

	public override void Activate(Player player, int amount = 6){
		player._DicePowerUpValue = amount;
		player._UseDicePowerUp = true;
	}
}
