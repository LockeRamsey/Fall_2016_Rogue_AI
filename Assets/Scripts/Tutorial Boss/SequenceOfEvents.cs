using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SequenceOfEvents : MonoBehaviour {

    public GameObject player;
    public GameObject startDialogueTrigger;
    public GameObject textBoxManager;
    public GameObject spaceGun;
    public GameObject Boss;
    public GameObject theCamera;
    public GameObject missile;
    public GameObject bomb;
    public GameObject ceiling1;
    public GameObject ceiling2;
    public GameObject ceiling3;
    public GameObject litWall;
    public GameObject ground;
    public GameObject extendedBackground;
    public GameObject PointA;
    public GameObject PointB;
    public GameObject BossShield;
    public GameObject Buttons;
    public GameObject leftButton;
    public GameObject rightButton;
    public GameObject Pillar;
    public GameObject fistWarning;
    public GameObject fistHitbox;
    public GameObject TractorBeam;
    public GameObject pillarWeakspot;
    public TextAsset theText;

    private Rigidbody2D playerRigidbody;
    public TextBoxManager textBox;
    public PlayerMovement playerMain;
    public DontHitMe hittingEnemy;
    public BoxCamera_Locke cameraScript;
    private LeftButton leftButtonScript;
    private RightButton rightButtonScript;
    private DamageFistPillar pillarDamaged;
    public PlayerStatus playersStatus;
    private bool testMoveLeft;
    private bool testMoveRight;
    private bool heFell;
    private bool movementTutorialComplete;
    private bool spawnOnly1Gun;
    public bool pickedUpGun;
    public bool bossMoved;
    private bool firstHintDone;
    public bool phase2Start;
    public bool moveToPointA;
    private bool moveToPointB;
    private bool bossMovedToCeiling;
    private bool shieldFirstActivated;
    public bool endOfPhase2;
    public bool ThirdCeilingisSet;
    private bool pillarInPosition;
    private bool startingFistAttack;
    private bool fistIsSlamming;
    private bool pillarIsAtCeiling;
    private bool finalTextActive;
    private bool hopefullyTheLastBoolInThisGodForsakenScript;
    private bool goToNextBoss;
    public bool droppingBombs;
    public bool laserFired;
    public bool Phase1MissilesActive;
    public bool bossIsDefeated;
	GameObject gameControl;
	GameControl playerSave;

	void Start ()
    {
        testMoveLeft = false;
        testMoveRight = false;
        heFell = false;
        movementTutorialComplete = false;
        spawnOnly1Gun = false;
        pickedUpGun = false;
        bossMoved = false;
        firstHintDone = false;
        Phase1MissilesActive = false;
        laserFired = false;
        phase2Start = false;
        moveToPointA = false;
        moveToPointB = false;
        shieldFirstActivated = false;
        endOfPhase2 = false;
        ThirdCeilingisSet = false;
        bossMovedToCeiling = false;
        droppingBombs = false;
        pillarInPosition = false;
        startingFistAttack = false;
        fistIsSlamming = false;
        pillarIsAtCeiling = true;
        bossIsDefeated = false;
        finalTextActive = false;
        hopefullyTheLastBoolInThisGodForsakenScript = false;
        goToNextBoss = false;
        ceiling2.SetActive(false);
        ceiling3.SetActive(false);
        litWall.SetActive(true);
        ground.SetActive(true);
        extendedBackground.SetActive(false);
        BossShield.SetActive(false);
        Buttons.SetActive(false);
        fistHitbox.SetActive(false);
        TractorBeam.SetActive(false);
        playerRigidbody = player.GetComponent<Rigidbody2D>();
        textBox = textBoxManager.GetComponent<TextBoxManager>();
        playerMain = player.GetComponent<PlayerMovement>();
        hittingEnemy = Boss.GetComponent<DontHitMe>();
        cameraScript = theCamera.GetComponent<BoxCamera_Locke>();
        leftButtonScript = leftButton.GetComponent<LeftButton>();
        rightButtonScript = rightButton.GetComponent<RightButton>();
        pillarDamaged = pillarWeakspot.GetComponent<DamageFistPillar>();
        playersStatus = GetComponent<PlayerStatus>();
	}
	
	void Update ()
    {
        if(pickedUpGun == false)
        {
            playerMain.canShoot = false;
        }

        if(textBox.isActive == true) //make sure Player does not collide with any weapons the boss has instantiated while the textbox is active
        {
            Physics2D.IgnoreLayerCollision(2, 8, true);
            Destroy(GameObject.Find("missile(Clone)"));
            Destroy(GameObject.Find("Bomb(Clone)"));
            GetComponent<Animator>().enabled = false;
        }
        else
        {
            Physics2D.IgnoreLayerCollision(2, 8, false);
            GetComponent<Animator>().enabled = true;
        }

        if (playerRigidbody.velocity.x < -(playerMain.maxSpeed * 0.75f) 
                && textBox.isActive == false
                && testMoveLeft == false 
                && (textBox.currentLine == 3 || textBox.currentLine == 8)) //test move left correctly done
        {
            //Debug.Log(playerRigidbody.velocity.x);
            testMoveLeft = true; //test player moving to the left marked as complete
            textBox.ReloadScript(theText);
            textBox.currentLine = 4;
            textBox.endAtLine = 5;
            textBox.EnableTextBox();
        }

        if(playerRigidbody.velocity.x > (playerMain.maxSpeed * 0.75f) 
                && textBox.isActive == false 
                && testMoveLeft == false
                && (textBox.currentLine == 3 || textBox.currentLine == 8)) //moves right when prompted to move left
            IncorrectAction();

        if (playerRigidbody.velocity.x > (playerMain.maxSpeed * 0.75f) 
                && textBox.isActive == false 
                && testMoveLeft == true
                && (textBox.currentLine == 6 || textBox.currentLine == 8)) //test move right correctly done
        {
            testMoveRight = true; //test player moving to the right marked as complete
            textBox.ReloadScript(theText);
            textBox.currentLine = 9;
            textBox.endAtLine = 10;
            textBox.EnableTextBox();
        }

        if (playerRigidbody.velocity.x < -(playerMain.maxSpeed * 0.75f) 
                && textBox.isActive == false 
                && testMoveLeft == true
                && (textBox.currentLine == 6 || textBox.currentLine == 8)) //moves left when prompted to move right
            IncorrectAction();

        if(playerRigidbody.velocity.y < -(playerMain.maxJump * 1.0f) // test jump; no fail conditions
                && textBox.isActive == false
                && textBox.currentLine == 11
                && heFell == false)
        {
            heFell = true;
            StartCoroutine("JumpTutorialDone");
        }

        if(movementTutorialComplete == true && textBox.isActive == false && spawnOnly1Gun == false) //Boss "throws" gun to player once basic movement tutorial is done
        {
            Instantiate(spaceGun, new Vector2(Boss.transform.position.x - 1, Boss.transform.position.y), Quaternion.identity);
            GameObject.Find("Space_Gun(Clone)").GetComponent<Rigidbody2D>().AddForce(new Vector2(-100, 0));
            spawnOnly1Gun = true;
        }

        if(Mathf.Abs(transform.position.x - spaceGun.transform.position.x) <= 2.5f && Input.GetKeyDown(KeyCode.Return) && pickedUpGun == false) //Checks to see if the distance between the player and the gun is close enough to pick up.
        {
            playerMain.canShoot = true;
            playerMain.canSwitch = false;
            textBox.ReloadScript(theText);
            textBox.currentLine = 17;
            textBox.endAtLine = 18;
            textBox.EnableTextBox();
            Destroy(GameObject.Find("Space_Gun(Clone)"));
            pickedUpGun = true;
			PlayerMovement pistol = GetComponent<PlayerMovement> ();
			pistol.pistol.color = new Color (255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
			pistol.pistol.color = new Color (1f, 1f, 1f, 1f);
			PlayerMovement pickedUpPistol = playerMain.GetComponent<PlayerMovement> ();
			pickedUpPistol.pickedUpWeapon = true;
			Debug.Log ("picked up pistol");
        }

        //set checkpoint marker here, pass integer into a checkpoint set method in this script

        if(textBox.currentLine == 21 && hittingEnemy.timesHit == 0) //first time boss gets hit
            hittingEnemy.timesHit++;

        if (textBox.currentLine == 23 && hittingEnemy.timesHit == 1) //second time boss gets hit
            hittingEnemy.timesHit++;

        if (textBox.currentLine == 26 && hittingEnemy.timesHit == 2) //third time boss gets hit
            hittingEnemy.timesHit++;

        if(textBox.currentLine == 28 && textBox.isActive == false && bossMoved == false) //fourth and final time boss gets hit, moves the boss to the wall
        {
            cameraScript.zoomOnBoss = false;
            Boss.transform.position = new Vector2(7.46f, 3.69f);
            bossMoved = true;
            hittingEnemy.timesHit++;
            Phase1MissilesActive = true;
            Phase1();
            CheckpointMarker(1);
        }

        //End of opening tutorial and dialogue.

        if (textBox.currentLine == 28 && textBox.isActive == false && laserFired == true) //trying to get the missile coroutine to start again after laser is fired
        {
            Phase1();
            laserFired = false;
        }

        if(textBox.currentLine == 28 && textBox.isActive == false && playersStatus.timesHit == 3) //hint the player to double jump if they get hit three times
        {
            playersStatus.timesHit++;
            textBox.ReloadScript(theText);
            textBox.currentLine = 38;
            textBox.endAtLine = 38;
            textBox.EnableTextBox();
        }

        if (textBox.currentLine == 39 && textBox.isActive == false && playersStatus.timesHit > 3 && firstHintDone == false) //Missiles would not be continuing to launch when hint would come up and laser would fire afterwards.
        {                                                                                                                   //This is because when the laser would fire, it would stop the missile co-routine and the line of text 
            textBox.currentLine = 28;                                                                                       //required for the if statement to have the missiles keep launching was never set back.
            firstHintDone = true;
        }

        //End of Phase 1

        if (textBox.currentLine == 31 && textBox.isActive == false && phase2Start == false) //beginning of phase 2
        {
            ceiling1.SetActive(false);
            ceiling2.SetActive(true);
            phase2Start = true;
            cameraScript.phase2Activated = true;
            moveToPointA = true;
            playersStatus.timesHit = 0;
            CheckpointMarker(2);
        }

        if (phase2Start == true && moveToPointA == true) //moving the boss to the right side of the raised ceiling as part of phase 2
        {
            Boss.transform.position = Vector2.MoveTowards(Boss.transform.position, PointA.transform.position, 4 * Time.deltaTime);
            Invoke("PointAMethod", 0.1f);
        }

        if(moveToPointA == true && Boss.GetComponent<Rigidbody2D>().velocity.y < 0.1f && shieldFirstActivated == false)
        {
            shieldFirstActivated = true;
            StartCoroutine("InitialShieldActivation");
            StartCoroutine("HintPlayerOfDash");
        }

        if (phase2Start == true && moveToPointB == true) //moving the boss to the left side of the raised ceiling as part of phase 2
        {
            Boss.transform.position = Vector2.MoveTowards(Boss.transform.position, PointB.transform.position, 4 * Time.deltaTime);
            Invoke("PointBMethod", 0.1f);
        }

        if (phase2Start == true && leftButtonScript.bossIsVulnerable == true && bossMovedToCeiling == true) 
            Invoke("DeActivateShield", 0.1f); //activates the shield according to button press.

        if(phase2Start == true && bossMovedToCeiling == true && droppingBombs == false) //boss starts to drop bombs here
        {
            droppingBombs = true;
            Phase2();
        }

        if(phase2Start == true && bossMovedToCeiling == true && droppingBombs == true && endOfPhase2 == false && playersStatus.timesHit == 3)
        {
            playersStatus.timesHit++;
            textBox.ReloadScript(theText);
            textBox.currentLine = 40;
            textBox.endAtLine = 40;
            textBox.EnableTextBox();
        }

        if(hittingEnemy.Phase3Active == true && endOfPhase2 == false)
        {
            CancelInvoke();
            StopCoroutine("DropBombs");
            StopCoroutine("DeActivatingBossShield"); //for some reason only calling this once here does not prevent the shield from flickering
            PointA.transform.position = new Vector2(0, 13);
            endOfPhase2 = true;
            playersStatus.timesHit = 0;
        }

        if(hittingEnemy.Phase3Active == true && endOfPhase2 == true) //once phase 3 starts, stop the boss moving left and right, stop it from dropping bombs, deactivate buttons, and move boss out of view
        {
            Boss.transform.position = Vector2.MoveTowards(Boss.transform.position, PointA.transform.position, 4 * Time.deltaTime);
            Buttons.SetActive(false);
            StopCoroutine("DeActivatingBossShield"); //This helps to make sure the shield actually doesn't flicker, even though it's running every frame
            StartCoroutine("BossShieldStayOnForPhase3");
        }

        //End of Phase 2

        if(hittingEnemy.Phase3Active == true && textBox.isActive == false && ThirdCeilingisSet == false)
        {
            ceiling2.SetActive(false);
            ceiling3.SetActive(true);
            litWall.SetActive(false);
            ground.SetActive(false);
            extendedBackground.SetActive(true);
            ThirdCeilingisSet = true;
            CheckpointMarker(3);
        }

        if(hittingEnemy.Phase3Active == true && textBox.isActive == false && pillarIsAtCeiling == true && bossIsDefeated == false && Pillar.transform.position.y >= 12.1f) //moves the fist pillar down slowly to its ceiling position
        {
            PointB.transform.position = new Vector3(player.transform.position.x, 12.19f, -0.1f);
            PointA.transform.position = Vector3.MoveTowards(PointA.transform.position, new Vector3(PointB.transform.position.x, 4.71f, -0.1f), 10 * Time.deltaTime);
            Pillar.transform.position = Vector3.MoveTowards(Pillar.transform.position, PointB.transform.position, 2 * Time.deltaTime);
        }

        if (hittingEnemy.Phase3Active == true && textBox.isActive == false && pillarIsAtCeiling == true && bossIsDefeated == false && Pillar.transform.position.y <= 12.05f) //moves the fist pillar down slowly to its ceiling position
        {
            PointB.transform.position = new Vector3(player.transform.position.x, 12.19f, -0.1f);
            PointA.transform.position = Vector3.MoveTowards(PointA.transform.position, new Vector3(PointB.transform.position.x, 4.71f, -0.1f), 10 * Time.deltaTime);
            Pillar.transform.position = Vector3.MoveTowards(Pillar.transform.position, PointB.transform.position, 10 * Time.deltaTime);
        }

        if (Pillar.transform.position.y < 12.2f && Pillar.transform.position.y > 12 && pillarIsAtCeiling == true && bossIsDefeated == false) //Once pillar is in ceiling position, it locks onto player's speed and x coordinate
        {
            Pillar.transform.position = Vector3.MoveTowards(Pillar.transform.position, PointB.transform.position, 10 * Time.deltaTime);
            TractorBeam.SetActive(true);
            pillarInPosition = true;
        } 

        if(pillarInPosition == true && startingFistAttack == false && bossIsDefeated == false) //starts the actual fist slam attack
        {
            StartCoroutine("FistSlam");
            startingFistAttack = true;
        }

        if(fistIsSlamming == true && bossIsDefeated == false) //makes the fist move very fast when it is descending, making it hard for the player to move out of the way in time
        {
            Pillar.transform.position = Vector3.MoveTowards(Pillar.transform.position, PointA.transform.position, 40 * Time.deltaTime);
        }

        if(TractorBeam.activeInHierarchy == true && bossIsDefeated == false) //if the tractor beam is active, slows the player's movement, otherwise player movement is normal
        {
            playerMain.maxSpeed = 6;
            playerRigidbody.mass = 0.25f;
        } else
        {
            playerMain.maxSpeed = 12.5f;
            playerRigidbody.mass = 1;
        }

        if (pillarDamaged.timesHit == 4 && bossIsDefeated == true && hopefullyTheLastBoolInThisGodForsakenScript == false) //idk this if statement is necessary or it's doing a bunch of extra shit
        {
            hopefullyTheLastBoolInThisGodForsakenScript = true;
            finalTextActive = true;
            Buttons.SetActive(false);
            PointB.transform.position = new Vector3(player.transform.position.x, 22.19f, -0.1f);
        }

        if(hopefullyTheLastBoolInThisGodForsakenScript == true) //move the pillar out of view cause it's defeated
        {
            Pillar.transform.position = Vector3.MoveTowards(Pillar.transform.position, PointB.transform.position, 3 * Time.deltaTime);
        }

        if(finalTextActive == true) //activate the text only one time once phase 3 is done
        {
            finalTextActive = false;
            Boss.SetActive(false);
            BossShield.SetActive(false);
            CancelInvoke();
            StopAllCoroutines();
            textBox.ReloadScript(theText);
            textBox.currentLine = 34;
            textBox.endAtLine = 34;
            textBox.EnableTextBox();
            goToNextBoss = true;
        }

        //End of Phase 3

        if (goToNextBoss == true && textBox.isActive == false)
        {
            goToNextBoss = false;
			gameControl = GameObject.FindGameObjectWithTag ("GameController");
			playerSave = gameControl.GetComponent<GameControl> ();
			playerSave.pickedUpShotgun = true;
            //SceneManager.LoadScene("First Boss Room");
            GameObject.Find("LevelManager").GetComponent<LevelManager>().LoadScene("First Boss Room");
        }
    }

    public void OnTriggerEnter2D(Collider2D triggerCollider) //Player enters collider of gun on the ground
    {
        if(triggerCollider.gameObject.tag == "Gun")
        {
            Debug.Log("In range of gun");
            textBox.ReloadScript(theText);
            textBox.currentLine = 15;
            textBox.endAtLine = 15;
            textBox.EnableTextBox();
        }
    }

    void IncorrectAction() //shortcut method for the opening tutorial dialogue so I don't have to write the same 4 lines of code
    {
        textBox.ReloadScript(theText);
        textBox.currentLine = 7;
        textBox.endAtLine = 7;
        textBox.EnableTextBox();
    }

    IEnumerator JumpTutorialDone()
    {
        yield return new WaitForSeconds(0.1f);
        textBox.ReloadScript(theText);
        textBox.currentLine = 12;
        textBox.endAtLine = 13;
        textBox.EnableTextBox();
        movementTutorialComplete = true;
    }

    void Phase1() //idk why i made these methods I just wanted to call the method instead of start coroutine. Damn I'm lazy
    {
        StartCoroutine("LaunchMissiles"); 
    }

    void Phase2()
    {
        StartCoroutine("DropBombs");
    }

    void PointAMethod() //This and PointBMethod needed to be methods because you can't directly invoke co-routines, you can only invoke methods, afaik
    {
        StartCoroutine("MovingToPointA");
    }

    void PointBMethod()
    {
        StartCoroutine("MovingToPointB");
    }

    void ActivateShield() //also needed to invoke but I might have the change this to fix the flickering bug. Also likely caused due to the fact that there are two buttons instead of 1.
    {
        //Turns out the flickering problem was due to me invoking this method for some odd reason. This method is now obsolete.
    }

    void DeActivateShield()
    {
        StartCoroutine("DeActivatingBossShield");
    }

    void FistSlamMethod()
    {
        StartCoroutine("FistSlam");
    }

    IEnumerator LaunchMissiles() //coroutine to constantly launch missiles as long as missiles being shot is active
    {
        while (Phase1MissilesActive == true)
        {
            yield return new WaitForSeconds(0.5f);
            Instantiate(missile, new Vector2(Boss.transform.position.x + 0.5f, Boss.transform.position.y - 4), Quaternion.identity);
            yield return new WaitForSeconds(0.3f);
            Instantiate(missile, new Vector2(Boss.transform.position.x + 0.5f, Boss.transform.position.y - 2), Quaternion.identity);
            yield return new WaitForSeconds(2.0f);
        }
        
    }

    IEnumerator DropBombs()
    {
        while (hittingEnemy.isInvincible == false)
        {
            yield return new WaitForSeconds(0.7331f);
            Instantiate(bomb, new Vector2(Boss.transform.position.x, Boss.transform.position.y - 1), Quaternion.identity);
        }
    }

    IEnumerator HintPlayerOfDash()
    {
        yield return new WaitForSeconds(5);
        textBox.ReloadScript(theText);
        textBox.currentLine = 36;
        textBox.endAtLine = 36;
        textBox.EnableTextBox();
    }

    IEnumerator MovingToPointA()
    {
        yield return new WaitForSeconds(3.0f);
        moveToPointA = false;
        moveToPointB = true;
    }

    IEnumerator MovingToPointB()
    {
        yield return new WaitForSeconds(3.0f);
        moveToPointB = false;
        moveToPointA = true;
    }

    IEnumerator InitialShieldActivation()
    {
        yield return new WaitForSeconds(2.0f);
        BossShield.SetActive(true);
        Buttons.SetActive(true);
        bossMovedToCeiling = true;
    }

    IEnumerator DeActivatingBossShield()
    {
        BossShield.SetActive(false);
        yield return new WaitForSeconds(3.0f);
        BossShield.SetActive(true);
    }

    IEnumerator BossShieldStayOnForPhase3()
    {
        yield return new WaitForSeconds(3.0f);
        BossShield.SetActive(true);
    }

    IEnumerator FistSlam()
    {
        yield return new WaitForSeconds(3.0f);//waits a moment before attacking the player
        for (int i = 0; i < 12; i++)          //warning flashes
        {
            fistWarning.SetActive(true);
            yield return new WaitForSeconds(0.03f);
            fistWarning.SetActive(false);
            yield return new WaitForSeconds(0.03f);
        }
        pillarIsAtCeiling = false;
        PointA.transform.position = new Vector3(PointA.transform.position.x, 4.71f, -0.1f); //sets the target slam position
        fistHitbox.SetActive(true);        
        fistIsSlamming = true;
        yield return new WaitForSeconds(0.16666667f);
        TractorBeam.SetActive(false);
        fistHitbox.SetActive(false);            //fist hitbox becomes inactive
        yield return new WaitForSeconds(2.0f); //time passes, then the fist retreats back up to the ceiling
        pillarIsAtCeiling = true;
        fistIsSlamming = false;
        startingFistAttack = false;
    }

    public static void CheckpointMarker(int checkpointNumber)
    {
        GameObject.Find("Checkpoint Manager").GetComponent<CheckpointManager>().checkpointMarkerNumber = checkpointNumber;
    }
}
