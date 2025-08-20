using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class lever2D : MonoBehaviour
{
    private bool isin=false;
    public GameObject[] gameObjects;
    public GameObject colliders;
    private Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if(isin)
        {   
            if(Input.GetKeyDown(KeyCode.E))
            {
                animator.SetBool("isactive", true);
                colliders.SetActive(false);
                foreach(GameObject go in gameObjects)
                {
                    go.SetActive(true);
                }
            }
        }
    }
     
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
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
