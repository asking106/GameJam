using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnterScript : MonoBehaviour
{   
    private bool isin=false;
    public GameObject[] gameObjectTrues;
    public GameObject[] gameObjectFalses;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isin)
        {
            if(Input.GetKeyDown(KeyCode.E))
            {
                foreach (GameObject gameObject in gameObjectTrues)
                {
                    gameObject.SetActive(true);
                }
                foreach (GameObject gameObject in gameObjectFalses)
                {
                    gameObject.SetActive(false);
                }
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag=="Player")
        {
            isin = true;
        }

    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            isin = false;
        }
    }
}
