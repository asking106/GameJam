using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class makeTransform : MonoBehaviour
{
    public GameObject orginCamera;
    public GameObject NewSceneCamera;
    public GameObject Player;
    public Transform transformPosition;
    public bool IfneedPressE;
    public bool ison;
    // Start is called before the first frame update
    void Start()
    {
        ison=false;
    }

    // Update is called once per frame
    void Update()
    {
        if(ison)
        {
            if(Input.GetKeyDown(KeyCode.E))
            {
                Player.transform.position = transformPosition.position;
                orginCamera.SetActive(false);
                NewSceneCamera.SetActive(true);
            }
        }
    }
   
    private void OnTriggerEnter2D(Collider2D collision)
    {   
        if(collision.gameObject.tag == "Player")
        {   
            if(IfneedPressE)
            {
                ison = true;
            }
            else
            {
                Player.transform.position = transformPosition.position;
                orginCamera.SetActive(false);
                NewSceneCamera.SetActive(true);
            }
            
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag =="Player")
        {
            if(IfneedPressE)
            {
                ison = false;
            }
        }
    }
}
