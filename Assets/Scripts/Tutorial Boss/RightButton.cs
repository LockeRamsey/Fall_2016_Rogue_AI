using UnityEngine;
using System.Collections;

public class RightButton : MonoBehaviour
{
    public GameObject Player;
    public GameObject buttonUnpressedPosition;
    public GameObject buttonHalfPressedPosition;
    public GameObject buttonFullyPressedPosition;
    public GameObject buttonBaseRed;
    public GameObject otherButtonObject;

    private Rigidbody2D playerRigidBody;
    private LeftButton otherButtonScript;
    public bool goToOriginalPosition;
    public bool goToFullPressedPosition;
    public bool bossIsVulnerable;

    void Start()
    {
        playerRigidBody = Player.GetComponent<Rigidbody2D>();
        otherButtonScript = otherButtonObject.GetComponent<LeftButton>();
        goToOriginalPosition = true;
        goToFullPressedPosition = false;
        bossIsVulnerable = false;
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (other.tag == "Player" && playerRigidBody.velocity.x < 0.1f && Input.GetKey(KeyCode.D) && goToOriginalPosition == true)
        {
            transform.position = Vector2.MoveTowards(transform.position, buttonHalfPressedPosition.transform.position, 3 * Time.deltaTime);
        }

        if (other.tag == "Player" && playerRigidBody.velocity.x < 0.1f && Input.GetKey(KeyCode.D) && Input.GetKeyDown(KeyCode.Space) && goToOriginalPosition == true)
        {
            StartCoroutine("ButtonFullyPressed");
        }
    }

    void Update()
    {
        if (goToOriginalPosition == true)
            transform.position = Vector2.MoveTowards(transform.position, buttonUnpressedPosition.transform.position, Time.deltaTime);

        if (goToFullPressedPosition == true)
            transform.position = Vector2.MoveTowards(transform.position, buttonFullyPressedPosition.transform.position, 3 * Time.deltaTime);
    }

    IEnumerator ButtonFullyPressed() //Got rid of unnecessary code that would set the "BossIsVulnerable variable in this script, confusing the Sequence of Events script
    {
        goToOriginalPosition = false;
        goToFullPressedPosition = true;
        buttonBaseRed.SetActive(false);

        otherButtonScript.goToOriginalPosition = false;
        otherButtonScript.goToFullPressedPosition = true;
        otherButtonScript.buttonBaseRed.SetActive(false);
        otherButtonScript.bossIsVulnerable = true; //make sure that the bool in the other button script is also set to false

        yield return new WaitForSeconds(3.0f); //time wait for button to be green and shield to be down on boss

        buttonBaseRed.SetActive(true);

        otherButtonScript.bossIsVulnerable = false;
        otherButtonScript.buttonBaseRed.SetActive(true);

        yield return new WaitForSeconds(3.0f); //time wait before button can be pressed again

        goToOriginalPosition = true;
        goToFullPressedPosition = false;

        otherButtonScript.goToOriginalPosition = true;
        otherButtonScript.goToFullPressedPosition = false;
    }

}
