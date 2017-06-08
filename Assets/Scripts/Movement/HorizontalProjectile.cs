using UnityEngine;
using System.Collections;

public class HorizontalProjectile : MonoBehaviour {
private Rigidbody2D rig2d;
	float h = 0;
	float v = 0;

	public int DamageValue;
	// Use this for initialization
	void Start () {
		DamageValue = 5;
		rig2d = GetComponent<Rigidbody2D>();
		Destroy(gameObject, 1f);
	}

	void Awake () {
		bool checkRight = GameObject.Find("Player").GetComponent<PlayerMovement>().isFacingRight;
		bool checkDiagUp = GameObject.Find("Player").GetComponent<PlayerMovement>().isDiagUp;
		bool checkDiagDown = GameObject.Find("Player").GetComponent<PlayerMovement>().isDiagDown;
		bool checkUp = GameObject.Find("Player").GetComponent<PlayerMovement>().isFacingUp;
		if (checkRight)
			h = 1f;
		else
			h = -1f;
		if (checkDiagUp)
			v = 1;
		else if (checkDiagDown)
			v = -.8f;
		else
			v = 0;
		if (checkUp) {
			v = 1;
			h = 0;
		}
}
	
	// Update is called once per frame
	void Update () {
		rig2d.velocity = new Vector2(20 * h, 24 * v);
	}

	void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "Enemy")
        {
            Destroy(gameObject);
        }
        if(other.tag == "Ground" || other.tag == "Wall" || other.tag == "Ceiling") {
        	Destroy(gameObject);
        }
		if (other.tag == "EnemyWeapon")
			Destroy (gameObject);
    }

 
}
