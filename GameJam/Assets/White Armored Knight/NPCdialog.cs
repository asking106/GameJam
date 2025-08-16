using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCdialog : MonoBehaviour
{
    public GameObject games;
    private bool isin=false;
    private Playermovement playermove;
    public GameObject[] dialogs;
    private int dialogsCount = 0;
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
            dialogs[dialogsCount-1].SetActive(false);
            dialogs[dialogsCount].SetActive(true);
            dialogsCount++;
            

         }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            if (Input.GetKey(KeyCode.E)&&!isin)
            {
                playermove = collision.gameObject.GetComponent<Playermovement>();
                playermove.enabled = false;
                
                isin = true;
                dialogs[dialogsCount].SetActive(true);
                dialogsCount++;


            }
        }
    }
    private void OnTriggerExit(Collider other)
    {

    }
    public void setback()
    {
        playermove.enabled = true;
    }
}
