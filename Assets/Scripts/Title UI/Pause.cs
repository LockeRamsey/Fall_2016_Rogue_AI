using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Pause : MonoBehaviour {
	public Transform canvas;
	public Transform player;
	public Button resume;
	public Button option;
	public Button returnToMenu;
	public Transform canvasOption;
	bool disableEscapeKey = false;


	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.Escape) && disableEscapeKey == false) {
			if (canvas.gameObject.activeInHierarchy == false) {
				canvas.gameObject.SetActive (true);
				Time.timeScale = 0;
				player.GetComponent<PlayerMovement> ().enabled = false;
				EventSystem es = GameObject.Find("EventSystem").GetComponent<EventSystem>();
				es.SetSelectedGameObject(null);
				es.SetSelectedGameObject(es.firstSelectedGameObject);
				resume.image.color = new Color (255f, 255f, 255f, 255f);
				option.image.color = new Color (255f, 255f, 255f, 255f);

			} else {
				canvas.gameObject.SetActive (false);
				Time.timeScale = 1;
				player.GetComponent<PlayerMovement> ().enabled = true;
				resume.image.color = new Color (255f, 255f, 255f, 255f);
				option.image.color = new Color (255f, 255f, 255f, 255f);
			}
		}
	}
	public void Resume(){
		canvas.gameObject.SetActive (false);
		Time.timeScale = 1;
		player.GetComponent<PlayerMovement> ().enabled = true;
		resume.image.color = new Color (255f, 255f, 255f, 255f);
		option.image.color = new Color (255f, 255f, 255f, 255f);
	}
	public void Options(){
		canvas.gameObject.SetActive (false);
		canvasOption.gameObject.SetActive (true);
		option.image.color = new Color (255f, 255f, 255f, 255f);
		disableEscapeKey = true;
	}
		
	public void Return(){
		canvas.gameObject.SetActive (true);
		canvasOption.gameObject.SetActive (false);
		returnToMenu.image.color = new Color (255f, 255f, 255f, 255f);
		disableEscapeKey = false;
	}
}
