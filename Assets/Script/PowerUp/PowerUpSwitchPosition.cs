using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(menuName = "Power Up/Switch Position")]
public class PowerUpSwitchPosition : PowerUp {

	public override void Activate(Player originalPlayer, Player targetPlayer){
		Vector3 t1 = originalPlayer.gameObject.transform.position;
		Vector3 t2 = targetPlayer.gameObject.transform.position;

		originalPlayer.gameObject.transform.position = t2;
		targetPlayer.gameObject.transform.position = t1;
	}
}
