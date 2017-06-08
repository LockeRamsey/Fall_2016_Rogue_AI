using UnityEngine;
using System.Collections;

public class IceSpikeScript : MonoBehaviour {

	public float risingTime;
	public float destroyTime;

	void Start () {

	}

	void Update () {

		risingTime-=.5f;
		destroyTime--;

		if(risingTime >= 0)
			this.transform.position = this.transform.position + new Vector3 (0, 1.2f);

		if (destroyTime <= 0)
			Destroy (this.gameObject);

	}

	void OnCollisionEnter2D(Collision2D col)
	{
		if (col.gameObject.tag == "Ground" || col.gameObject.tag == "Wall")
			Physics2D.IgnoreCollision (this.GetComponent<Collider2D> (), col.collider);
	}
}
