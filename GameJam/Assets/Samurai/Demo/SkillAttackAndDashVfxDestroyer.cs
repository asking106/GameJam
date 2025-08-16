using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillAttackAndDashVfxDestroyer : MonoBehaviour 
{
	public float duration = 0.7f;
	void Start () 
	{
		Invoke("destroy", duration);
	}
	
	
	void destroy () 
	{
		Destroy(gameObject);
	}
}
