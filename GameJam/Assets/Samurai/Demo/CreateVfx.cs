using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateVfx : MonoBehaviour 
{
    #region FIELD

    private GameObject tem = null;
	private Animator myAnim;
	private SpriteRenderer mySprite;
	private Playermovement playerMv;

	[HideInInspector] public Vector3 jumpVfxPosition;

	private bool accelerationVfxCanPlay = true;
	private bool dashVfxCanPlay = true;
	private bool skillAttackVfxCanPlay = true;
	private bool jumpAscendingVfxCanPlay = true;
	private bool jumpLandingVfxCanPlay = true;
	[HideInInspector] public bool jumpLandingVfxShouldPlay;

    #endregion
    void Awake () 
	{
		myAnim = GetComponent<Animator>();
		mySprite = GetComponent<SpriteRenderer>();
		playerMv = GetComponent<Playermovement>();
	}
	
	void Update () 
	{
        #region RUN ACCELERATION
        if (!myAnim.GetCurrentAnimatorStateInfo(0).IsName("RunAcceleration"))
			accelerationVfxCanPlay = true;

		if (myAnim.GetCurrentAnimatorStateInfo(0).IsName("RunAcceleration") && accelerationVfxCanPlay)
        {
			tem = Instantiate(Resources.Load("Prefabs/Vfx/AccelerationVfx"), transform.position, transform.rotation) as GameObject;
			tem.GetComponent<SpriteRenderer>().flipX = !mySprite.flipX;
			accelerationVfxCanPlay = false;
		}
        #endregion

        #region DASH
		//Dash
        if (!myAnim.GetCurrentAnimatorStateInfo(0).IsName("Dash"))
			dashVfxCanPlay = true;

		if (myAnim.GetCurrentAnimatorStateInfo(0).IsName("Dash") && playerMv.grounded && dashVfxCanPlay)
        {
			tem = Instantiate(Resources.Load("Prefabs/Vfx/SkillAttack&DashVfx"), transform.position, transform.rotation) as GameObject;
			tem.GetComponent<SpriteRenderer>().flipX = !mySprite.flipX;
			dashVfxCanPlay = false;
		}

		#endregion

		#region SKILLL ATTACK
		if (!myAnim.GetCurrentAnimatorStateInfo(0).IsName("Attacking"))
			skillAttackVfxCanPlay = true;

		if (myAnim.GetCurrentAnimatorStateInfo(0).IsName("Attacking") && skillAttackVfxCanPlay)
		{
			tem = Instantiate(Resources.Load("Prefabs/Vfx/SkillAttack&DashVfx"), transform.position, transform.rotation) as GameObject;
			tem.GetComponent<SpriteRenderer>().flipX = !mySprite.flipX;
			skillAttackVfxCanPlay = false;
		}

		#endregion

		#region JUMP
		//Jump Ascending
		if (!myAnim.GetCurrentAnimatorStateInfo(0).IsName("Jump"))
        {
			jumpAscendingVfxCanPlay = true;
        }

		if(jumpAscendingVfxCanPlay && jumpLandingVfxShouldPlay && myAnim.GetCurrentAnimatorStateInfo(0).IsName("Jump") && myAnim.GetCurrentAnimatorStateInfo(0).normalizedTime > 4f)
		{
			tem = Instantiate(Resources.Load("Prefabs/Vfx/JumpAscendingVfx"), jumpVfxPosition, transform.rotation) as GameObject;
			tem.GetComponent<SpriteRenderer>().flipX = !mySprite.flipX;
			jumpLandingVfxShouldPlay = false;
			jumpAscendingVfxCanPlay = false;	
		}
		//JumpLanding
		if (!myAnim.GetCurrentAnimatorStateInfo(0).IsName("JumpLandingIdle") && !myAnim.GetCurrentAnimatorStateInfo(0).IsName("JumpLandingRun"))
			jumpLandingVfxCanPlay = true;

		if ((myAnim.GetCurrentAnimatorStateInfo(0).IsName("JumpLandingIdle") || myAnim.GetCurrentAnimatorStateInfo(0).IsName("JumpLandingRun")) && jumpLandingVfxCanPlay)
		{
			tem = Instantiate(Resources.Load("Prefabs/Vfx/JumpLandingVfx"), transform.position, transform.rotation) as GameObject;
			tem.GetComponent<SpriteRenderer>().flipX = !mySprite.flipX;
			jumpLandingVfxCanPlay = false;
		}

		#endregion
	}
}
