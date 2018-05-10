using UnityEngine;
using Photon;

public class InactivateForNonLocal : Photon.MonoBehaviour {

	PhotonView pv;

	void Start () {
		pv = GetComponentInParent<PhotonView> ();
		Debug.Log ("isMine : " + pv.isMine);
		if(!pv.isMine){
			this.gameObject.SetActive (false);
		}
	}
}
