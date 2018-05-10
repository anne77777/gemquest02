using UnityEngine;

public enum GemType{
	Red,
	Blue,
	Green,
	Yellow,
	White
}

[System.Serializable]
public class Gem {

	public GemType Type;
	public int Amount;

	public Gem(GemType Type = GemType.Red, int Amount = 0){
		this.Type = Type;
		this.Amount = Amount;
	}
}