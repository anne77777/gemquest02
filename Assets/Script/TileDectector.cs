using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class TileDectector : MonoBehaviour {

	Player player;

	void OnTriggerEnter(Collider other) {
		Tile tile = other.gameObject.GetComponentInParent<Tile> ();
		if (tile != null) {
			player = GetComponentInParent<Player> ();
			player.NextTile = tile;

			/*
			if (tile.ThisTileType != TileType.GOAL && tile.ThisTileType != TileType.ORIGIN) {
				player.NextTile = tile;
			} else {
				player.CurrentTile = player.NextTile;
			}*/
		}
	}
}
