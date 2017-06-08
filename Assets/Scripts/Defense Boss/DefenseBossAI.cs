using UnityEngine;
using System.Collections;

public class DefenseBossAI : MonoBehaviour {

    public GameObject homingMissile;
	public GameObject iceSpike;
    public GameObject player;

    public int movementTime;
    public int iceAttackTime;
    public int homingAttackTime;

    private bool playerRight;
    private bool facingRight;
    private Rigidbody2D bossRB;

	void Start () {
        bossRB = this.GetComponent<Rigidbody2D>();
	}

    void Update () {

        if (player.transform.position.x < this.transform.position.x)
            playerRight = false;
        else    
            playerRight = true;

        if (iceAttackTime <= 0)
        {
            StartCoroutine("IceAttack");
            iceAttackTime = Random.Range(200, 250);
        }

        if (homingAttackTime <= 0 && iceAttackTime > 100)
        {
            StartCoroutine("HomingMissileAttack");
            homingAttackTime = Random.Range(500, 600);
        }
            

        isFacingRight();

        movementTime--;
        iceAttackTime--;
        homingAttackTime--;

        if (this.transform.position.x < -14)
            bossRB.AddForce(new Vector2(20000, 0), ForceMode2D.Impulse);
        if (this.transform.position.x > 16)
            bossRB.AddForce(new Vector2(-20000, 0), ForceMode2D.Impulse);

        if (movementTime <= 0 && !facingRight && Mathf.Abs(player.transform.position.x - this.transform.position.x) < 5)
        {
            bossRB.AddForce(new Vector2(10000, 0), ForceMode2D.Impulse);
            movementTime =  Random.Range(100, 200);
        }
        else if (movementTime <= 0 && facingRight && Mathf.Abs(player.transform.position.x - this.transform.position.x) < 5)
        {
            bossRB.AddForce(new Vector2(-10000, 0), ForceMode2D.Impulse);
            movementTime = 180;
            movementTime = Random.Range(100, 200);
        }
            

		if (Input.GetKeyDown (KeyCode.B))
			StartCoroutine ("HomingMissileAttack");

		if (Input.GetKeyDown (KeyCode.I)) {
			StartCoroutine ("IceAttack");
		}

    }

    void isFacingRight()
    {
        if (playerRight && !facingRight)
        {
            Vector3 flipScale = transform.localScale;
            flipScale.x *= -1;
            transform.localScale = flipScale;
            facingRight = true;
        }
        else if (!playerRight && facingRight)
        {
            Vector3 flipScale = transform.localScale;
            flipScale.x *= -1;
            transform.localScale = flipScale;
            facingRight = false;
        }
    }

	IEnumerator HomingMissileAttack(){
            Instantiate(homingMissile, this.transform.position + new Vector3(0, 2, 0), new Quaternion(0, 0, 0, 0));
            yield return new WaitForSeconds(.3f);
            Instantiate(homingMissile, this.transform.position + new Vector3(0, 2, 0), new Quaternion(0, 0, 0, 0));
            yield return new WaitForSeconds(.3f);
            Instantiate(homingMissile, this.transform.position + new Vector3(0, 2, 0), new Quaternion(0, 0, 0, 0));		
	}

	IEnumerator IceAttack()
	{
        bossRB.AddForce(new Vector2(0, 20000), ForceMode2D.Impulse);
        yield return new WaitForSeconds(1f);
        Instantiate (iceSpike, this.transform.position + new Vector3 (-1.7f, -4, 0), new Quaternion (0, 0, 0, 0));
        Instantiate (iceSpike, this.transform.position + new Vector3(1.7f, -4, 0), new Quaternion(0, 0, 0, 0));
        yield return new WaitForSeconds (.3f);
		Instantiate (iceSpike, this.transform.position + new Vector3 (-3.2f, -4, 0), new Quaternion (0, 0, 0, 0));
        Instantiate (iceSpike, this.transform.position + new Vector3(3.2f, -4, 0), new Quaternion(0, 0, 0, 0));
        yield return new WaitForSeconds (.3f);
		Instantiate (iceSpike, this.transform.position + new Vector3 (-4.7f, -4, 0), new Quaternion (0, 0, 0, 0));
        Instantiate (iceSpike, this.transform.position + new Vector3(4.7f, -4, 0), new Quaternion(0, 0, 0, 0));
    }

}
