using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

//[RequireComponent(typeof(Collider))]
public class ChooseArrow : MonoBehaviour, IPointerDownHandler/*, IPointerUpHandler, IPointerExitHandler */{

	[Header("Way")]
	public WayType ArrowWay;
	[Header("Color")]
	public Image image;
	public Color OriginalColor;
	public Color HighlightColor;

	void Start(){
		OriginalColor = image.color;
	}
	/*
	void OnMouseDown(){
		image.color = HighlightColor;
		Debug.Log ("DOWN!!! : " + this.gameObject.name);
		Player p = GetComponentInParent<Player> ();
		p.CurrentWay = ArrowWay;
		p.InactiveWayArrows ();
		p._SelectedWay = true;
	}

	void OnMouseEnter(){
		Debug.Log ("ENTERED!!! : " + this.gameObject.name);
	}

	void OnMouseUp(){
		image.color = OriginalColor;
	}*/

	void OnEnable(){
		image.color = OriginalColor;
	}

	public void OnPointerDown (PointerEventData eventData){
		image.color = HighlightColor;
		Player p = GetComponentInParent<Player> ();
		p.CurrentWay = ArrowWay;
		p._SelectedWay = true;
		SetAllOriginalColor ();
		p.InactiveWayArrows ();
	}
	/*
	public void OnPointerUp (PointerEventData eventData){
		Debug.Log ("UPPPP!!! : " + this.gameObject.name);
		image.color = OriginalColor;
	}

	public void OnPointerExit (PointerEventData eventData){
		Debug.Log ("EXITTTT!!! : " + this.gameObject.name);
		image.color = OriginalColor;
	}*/

	public static void SetAllOriginalColor(){
		ChooseArrow[] arrows = GameObject.FindObjectsOfType<ChooseArrow> ();

		for (int i = 0; i < arrows.Length; i++) {
			arrows [i].image.color = arrows [i].OriginalColor;
		}
	}
}
