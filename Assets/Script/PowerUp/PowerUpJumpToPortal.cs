using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(menuName = "Power Up/Jump To Portal")]
public class PowerUpJumpToPortal : PowerUp {

	public override void Activate(Player player, Tile tile){
		player.gameObject.transform.position = tile.gameObject.transform.position;
		player.CurrentTile = tile;
		player.CurrentWay = WayType.None;
		player._PortalFirst = true;
		player._PortalSecond = false;
	}
}

