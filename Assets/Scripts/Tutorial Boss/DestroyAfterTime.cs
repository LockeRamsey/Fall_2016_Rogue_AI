using UnityEngine;
using System.Collections;

public class DestroyAfterTime : MonoBehaviour
{

    public float seconds = 5f;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Object.Destroy(gameObject, seconds);
    }
}
