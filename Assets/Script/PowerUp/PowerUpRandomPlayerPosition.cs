using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(menuName = "Power Up/Random Player Position")]
public class PowerUpRandomPlayerPosition : PowerUp {

	public override void Activate(Player player){
		Tile[] alltiles = GameObject.FindObjectsOfType<Tile> ();
		List<Tile> tiles = new List<Tile> ();
		for (int i = 0; i < alltiles.Length; i++) {
			if(alltiles[i].ThisTileType != TileType.GOAL &&alltiles[i].ThisTileType != TileType.ORIGIN){
				tiles.Add (alltiles[i]);
			}
		}

		int index = Random.Range (0, tiles.Count);
		if(tiles[index].ThisTileType == TileType.PORTAL){
			player._PortalFirst = true;
			player._PortalSecond = false;
		}

		player.CurrentTile = tiles[index];
		player.gameObject.transform.position = tiles [index].gameObject.transform.position;
		player.OriginalWay = WayType.None;
	}
}

