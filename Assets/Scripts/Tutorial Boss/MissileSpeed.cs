using UnityEngine;
using System.Collections;

public class MissileSpeed : MonoBehaviour {

    public float speed;

    private Rigidbody2D myrigidbody2D;

    void Start ()
    {
        myrigidbody2D = GetComponent<Rigidbody2D>();
    }
	
	void Update ()
    {
        myrigidbody2D.velocity = new Vector2(speed, myrigidbody2D.velocity.y);
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.collider.tag == "Player" || other.collider.tag == "Wall")
        {
            Destroy(gameObject);
        }
        else
        {

        }


    }
}
