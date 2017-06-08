using UnityEngine;
using System.Collections;

public class GameControl : MonoBehaviour {

	public static GameControl control;

	public int health = 0;
	public bool pickedUpPistol;
	public bool pickedUpShotgun;
	public bool pickedUpSniper;
	public int currentWeapon;

	// Use this for initialization
	void Awake () {
		if (control == null) {
			DontDestroyOnLoad (gameObject);
			control = this;
		} else if (control != this) {
			Destroy (gameObject);
		}
		playerPref ();

	}
	
	// Update is called once per frame
	void playerPref(){
		PlayerStatus hp = GameObject.FindGameObjectWithTag ("Player").GetComponent<PlayerStatus> ();
		PlayerMovement weapon = GameObject.FindGameObjectWithTag ("Player").GetComponent<PlayerMovement> ();
		health = hp.currentHealth;
		pickedUpPistol = weapon.pickedUpWeapon;
		pickedUpShotgun = weapon.shotgunPickedUpWeapon;
		pickedUpSniper = weapon.sniperPickedUpWeapon;
		currentWeapon = weapon.currentWeapon;
	}
}
