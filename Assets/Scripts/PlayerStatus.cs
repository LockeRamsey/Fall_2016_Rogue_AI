 using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerStatus : MonoBehaviour {

    public GameObject homing;

    public GameObject player;

	public GameObject gameControl;

	GameControl playerSave;

    public Rigidbody2D playerRB;
    public bool playerInvulnerable = false;

    public bool enemyRight;

    public int timesHit;

    //ricky added this
	public int currentHealth = 100;
	public Slider healthSlider;
	public Image fill;
    
	//------------------
	private bool isDead;
	public Transform canvas;

	void Awake()
	{
		player.GetComponent<PlayerMovement> ().enabled = true;
		Time.timeScale = 1;
		playerRB = player.GetComponent<Rigidbody2D>();
		isDead = false;
		fill.GetComponent<Image> ().color = new Color (1, 0, 0, 1);
        timesHit = 0;
		gameControl = GameObject.FindGameObjectWithTag ("GameController");
		playerSave = gameControl.GetComponent<GameControl> ();
		currentHealth = playerSave.health;
	}

    void OnCollisionStay2D(Collision2D col)
    {
        if (col.gameObject.tag == "Enemy" && !playerInvulnerable)
        {
            currentHealth -= 5;
            playerRB.velocity = Vector2.zero;
            playerInvulnerable = true;
            StartCoroutine("Knockback");
        }
    }

	void OnCollisionEnter2D(Collision2D col)
	{
        int damage;        
		
        if(col.gameObject.tag == "EnemyWeapon" && !playerInvulnerable)
        {
            damage = col.gameObject.GetComponent<EnemyStatus>().damage;
            currentHealth -= damage;
            playerRB.velocity = Vector2.zero;
            playerInvulnerable = true;
            StartCoroutine("Knockback");
        }

        if(col.gameObject.tag == "JustBomb" && !playerInvulnerable) //This was added so that if a bomb hits the player, they still take damage but don't become invincible.
        {                                                           //This was done because you could get hit by a bomb's impact, but not be damaged by the more-damaging explosion afterward due to you being invincible.
            damage = col.gameObject.GetComponent<EnemyStatus>().damage;
            currentHealth -= damage;
            playerRB.velocity = Vector2.zero;
            StartCoroutine("NotInvincibleKnockback");
        }
	}

	void OnTriggerEnter2D(Collider2D col) {
		if (col.gameObject.tag == "Laser" && !playerInvulnerable)
		{
			currentHealth -= 20;
			playerRB.velocity = Vector2.zero;
            playerInvulnerable = true;
            StartCoroutine("Knockback");
		}

        if (col.gameObject.tag == "Bomb" && !playerInvulnerable)
        {
            currentHealth -= 10;
            playerRB.velocity = Vector2.zero;
            playerInvulnerable = true;
            StartCoroutine("Knockback");
        }

        if(col.gameObject.tag == "Fist" && !playerInvulnerable)
        {
            currentHealth -= 20;
            playerRB.velocity = Vector2.zero;
            playerInvulnerable = true;
            StartCoroutine("ExtendedKnockback");
        }
	}

    void Update() {
		if (isDead == false) {
			playerSave.health = currentHealth;
		} else {
			playerSave.health = 100;
			playerSave.pickedUpPistol = false;
			isDead = false;
		}

        if (Input.GetKeyDown(KeyCode.T))
            Debug.Log("Player Health: " + currentHealth);
                
		if (Physics2D.Raycast(this.transform.position, transform.right, 1f) || Physics2D.Raycast(this.transform.position, transform.right + new Vector3(0, 1), 1f) || Physics2D.Raycast(this.transform.position, transform.right + new Vector3(0, -1), 1f))
            enemyRight = true;
        Debug.DrawRay(this.transform.position, this.transform.forward + new Vector3(1, 0, 0), Color.red);
        Debug.DrawRay(this.transform.position, this.transform.forward + new Vector3(1, 1, 0), Color.red);
		Debug.DrawRay(this.transform.position, this.transform.forward + new Vector3(1, -1, 0), Color.red);
		if (Physics2D.Raycast(this.transform.position, -1 * transform.right, 1f) || Physics2D.Raycast(this.transform.position, -1 * transform.right + new Vector3(0, 1), 1f) || Physics2D.Raycast(this.transform.position, -1 * transform.right + new Vector3(0, -1), 1f))
            enemyRight = false;
        Debug.DrawRay(this.transform.position, this.transform.forward + new Vector3(-1, 0, 0), Color.cyan);
        Debug.DrawRay(this.transform.position, this.transform.forward + new Vector3(-1, 1, 0), Color.cyan);
		Debug.DrawRay(this.transform.position, this.transform.forward + new Vector3(-1, -1, 0), Color.cyan);

        if (currentHealth > 0)
            healthSlider.value = currentHealth;
        else {
			isDead = true;
			canvas.gameObject.SetActive (true);
			fill.GetComponent<Image> ().color = new Color (1, 0, 0, 0);
			Time.timeScale = .3f;
			player.GetComponent<PlayerMovement> ().enabled = false;
		}
    }

    IEnumerator Knockback()
    {
        float knockbackTime = .50f;
        
        player.GetComponent<PlayerMovement>().canMove = false;
        playerRB.gravityScale = 5;

        if (knockbackTime > 0)
        {
            knockbackTime -= Time.deltaTime;
            if(enemyRight)
                playerRB.AddForce(new Vector2(-10, 10), ForceMode2D.Impulse);
            else if(!enemyRight)
				playerRB.AddForce(new Vector2(10, 10), ForceMode2D.Impulse);
        }

        yield return new WaitForSeconds(.30f);

        player.GetComponent<PlayerMovement>().canMove = true;

        for (int i = 0; i < 15; i++)
        {
            GetComponent<SpriteRenderer>().enabled = false;
            yield return new WaitForSeconds(0.016666667f);
            GetComponent<SpriteRenderer>().enabled = true;
            yield return new WaitForSeconds(0.016666667f);
        }

        playerInvulnerable = false;
        timesHit++;
    }

    IEnumerator NotInvincibleKnockback() //Specific co-routine for only applying knockback with no invincibility
    {
        float knockbackTime = .50f;

        player.GetComponent<PlayerMovement>().canMove = false;
        playerRB.gravityScale = 5;

        if (knockbackTime > 0)
        {
            knockbackTime -= Time.deltaTime;
            if (enemyRight)
                playerRB.AddForce(new Vector2(-300, 300));
            else if (!enemyRight)
                playerRB.AddForce(new Vector2(300, 300));
        }

        yield return new WaitForSeconds(.30f);

        player.GetComponent<PlayerMovement>().canMove = true;
    }

    IEnumerator ExtendedKnockback()
    {
        transform.rotation = Quaternion.Euler(0, 0, 90);
        transform.position = new Vector2(transform.position.x, transform.position.y - 0.75f);
        player.GetComponent<PlayerMovement>().enabled = false;
        playerRB.gravityScale = 50;

        yield return new WaitForSeconds(0.25f);

        playerRB.isKinematic = true;

        for (int i = 0; i < 45; i++)
        {
            GetComponent<SpriteRenderer>().enabled = false;
            yield return new WaitForSeconds(0.016666667f);
            GetComponent<SpriteRenderer>().enabled = true;
            yield return new WaitForSeconds(0.016666667f);
        }
        playerRB.isKinematic = false;
        transform.rotation = Quaternion.Euler(0, 0, 0);
        transform.position = new Vector2(transform.position.x, transform.position.y + 0.75f);
        player.GetComponent<PlayerMovement>().enabled = true;
        playerInvulnerable = false;
        timesHit++;
    }
}