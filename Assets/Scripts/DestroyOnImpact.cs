using UnityEngine;
using System.Collections;

public class DestroyOnImpact : MonoBehaviour {

    public GameObject bombExplosion;

	void OnCollisionEnter2D(Collision2D other)
    {
        if (other.collider.tag == "Ground")
        {
            Instantiate(bombExplosion, new Vector2(transform.position.x, transform.position.y), Quaternion.identity);
            Destroy(gameObject);
        }
        else
        {

        }
    }
}
