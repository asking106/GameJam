using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Move2D : MonoBehaviour
{
    public float speed = 5f;
    private Rigidbody2D rb;
    private Vector2 movedirection;

    private void Start()
    {
         
        rb=GetComponent<Rigidbody2D>();
        
    }

    private void FixedUpdate()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        movedirection=new Vector2(horizontalInput, verticalInput).normalized;
        rb.velocity = movedirection*speed;
        
    }

     
}
