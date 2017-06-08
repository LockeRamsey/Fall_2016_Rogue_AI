using UnityEngine;
using System.Collections;

public class DamageFistPillar : MonoBehaviour {

    private bool weakspotIsInvincible;
    public int timesHit;
    public GameObject Boss;
    public GameObject Player;
    private DontHitMe BossHitScript;
    private SequenceOfEvents PlayerSequenceScript;

    void Start()
    {
        weakspotIsInvincible = false;
        timesHit = 0;
        BossHitScript = Boss.GetComponent<DontHitMe>();
        PlayerSequenceScript = Player.GetComponent<SequenceOfEvents>();
    }

	void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Bullet" && weakspotIsInvincible == false)
        {
            timesHit++;
            StartCoroutine("weakspotBecomesInvincible");
        }
    }

    void Update()
    {
        if(timesHit == 4)
        {
            PlayerSequenceScript.bossIsDefeated = true;
        }
    }

    IEnumerator weakspotBecomesInvincible()
    {
        weakspotIsInvincible = true;
        for (int i = 0; i < 30; i++)
        {
            GetComponent<SpriteRenderer>().enabled = false;
            yield return new WaitForSeconds(0.016666667f);
            GetComponent<SpriteRenderer>().enabled = true;
            yield return new WaitForSeconds(0.016666667f);
        }
        weakspotIsInvincible = false;
    }
}
