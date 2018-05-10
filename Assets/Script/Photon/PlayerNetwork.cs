using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerNetwork : Photon.MonoBehaviour {
	
	[SerializeField]private Player[] players;
	private PhotonView photonView;

	bool initialized = false;

	Vector3 realPos = Vector3.zero;
	Quaternion realRot = Quaternion.identity;
	//float realSpeed;
	Animator ani;

	void Start(){

		photonView = GetComponent<PhotonView> ();
		ani = GetComponent<Animator> ();

		Initialize ();
	}
	/*
	void Update(){
		//Initialize ();
		if(!initialized){
			return;
		}

		if(!photonView.isMine){
			transform.position = Vector3.Lerp (transform.position, realPos , 1.5f);
			transform.rotation = Quaternion.Lerp (transform.rotation, realRot, 0.1f);
			//Mathf.Lerp(ani.GetFloat("Walk"), realSpeed, 0.1f);
			return;
		}
	}*/

	void Initialize(){
		if (photonView.isMine) {
			Debug.Log ("photonView.isMine");
		} 
	}

	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info){
		Debug.Log ("OnPhotonSerializeView");
		if(stream.isWriting){
			Debug.Log ("stream.isWriting");
			//stream.SendNext (health);
			stream.SendNext (transform.position);
			stream.SendNext (transform.rotation);
			//stream.SendNext (ani.GetBool("Jump"));
			//stream.SendNext (ani.GetFloat("Walk"));
		}else if(stream.isReading){
			Debug.Log ("stream.isReading");
			//health = (int)stream.ReceiveNext (); // the later is an object
			realPos = (Vector3)stream.ReceiveNext();
			realRot = (Quaternion)stream.ReceiveNext();
			//ani.SetFloat ("Jump", (bool)stream.ReceiveNext());
			//ani.SetFloat ("Walk", (float)stream.ReceiveNext());
		}
	}
}
