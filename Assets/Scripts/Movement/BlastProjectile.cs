using UnityEngine;
using System.Collections;

public class BlastProjectile : MonoBehaviour {


	public int DamageValue;
	// Use this for initialization
	void Start () {
		Destroy(gameObject, .4f);
		DamageValue = 20;

		bool checkRight = GameObject.Find("Player").GetComponent<PlayerMovement>().isFacingRight;
		bool checkDiagUp = GameObject.Find("Player").GetComponent<PlayerMovement>().isDiagUp;
		bool checkDiagDown = GameObject.Find("Player").GetComponent<PlayerMovement>().isDiagDown;
		bool checkUp = GameObject.Find("Player").GetComponent<PlayerMovement>().isFacingUp;

		if (checkDiagUp) {
			if (checkRight)
				transform.Rotate(new Vector3(0, 0, -135));
			if (!checkRight)
				transform.Rotate(new Vector3(0, 0, -45));
		}
		else if (checkDiagDown) {
			if (checkRight)
				transform.Rotate(new Vector3(0, 0, 135));
			if (!checkRight)
				transform.Rotate(new Vector3(0, 0, 45));
		}
		else {
			if (checkRight)
				transform.Rotate(new Vector3(0, 0, 180));
			if (!checkRight)
				transform.Rotate(new Vector3(0, 0, 0));
		}
		if (checkUp) {
			transform.Rotate(new Vector3(0, 0, 90));
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
