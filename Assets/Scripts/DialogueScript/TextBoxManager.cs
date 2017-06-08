using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TextBoxManager : MonoBehaviour {

    public GameObject textBox;

    public Text theText;

    public TextAsset textFile;
    public string[] textLines;

    public int currentLine;
    public int endAtLine;

    public bool isActive;
    public bool stopPlayerMovement;

    private bool isTyping = false;
    private bool cancelTyping = false;

    public float typeSpeed;

    public GameObject thePlayer;

    void Start()
    {

        if (textFile != null)
        {
            textLines = (textFile.text.Split('\n'));
        }

        if(endAtLine == 0)
        {
            endAtLine = textLines.Length - 1;
        }

        if (isActive)
        {
            EnableTextBox();
        }
        else
        {
            DisableTextBox();
        }

    }

    void Update()
    {

        if (!isActive)
        {
            return;
        }
        
        //theText.text = textLines[currentLine];

        if (textBox.activeSelf == true  && (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)))
        {
            
            if (!isTyping)
            {
                currentLine += 1;

                if (currentLine > endAtLine)
                {
                    DisableTextBox();
                }
                else
                {
                    StartCoroutine(TextScroll(textLines[currentLine]));
                }

            } else if(isTyping && !cancelTyping)
            {
                cancelTyping = true;
            }    
        }

        
        //StartCoroutine(TextScroll(textLines[currentLine]));
    }

    public void EnableTextBox()
    {
        textBox.SetActive(true);
        isActive = true;

        if (stopPlayerMovement)
        {
            GameObject.Find("Player").GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
            GameObject.Find("Player").GetComponent<Rigidbody2D>().gravityScale = 0;
            GameObject.Find("Player").GetComponent<PlayerMovement>().enabled = false; //useful code that finds the game object labeled as "Player"
        }                                                                             //in the hierarchy, takes its class, and changes the canMove to false

        StartCoroutine(TextScroll(textLines[currentLine]));
    }

    public void DisableTextBox()
    {
        textBox.SetActive(false);
        isActive = false;

        GameObject.Find("Player").GetComponent<PlayerMovement>().enabled = true; //this line of code is useful for changing the values of variables in a different script
    }

    public void ReloadScript(TextAsset theText)
    {
        if(theText != null)
        {
            textLines = new string[1];
            textLines = (theText.text.Split('\n'));
        }
    }

    private IEnumerator TextScroll (string lineOfText)
    {
        int letter = 0;
        theText.text = "";
        isTyping = true;
        cancelTyping = false;
        while (isTyping && !cancelTyping && (letter < lineOfText.Length - 1))
        {
            theText.text += lineOfText[letter];
            letter += 1;
            yield return new WaitForSeconds(typeSpeed);
        }
        theText.text = lineOfText;
        isTyping = false;
        cancelTyping = false;
    }
}
