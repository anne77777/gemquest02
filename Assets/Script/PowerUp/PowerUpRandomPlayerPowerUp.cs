using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(menuName = "Power Up/Random Player PowerUp")]
public class PowerUpRandomPlayerPowerUp : PowerUp {

	public override void Activate(Player player){
		int len = player.PowerUps.Count;
		Object[] pus = Resources.LoadAll ("PowerUp", typeof(PowerUp));

		player.PowerUps.Clear ();
		for (int i = 0; i < len; i++) {
			PowerUp pu = pus [Random.Range (0, pus.Length)] as PowerUp;
			player.PowerUps.Add (pu);
		}
	}
}

