using Assets.FantasyInventory.Scripts.Data;
using Assets.FantasyInventory.Scripts.Enums;
using Assets.FantasyInventory.Scripts.Interface;
using Assets.FantasyInventory.Scripts.Interface.Elements;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pickUpItem : MonoBehaviour
{ private bool isin;
    public ScrollInventory inventory;
    // Start is called before the first frame update
    void Start()
    {
        isin=false;
    }

    // Update is called once per frame
    void Update()
    {
        if(isin)
        {
            if(Input.GetKeyDown(KeyCode.E))
            {
                inventory.Items.Add(new Item(ItemId.仙女剑, 1));
                
                Destroy(gameObject);

                
            }
                
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag =="Player")
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
