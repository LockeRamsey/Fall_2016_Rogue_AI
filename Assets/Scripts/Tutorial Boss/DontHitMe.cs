using UnityEngine;
using System.Collections;

public class DontHitMe : MonoBehaviour {

    public GameObject theCamera;
    public GameObject textBoxManager;
    public GameObject theBoss;
    public GameObject Red;
    public GameObject Player;
    public GameObject Laser;
    public GameObject Warning;
    public TextAsset theText;
    public float laserDuration;

    public int timesHit;
    public int timesHitDuringMissiles;
    public int phase1totalhits;
    public int phase2totalhits;
    private TextBoxManager textBox;
    public bool isInvincible;
    private SequenceOfEvents playerSequence;
    private bool FirstHit;
    private bool SecondHit;
    private bool ThirdHit;
    private bool FourthHit;
    public bool Phase1Ends;
    public bool Phase2Ends;
    public bool Phase1Active;
    public bool Phase2Active;
    public bool Phase3Active;

    void Start()
    {
        Red.GetComponent<SpriteRenderer>().enabled = false;
        Laser.SetActive(false);
        Warning.SetActive(false);
        isInvincible = false;
        FirstHit = false;
        SecondHit = false;
        ThirdHit = false;
        FourthHit = false;
        Phase1Ends = false;
        Phase2Ends = false;
        Phase1Active = true;
        Phase2Active = false;
        Phase3Active = false;
        timesHit = 0;
        timesHitDuringMissiles = 0;
        phase1totalhits = 0;
        phase2totalhits = 0;
        textBox = textBoxManager.GetComponent<TextBoxManager>();
        playerSequence = Player.GetComponent<SequenceOfEvents>();
    }

	void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "Bullet" && timesHit == 0 && FirstHit == false) //player shoot boss, first warning
        {
            FirstHit = true;
            textBox.ReloadScript(theText);
            textBox.currentLine = 20;
            textBox.endAtLine = 20;
            textBox.EnableTextBox();
        }

        if (other.tag == "Bullet" && timesHit == 1 && SecondHit == false) //second warning
        {
            SecondHit = true;
            textBox.ReloadScript(theText);
            textBox.currentLine = 22;
            textBox.endAtLine = 22;
            textBox.EnableTextBox();
        }

        if (other.tag == "Bullet" && timesHit == 2 && ThirdHit == false) //final warning
        {
            ThirdHit = true;
            textBox.ReloadScript(theText);
            textBox.currentLine = 24;
            textBox.endAtLine = 25;
            textBox.EnableTextBox();
        }

        if (other.tag == "Bullet" && timesHit == 3 && FourthHit == false) //boss alerts player that they will be engaged, moves camera to boss and turns screen red briefly
        {
            FourthHit = true;
            Red.GetComponent<SpriteRenderer>().enabled = true;
            theCamera.GetComponent<Camera>().orthographicSize = 1.0f;
            theCamera.GetComponent<BoxCamera_Locke>().zoomOnBoss = true;
            theCamera.GetComponent<BoxCamera_Locke>().lookAheadDstX = 1;
            theCamera.GetComponent<BoxCamera_Locke>().lookSmoothTimeX = 0;
            theCamera.GetComponent<BoxCamera_Locke>().verticalOffset = -1.5f;
            textBox.ReloadScript(theText);
            textBox.currentLine = 27;
            textBox.endAtLine = 27;
            textBox.EnableTextBox();
        }

        if(other.tag == "Bullet" && timesHit == 4 && isInvincible == false && playerSequence.Phase1MissilesActive == true) //hitting boss during phase 1
        {
            timesHitDuringMissiles++;
            phase1totalhits++;
            StartCoroutine("bossBecomesInvincible");
        }

        if(timesHitDuringMissiles >= 3) //laser shoots every 3 times boss is hit
        {
            StartCoroutine("ShootLaser");
            timesHitDuringMissiles = 0;
        }

        if(other.tag == "Bullet" && phase1totalhits == 10 && Phase1Active == true && Phase1Ends == false) //after boss is hit 10 times, ends phase1, dialogue box confirms
        {
            Phase1Ends = true;
            playerSequence.Phase1MissilesActive = false;
            Phase1Active = false;
            Phase2Active = true;
            textBox.ReloadScript(theText);
            textBox.currentLine = 29;
            textBox.endAtLine = 30;
            textBox.EnableTextBox();
        }

        if(other.tag == "Bullet" && Phase2Active == true && isInvincible == false && playerSequence.BossShield.activeSelf == false) //counting the number of hits the boss has taken in phase 2
        {
            phase2totalhits++;
            StartCoroutine("bossBecomesInvincible");
        }

        if(other.tag == "Bullet" && phase2totalhits == 4 && Phase2Active == true && Phase2Ends == false) //boss hit 4 times in phase 2, end phase and do dialogue again
        {
            Phase2Ends = true;
            Phase2Active = false;
            Phase3Active = true;
            textBox.ReloadScript(theText);
            textBox.currentLine = 32;
            textBox.endAtLine = 32;
            textBox.EnableTextBox();
        }

    }

    IEnumerator bossBecomesInvincible()
    {
        isInvincible = true;
        for(int i = 0; i < 30; i++)
        {
            GetComponent<SpriteRenderer>().enabled = false;
            yield return new WaitForSeconds(0.016666667f);
            GetComponent<SpriteRenderer>().enabled = true;
            yield return new WaitForSeconds(0.016666667f);
        }
        playerSequence.droppingBombs = false;
        isInvincible = false;
    }

    IEnumerator ShootLaser() //I think this coroutine is in here because it is being called from SequenceOfEvents that depends on conditions in this script.
    {
        playerSequence.Phase1MissilesActive = false;
        for(int i = 0; i < 6; i++)
        {
            Warning.SetActive(true);
            yield return new WaitForSeconds(0.1f);
            Warning.SetActive(false);
            yield return new WaitForSeconds(0.1f);
        }
        //yield return new WaitForSeconds(2.0f);
        Laser.SetActive(true);
        yield return new WaitForSeconds(laserDuration);
        Laser.SetActive(false);
        playerSequence.Phase1MissilesActive = true;
        playerSequence.laserFired = true;
    }
}
