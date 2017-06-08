using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class LevelManager : MonoBehaviour {

	public void LoadScene(string sceneName)
	{
		Debug.Log ("New level load: " + sceneName);


        if (!SceneManager.GetActiveScene().name.Equals(sceneName).Equals(SceneManager.GetActiveScene().name.Equals("Tutorial Boss Room"))) //makes sure that checkpoint marker is only reset to 0 when we are not reloading the same scene
        {
            GameObject.Find("Checkpoint Manager").GetComponent<CheckpointManager>().checkpointMarkerNumber = 0;
        }
        //SceneManager.LoadScene (sceneName);
        CheckpointManager.Instance.ReloadScene(sceneName);
	}
	public void QuitRequest(){
		Debug.Log ("Quit Requested");
		Application.Quit ();
	}
}
