using UnityEngine;
using System.Collections;

public class BoxCamera_Locke : MonoBehaviour
    {
    //public Controller2D target;
    public Vector2 focusAreaSize;
    public float verticalOffset;
    public float lookAheadDstX;
    public float lookSmoothTimeX;
    public float verticalSmoothTime;
    public float shakeTimer;
    public float shakeAmount;
    public float sizeOfCamera;
    public bool zoomOnBoss;
    public bool phase2Activated;

    public FocusArea focusArea;
    public GameObject player;

    float currentLookAheadX;
    float targetLookAheadX;
    float lookAheadDirX;
    float smoothLookVelocityX;
    float smoothVelocity;

    void Start()
    {
        player = GameObject.Find("Player");

        focusArea = new FocusArea(player.GetComponent<Collider2D>().bounds, focusAreaSize); //referencing "Player" 2D collider bounds on the rigidbody 

        shakeAmount = 0;

        shakeTimer = 0;

        zoomOnBoss = false;

        phase2Activated = false;
    }

    void Update()
    {
        if(zoomOnBoss == true && phase2Activated == false)
        {
            EnableZoomOnBoss();
        }

        if(zoomOnBoss == false && phase2Activated == false)
        {
            DisableZoomOnBoss();
        }

        if(phase2Activated == true)
        {
            verticalOffset = 2.5f;
            GetComponent<Camera>().orthographicSize = 9.0f;
        }
    }

    void EnableZoomOnBoss()
    {
        player = GameObject.Find("Boss Sprite");
    }

    void DisableZoomOnBoss()
    {
        player = GameObject.Find("Player");
        GetComponent<Camera>().orthographicSize = sizeOfCamera;
        lookAheadDstX = 0.0f;
        lookSmoothTimeX = 0.4f;
        verticalOffset = 1f;
        GameObject.Find("Red").GetComponent<SpriteRenderer>().enabled = false;
    }


    void LateUpdate()
    {
        focusArea.Update(player.GetComponent<Collider2D>().bounds);
        Vector2 focusPosition = focusArea.centre + Vector2.up * verticalOffset;
        

        if(focusArea.velocity.x != 0)
        {
            lookAheadDirX = Mathf.Sign(focusArea.velocity.x);

        }

        targetLookAheadX = lookAheadDirX * lookAheadDstX;
        currentLookAheadX = Mathf.SmoothDamp(currentLookAheadX, targetLookAheadX, ref smoothLookVelocityX, lookSmoothTimeX); //smooths the look ahead animation
        focusPosition += Vector2.right * currentLookAheadX;

        transform.position = (Vector3)focusPosition + Vector3.forward * -3; //this line of code is what actually moves the camera itself, the -3 is how far out the camera will be to the game view.


        /*if (shakeTimer >= 0)
        {
            Vector2 ShakePosition = Random.insideUnitCircle * shakeAmount;

            transform.position = new Vector3(transform.position.x + ShakePosition.x, transform.position.y + ShakePosition.y, transform.position.z);

            shakeTimer -= Time.deltaTime;
            shakeAmount = (shakeTimer / 4) - Time.deltaTime; //reduces shaking amount over duration of shake. Increase number to make shaking less intense, decrease it to make it more intense.
        }

        if (Input.GetKeyDown(KeyCode.F)) //Press 'F' to test shake effect
        {
            ShakeCamera(0.1f, 0.5f); //0.5 seconds is good length for colliding with walls or hard objects, 0.25 is good for aerial boost effect
        }*/
    }

    void OnDrawGizmos() //Method used to draw the actual box in the scene view so it can be seen how it is implemented
    {
        Gizmos.color = new Color(1, 0, 0, .5f);
        Gizmos.DrawCube(focusArea.centre, focusAreaSize);
    }

    /*public void ShakeCamera(float shakePower, float shakeDuration)
    {
        shakeAmount = shakePower;
        shakeTimer = shakeDuration;
    }*/

    public struct FocusArea //All of the mathematical code to get the Focus Area to move with the character is in this structure
    {
        public Vector2 centre;
        public Vector2 velocity;
        float left, right;
        float top, bottom;

        public FocusArea(Bounds targetBounds, Vector2 size)
        {
            left = targetBounds.center.x - size.x / 2;
            right = targetBounds.center.x + size.x / 2;
            bottom = targetBounds.min.y;
            top = targetBounds.min.y + size.y;

            velocity = Vector2.zero;
            centre = new Vector2((left + right) / 2, (top + bottom) / 2);
        }

        public void Update(Bounds targetBounds)
        {
            float shiftX = 0;
            if (targetBounds.min.x < left)
            {
                shiftX = targetBounds.min.x - left;
            }
            else if(targetBounds.max.x > right)
            {
                shiftX = targetBounds.max.x - right;
            }

            left += shiftX;
            right += shiftX;

            float shiftY = 0;
            if (targetBounds.min.y < bottom)
            {
                shiftY = targetBounds.min.y - bottom;
            }
            else if (targetBounds.max.y > top)
            {
                shiftY = targetBounds.max.y - top;
            }

            top += shiftY;
            bottom += shiftY;
            centre = new Vector2((left + right) / 2, (top + bottom) / 2);
            velocity = new Vector2(shiftX, shiftY);
        }
    }

    
}
