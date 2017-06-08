using UnityEngine;
using System.Collections;

public class HomingMissileScript : MonoBehaviour {

    public float speed;
    public float rotationSpeed;
    public GameObject target;

    private bool playerRight;

    Rigidbody2D rb;
	
	void Start () {
        target = GameObject.FindGameObjectWithTag ("Player");
        rb = GetComponent<Rigidbody2D> ();

		if (transform.position.x <= target.transform.position.x) {
			playerRight = true;
			transform.localScale = transform.localScale * -1;
			this.transform.rotation = Quaternion.Euler (0, 0, 135);
			rb.AddForce (new Vector2 (-10, 10), ForceMode2D.Impulse);
		} else {
			playerRight = false;

			this.transform.rotation = Quaternion.Euler (0, 0, 225);
			rb.AddForce (new Vector2 (10, 10), ForceMode2D.Impulse);
		}



    }
	
	
	void FixedUpdate () {

        Vector2 forward = transform.position - target.transform.position;

        forward.Normalize();

        float value = Vector3.Cross(forward, transform.right).z;

		if (playerRight) {
			rb.angularVelocity = rotationSpeed * value;
			rb.velocity = transform.right * speed;
            
		} else if (!playerRight) {
			rb.angularVelocity = rotationSpeed * -value;
			rb.velocity = -transform.right * speed;
		}
	}

    void OnCollisionEnter2D(Collision2D col)
    {
		if (col.gameObject.tag == "Enemy")
			Physics2D.IgnoreCollision (this.GetComponent<Collider2D> (), col.collider);
        if (col.gameObject.tag == "Player" || col.collider.tag == "Ground" || col.collider.tag == "Ceiling" || col.collider.tag == "Wall")
            Destroy(this.gameObject);
    }
}
