using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour {

	Vector3 offset;

	void Start () {
		offset = this.transform.position;
	}

	void Update () {
		if(TurnManager.instance.CurrentPlayer != null){
			this.transform.position = Vector3.Lerp (this.transform.position, offset + TurnManager.instance.CurrentPlayer.transform.position, 10.0f*Time.deltaTime);			
		}
	}
}
