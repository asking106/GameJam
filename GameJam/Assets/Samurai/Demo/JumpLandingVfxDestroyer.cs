using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpLandingVfxDestroyer : MonoBehaviour 
{
	private Animator myAnim;
	void Start () 
	{
		myAnim = GetComponent<Animator>();
		
	}
	
	void Update() 
	{
        if (myAnim.GetCurrentAnimatorStateInfo(0).IsName("JumpLandingVfxAnim"))
        {
			if (myAnim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
				Destroy(gameObject);
        }
	}
}
