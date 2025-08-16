using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCdialog : MonoBehaviour
{
    public GameObject games;
    private bool isin=false;
    private Playermovement playermove;
    public GameObject[] dialogs;
    public int dialogsCount = 0;
    // Start is called before the first frame update

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.E)&&isin)
        {
            if (dialogsCount == dialogs.Length)
            {
                dialogs[dialogsCount - 1].SetActive(false);
                playermove.enabled = true;

                return;

            }
            else
            {
                playermove.enabled=false;
            }
            

            if(dialogsCount!=0)
            dialogs[dialogsCount-1].SetActive(false);
            dialogs[dialogsCount].SetActive(true);
            dialogsCount++;
            

         }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        { 
                
                 playermove= collision.GetComponent<Playermovement>();
                 
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
    
    public void setback()
    {
        playermove.enabled = true;
    }
}
