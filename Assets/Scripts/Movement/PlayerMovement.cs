using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour {
public int PlayerNumber = 1;
	Transform enemy;

	private Rigidbody2D rig2d;
	public GameObject samusProj;
	public GameObject waveProj;
	Animator anim;

	float h, v;

	public float maxSpeed = 16;
	bool crouch;

	public float maxJump = 15;
	float jumpPowr;
	int jumpCount;

	bool falling;
	public bool onGround;
	bool canBoost = true;
	public bool isFacingRight = false;
	public bool isDiagUp = false;
	public bool isDiagDown = false;
	public bool isFacingUp = false;
	public bool isShine = false;
	public bool isDash = false;
	public bool onWall = false;
	bool canWallJump;
	public int currentWeapon = 0;
	public float blastVar = 500;
	float orienX = 0f, orienY = 0f;
	float blastX = 0f, blastY = 0f;

	public float boostCooldown; //made this public so that you can adjust in Inspector
	float boostDuration;
	public int speedBoost;

    public bool canMove;
    public bool canShoot;
    public bool canSwitch; //Locke added this
	//ricky added this
	public Image pistol;
	public Image shotgun;
	public Image sniper;
	public Image pistolSprite;
	public Image shotgunSprite;
	public Image sniperSprite;

	public bool pickedUpWeapon = false;
	public bool shotgunPickedUpWeapon = false;
	public bool sniperPickedUpWeapon = false;
	public GameObject gameControl;
	GameControl playerSave;
	//-------------------------------


	void Awake(){
        canShoot = true;
        pistol.color = new Color (255f/255f, 255f/255f, 255f/255f, 136f/255f); 
        shotgun.color = new Color (255f / 255f, 255f / 255f, 255f / 255f, 136f / 255f);
        sniper.color = new Color (255f / 255f, 255f / 255f, 255f / 255f, 136f / 255f);
        pistolSprite.color = new Color (255f / 255f, 255f / 255f, 255f / 255f, 136f / 255f);
        shotgunSprite.color = new Color (255f/255f, 255f/255f, 255f/255f, 136f/255f);
        sniperSprite.color = new Color (255f/255f, 255f/255f, 255f/255f, 136f/255f);
		gameControl = GameObject.FindGameObjectWithTag ("GameController");
		playerSave = gameControl.GetComponent<GameControl> ();
		pickedUpWeapon = playerSave.pickedUpPistol;
		shotgunPickedUpWeapon = playerSave.pickedUpShotgun;
		sniperPickedUpWeapon = playerSave.pickedUpSniper;
		currentWeapon = playerSave.currentWeapon;
    }
	// Use this for initialization
	void Start () {
		rig2d = GetComponent<Rigidbody2D>();
		anim = GetComponentInChildren<Animator>();
        canMove = true;
        canSwitch = true; //Locke added this
        canWallJump = true;
        jumpPowr = maxJump;

	}
	
	void Update () {

		playerSave.pickedUpPistol = pickedUpWeapon;
		playerSave.pickedUpShotgun = shotgunPickedUpWeapon;
		playerSave.pickedUpSniper = sniperPickedUpWeapon;
		playerSave.currentWeapon = currentWeapon;

		if (Input.GetKeyDown(KeyCode.RightShift)) {
			canMove = true;
			canShoot = true;
		}

        if (!canMove)
        {
        	h = 0;
        	UpdateAnimator();
            
            return;
        }

        //Checks and Updates
        UpdateAnimator();
		OnGroundCheck();
		RightFaceCheck();
        OrientationCheck();

        //Reset Shining Status
		if (Input.GetKeyUp(KeyCode.Z)) {
			isShine = false;
		}

		//Jumping
		if (Input.GetKeyDown(KeyCode.Space)) {
			if (onWall && !onGround) {
				if (canWallJump) {
					rig2d.velocity = new Vector2(rig2d.velocity.x, jumpPowr);
					canWallJump = false;
				} else if (!canWallJump) {}
			} else if (jumpCount < 2) {
				rig2d.velocity = new Vector2(rig2d.velocity.x, jumpPowr);
				jumpCount++;
			}
		}


        //Crouching
        crouch = (v < -0.1f && onGround);

        //Air Shining
        if (Input.GetKey(KeyCode.Z) && !onGround) {
        	isShine = true;
        	rig2d.velocity = Vector3.zero;
        }

        //Weapon Switching
        if(canSwitch == true) //Locke added this
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                currentWeapon -= 1;
                WeaponCheck();
            }
            if (Input.GetKeyDown(KeyCode.E))
            {
                currentWeapon += 1;
                WeaponCheck();
            }
			if (currentWeapon == 0 && pickedUpWeapon == true) {
				pistol.color = new Color (1f, 1f, 1f, 1f);
				pistolSprite.color = new Color (1f, 1f, 1f, 1f);
				shotgun.color = new Color (1f, 1f, 1f, 136f/255f);
				shotgunSprite.color = new Color (1f, 1f, 1f, 136f/255f);
				sniper.color = new Color (1f, 1f, 1f, 136f/255f);
				sniperSprite.color = new Color (1f, 1f, 1f, 136f/255f);
			}
			if (currentWeapon == 1 && shotgunPickedUpWeapon == true) {
				shotgun.color = new Color (1f, 1f, 1f, 1f);
				shotgunSprite.color = new Color (1f, 1f, 1f, 1f);
				sniper.color = new Color (1f, 1f, 1f, 136f/255f);
				sniperSprite.color = new Color (1f, 1f, 1f, 136f/255f);
				pistol.color = new Color (1f, 1f, 1f, 136f/255f);
				pistolSprite.color = new Color (1f, 1f, 1f, 136f/255f);
			}
			if (currentWeapon == 2 && sniperPickedUpWeapon == true) {
				sniper.color = new Color (1f, 1f, 1f, 1f);
				sniperSprite.color = new Color (1f, 1f, 1f, 1f);
				pistol.color = new Color (1f, 1f, 1f, 136f/255f);
				pistolSprite.color = new Color (1f, 1f, 1f, 136f/255f);
				shotgun.color = new Color (1f, 1f, 1f, 136f/255f);
				shotgunSprite.color = new Color (1f, 1f, 1f, 136f/255f);
			}
        }
        

		//Shooting
		if (Input.GetKeyDown(KeyCode.J)) {
			FireSamus(currentWeapon);
		}
		
		//Dash
		if (Input.GetKeyDown(KeyCode.LeftShift) && canBoost) {
			isDash = true;
            rig2d.velocity = Vector2.zero; //making sure the velocity doesn't affect the dash. Might be redundant code though
            StartCoroutine(Boost(0.25f)); //made the duration shorter
		}

	}

	void FixedUpdate ()
    {
		if (Input.GetKeyDown(KeyCode.Q)) {
			canMove = true;
			canShoot = true;
		}
        if (!canMove)
        {
            return;
        }

        h = Input.GetAxis("Horizontal");
        v = Input.GetAxis("Vertical");


		if (h > -1.1 && h < 1.1) {
			rig2d.velocity = new Vector2(h * maxSpeed, rig2d.velocity.y);
		}

		
		if (!onGround) {
			falling = true;
		}

		if (isFacingUp && onGround)
			rig2d.velocity = new Vector2(0, rig2d.velocity.y);

		if (crouch)
			rig2d.velocity = Vector3.zero;
	}

	void OnGroundCheck () {
		if (!onGround) {
			rig2d.gravityScale = 5;
		} else {
			rig2d.gravityScale = 3;
		}
	}

	void UpdateAnimator () {
		anim.SetBool("Crouching", crouch);
		anim.SetBool("OnGround", this.onGround);
		anim.SetBool("Dashing", isDash);
		anim.SetBool("DiagUp", isDiagUp);
		anim.SetBool("DiagDown", isDiagDown);
		anim.SetBool("AimUp", isFacingUp);
		anim.SetBool("Shining", isShine);
		anim.SetBool("Falling", this.falling);
		anim.SetBool("OnWall", onWall);
		anim.SetFloat("Movement", Mathf.Abs(h));
	}

	void OnCollisionEnter2D (Collision2D thing) {
		if (thing.collider.tag == "Ground") {
			jumpCount = 0;
			onGround = true;

			falling = false;
		}
		if (thing.collider.tag == "Wall") {								//Wall Jump
			onWall = true;
		}
	}

	void OnCollisionExit2D (Collision2D thing) {
		if (thing.collider.tag == "Ground") {
			onGround = false;
		}
		if (thing.collider.tag == "Wall") {
			onWall = false;
			canWallJump = true;
			jumpPowr = maxJump;
		}
	}

	void RightFaceCheck () {
		if ((h < 0 && isFacingRight) || (h > 0 && !isFacingRight)) {
			isFacingRight = !isFacingRight;
			Vector3 flipScale = transform.localScale;
			flipScale.x *= -1;
			transform.localScale = flipScale;
		}
	}

	void OrientationCheck () {
        if (Input.GetKey(KeyCode.K))
        {                               //Diagonally Up
            isDiagUp = true;
            isDiagDown = false;
        }
        else
            isDiagUp = false;

        if (Input.GetKey(KeyCode.L))
        {                               //Diagonally Down
            isDiagUp = false;
            isDiagDown = true;
        }
        else
            isDiagDown = false;

        if (v > 0.1f)
        {
            isFacingUp = true;
            isDiagUp = false;
            isDiagDown = false;
        }
        else if (v <= 0f)
        {
            isFacingUp = false;
        }

	}

	void WeaponCheck () {
        if (currentWeapon < 0)
        	currentWeapon = 2;
        if (currentWeapon > 2)
        	currentWeapon = 0;
	}

	void FireSamus (int currWep) {

        if (!canShoot)
        {
            return;
        }

        if (!crouch) {															//Standing + Jumping Aiming
	        if (isFacingRight)
	        	orienX = 1f;
	        else
	        	orienX = -1f;

	        if (isDiagUp)
	        	orienY = 1.4f;
	        if (isDiagDown)
	        	orienY = -.3f;
	        if (isFacingUp) {
	        	orienX = 0f;
	        	orienY = 1.5f;
	        }
	        if (!isDiagUp && !isDiagDown && !isFacingUp)
	        	orienY = .2f;
	    } else {																//Crouching Aiming
	        if (isFacingRight)
	        	orienX = 1f;
	        else
	        	orienX = -1f;

	        if (isDiagUp)
	        	orienY = .8f;
	        if (isDiagDown)
	        	orienY = -.8f;
	        if (!isDiagUp && !isDiagDown)
	        	orienY = -.3f;
	    }

	    switch (currWep) {
	    	case 0:
				Rigidbody2D samusInstance = Instantiate(samusProj, new Vector2(transform.position.x + orienX, transform.position.y + orienY), Quaternion.identity).GetComponent<Rigidbody2D>();
				break;
			case 1:
				rig2d.velocity = new Vector2(rig2d.velocity.x * 0.5f, rig2d.velocity.y);
				StartCoroutine("BlastBack");
				Rigidbody2D waveInstance = Instantiate(waveProj, new Vector2(transform.position.x + orienX, transform.position.y + orienY), Quaternion.identity).GetComponent<Rigidbody2D>();
				break;
			case 2:
				break;
		}
	}

	IEnumerator BlastBack () {
		float blastbackTime = 30f;

			if (isFacingRight)
	        	blastX = -blastVar;
	        else
	        	blastX = blastVar;

	        if (isDiagUp)
	        	blastY = -blastVar;
	        if (isDiagDown)
	        	blastY = blastVar;
	        if (!isDiagUp && !isDiagDown)
	        	blastY = 100;

	    canMove = false;
	    
		if (blastbackTime > 0) {
			blastbackTime -= Time.deltaTime;

			rig2d.AddForce(new Vector2(blastX, blastY));
		}

		yield return new WaitForSeconds(0.3f);
		canMove = true;
	}

	IEnumerator Boost(float boostDuration)
	{
		float time = 0;
		canBoost = false;
        canMove = false;        //Player was able to move during the dash at first. Adding this line of code prevents that
        rig2d.gravityScale = 0; //Player was also slowly descending while dashing if done in the air. This ensures that the player will no fall in the air when dashing
		while (boostDuration > time)
		{
			time += Time.deltaTime;
			//GetComponent<Rigidbody2D>().velocity = boostSpeed;

			if (isFacingRight)
				rig2d.AddForce(new Vector2(maxSpeed * speedBoost, 0));
			else if (!isFacingRight)
				rig2d.AddForce(new Vector2(-maxSpeed * speedBoost, 0));
            
			yield return 1;
		}

		
        rig2d.velocity = Vector2.zero; //momentum from the dash should not carry on once the dash is done
        rig2d.gravityScale = 5;        //gravity set back to normal
        canMove = true; 
		isDash = false;
        yield return new WaitForSeconds(boostCooldown); //cooldown wasn't being used before. Now it is.
        canBoost = true;
    }
}
