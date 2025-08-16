using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpAscendingDestroyer : MonoBehaviour 
{
	//public float duration = 0.7f;
	private Animator myAnim;
	void Start () 
	{
		//Invoke("destroy", duration);
		myAnim = GetComponent<Animator>();
	}
	
	void Update () 
	{
        if (myAnim.GetCurrentAnimatorStateInfo(0).IsName("JumpAscendingAnim")){
			if(myAnim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
				Destroy(gameObject);

		}
		
	}
}
