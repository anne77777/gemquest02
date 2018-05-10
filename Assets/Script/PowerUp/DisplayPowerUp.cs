using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DisplayPowerUp : MonoBehaviour {

	[Header("Power Up")]
	public PowerUp powerup;

	[Header("Displaying UIs")]
	public Image PowerUpIcon;
	public Image PowerUpImage;
	public Text PowerUpName;
	public Text PowerUpDescription;
	public GameObject PowerUpButton;
	public GameObject PowerUpRemove;

	public void ShowPowerUpInfo(PowerUp powerup){
		this.powerup = powerup;

		if(powerup == null){
			return;
		}

		if(PowerUpIcon != null){
			PowerUpIcon.sprite = powerup.PowerUpIconImage;
		}

		if(PowerUpImage != null){
			PowerUpImage.sprite = powerup.PowerUpContentImage;
		}

		if(PowerUpName != null){
			PowerUpName.text = powerup.PowerUpName;
		}

		if(PowerUpDescription != null){
			PowerUpDescription.text = powerup.PowerUpDescription;
		}

		if(PowerUpButton != null){
			EventTrigger trigger = PowerUpButton.GetComponent<EventTrigger> ();
			EventTrigger.Entry entry = new EventTrigger.Entry ();
			entry.eventID = EventTriggerType.PointerDown;
			entry.callback.AddListener ((data) => {
				SetPlayerCurrentPowerUp (powerup);
			});
			trigger.triggers.Add(entry);
		}

		if(PowerUpRemove != null){
			EventTrigger trigger = PowerUpRemove.GetComponent<EventTrigger> ();
			EventTrigger.Entry entry = new EventTrigger.Entry ();
			entry.eventID = EventTriggerType.PointerDown;
			entry.callback.AddListener ((data) => {
				RemovePlayerThisPowerUp (powerup);
			});
			trigger.triggers.Add(entry);
		}
	}

	void SetPlayerCurrentPowerUp(PowerUp powerup){
		Player player = GetComponentInParent<Player> ();
		player.CurrentPowerUp = powerup;
		player.ExitUsingPowerUp ();
		player._WhetherUsePowerUp = true;
		Debug.Log ("Selected the power up : " + powerup.PowerUpName);
	}

	void RemovePlayerThisPowerUp(PowerUp powerup){
		Player player = GetComponentInParent<Player> ();
		//int index = player.PowerUps.IndexOf (powerup);
		player.PowerUps.Remove (powerup);
		player.EndSelectPowerUp ();
		player._RemovePowerUp = true;
		player.AddSelectPowerUp ();
		player._SelectingPowerUp = false;
	}
}
