using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PowerUp : ScriptableObject{

	public string PowerUpName;
	[TextArea]public string PowerUpDescription;
	public Sprite PowerUpIconImage;
	public Sprite PowerUpContentImage;

	public virtual void Activate (Player player){}
	public virtual void Activate (Player player, int amount){}
	public virtual void Activate (Player originalPlayer, Player targetPlayer){}
	public virtual void Activate (Player player, Tile tile){}
	public virtual void Activate (Player originalPlayer, Player targetPlayer, GemType originalGem, GemType targetGem){}
}

