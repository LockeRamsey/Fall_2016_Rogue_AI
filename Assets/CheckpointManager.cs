using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CheckpointManager : MonoBehaviour {

    public static CheckpointManager Instance;
    AsyncOperation loadSceneOperation;
    public int checkpointMarkerNumber;

	public void Awake()
    {
        if (CheckpointManager.Instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
    }

	void Update ()
    {
		if(loadSceneOperation != null)
        {
            if (loadSceneOperation.isDone)
            {
                //put in if statement to make sure you're not restoring a checkpoint when in the main menu scene
                if (!SceneManager.GetActiveScene().name.Equals("Tutorial Boss Room"))
                {
                    loadSceneOperation = null;
                }
                else
                {
                    RestoreCheckpoint();
                    loadSceneOperation = null;
                }
            }
        }
	}

    public void ReloadScene(string sceneName)
    {
        loadSceneOperation = SceneManager.LoadSceneAsync(sceneName);
    }

    public void RestoreCheckpoint()
    {
        SequenceOfEvents GameManager = GameObject.Find("Player").GetComponent<SequenceOfEvents>();
        switch (checkpointMarkerNumber)
        {
            case 1:
                GameManager.pickedUpGun = true;
                GameManager.textBox.currentLine = 28;
                GameManager.playerMain.canShoot = true;
                GameManager.playerMain.canSwitch = false;
                GameManager.textBox.isActive = false;
                GameManager.startDialogueTrigger.SetActive(false);
                GameManager.hittingEnemy.timesHit = 3;
                break;
            case 2:
                GameManager.pickedUpGun = true;
                GameManager.textBox.currentLine = 31;
                GameManager.playerMain.canShoot = true;
                GameManager.playerMain.canSwitch = false;
                GameManager.textBox.isActive = false;
                GameManager.startDialogueTrigger.SetActive(false);
                GameManager.hittingEnemy.timesHit = 4;
                GameManager.ceiling1.SetActive(false);
                GameManager.ceiling2.SetActive(true);
                GameManager.phase2Start = true;
                GameManager.cameraScript.phase2Activated = true;
                GameManager.moveToPointA = true;
                GameManager.playersStatus.timesHit = 0;
                GameManager.hittingEnemy.phase1totalhits = 10;
                GameManager.hittingEnemy.Phase1Ends = true;
                GameManager.hittingEnemy.Phase1Active = false;
                GameManager.hittingEnemy.Phase2Active = true;
                break;
            case 3:
                GameManager.pickedUpGun = true;
                GameManager.textBox.currentLine = 41;
                GameManager.playerMain.canShoot = true;
                GameManager.playerMain.canSwitch = false;
                GameManager.textBox.isActive = false;
                GameManager.startDialogueTrigger.SetActive(false);
                GameManager.hittingEnemy.timesHit = 4;
                GameManager.ceiling1.SetActive(false);
                GameManager.ceiling3.SetActive(true);
                GameManager.litWall.SetActive(false);
                GameManager.ground.SetActive(false);
                GameManager.extendedBackground.SetActive(true);
                GameManager.cameraScript.phase2Activated = true;
                GameManager.endOfPhase2 = true;
                GameManager.moveToPointA = true;
                GameManager.playersStatus.timesHit = 0;
                GameManager.ThirdCeilingisSet = true;
                GameManager.hittingEnemy.phase1totalhits = 10;
                GameManager.hittingEnemy.phase2totalhits = 4;
                GameManager.hittingEnemy.Phase1Ends = true;
                GameManager.hittingEnemy.Phase2Ends = true;
                GameManager.hittingEnemy.Phase1Active = false;
                GameManager.hittingEnemy.Phase2Active = false;
                GameManager.hittingEnemy.Phase3Active = true;
                GameManager.StartCoroutine("DropBombs");
                break;
            default:
                break;    
        }
    }
}
