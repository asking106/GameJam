using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AccelerationDestroyer : MonoBehaviour {

	public float duration = 0.7f;
	void Start () 
	{
		Invoke("destroy", duration);
	}
	
	private void destroy()
    {
		Destroy(gameObject);
    }
}
