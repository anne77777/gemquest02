using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(menuName = "Power Up/Reverse Direction")]
public class PowerUpReverseDirection : PowerUp {

	public override void Activate(Player player){
		player.CurrentWay = player.OriginalWay;
	}
}

