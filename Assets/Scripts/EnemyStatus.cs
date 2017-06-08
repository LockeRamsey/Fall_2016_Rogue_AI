using UnityEngine;
using System.Collections;

public class EnemyStatus : MonoBehaviour {

    public int health;
    public int damage;
	public bool damageable;

	void Start () {
		
	}
	
	void Update () {

		if (health < 0)
			Destroy (this.gameObject);
	
	}

	void OnTriggerEnter2D(Collider2D col)
	{
		if (col.tag == "Bullet" && damageable) {
			health -= col.GetComponent<HorizontalProjectile>().DamageValue;
		}
        if (col.tag == "Bullet 2" && damageable)
        {
            health -= col.GetComponent<BlastProjectile>().DamageValue;
        }
    }
}
