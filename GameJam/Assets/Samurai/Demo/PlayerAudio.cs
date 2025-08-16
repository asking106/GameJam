using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAudio : MonoBehaviour 
{
    #region FIELD
    [SerializeField] private AudioSource runSfx;
	[SerializeField] private AudioSource AccelSfx;
	[SerializeField] private AudioSource dashSfx;
	[SerializeField] private AudioSource jumpSfx;
	[SerializeField] private AudioSource jumpLanding;
	[SerializeField] private AudioSource basicAttack01;
	[SerializeField] private AudioSource basicAttack02;
	[SerializeField] private AudioSource basicAttack03;
	[SerializeField] private AudioSource wallJump;
	[SerializeField] private AudioSource skillAttackAttacking;
	[SerializeField] private AudioSource skillAttackSheatKatana;
	[SerializeField] private AudioSource skillAttackCoolPose;
	[SerializeField] private AudioSource death;

	private Animator myAnim;
	private Playermovement playerMv;

	private bool accelCanPlay = true;
	private bool basicAttack01CanPlay = true;
	private bool basicAttack02CanPlay = true;
	private bool basicAttack03CanPlay = true;
	private bool skillAttackAttackingCanPlay = true;
	private bool skillAttackSheathKatanaCanPlay = true;
	private bool skillAttackCoolPoseCanPlay = true;
	private bool dashCanPlay = true;
	private bool jumpLandingCanPlay = true;
	private bool wallJumpCanPlay = true;
	private bool deathCanPlay = true;
	#endregion

	void Awake () 
	{
		myAnim = GetComponent<Animator>();
		playerMv = GetComponent<Playermovement>();
	}
	
	void Update () 
	{
		#region DASH
		//set dash Sound Effect
		if (!myAnim.GetCurrentAnimatorStateInfo(0).IsName("Dash"))
			dashCanPlay = true;

		if (myAnim.GetCurrentAnimatorStateInfo(0).IsName("Dash") && dashCanPlay)
        {
			dashSfx.Play();
			dashCanPlay = false;
		}
        #endregion

        #region RUN
        //set run Sound Effect 

        //set run acceleration Sound Effect
		if (!myAnim.GetCurrentAnimatorStateInfo(0).IsName("RunAcceleration"))
			accelCanPlay = true;
				

		if (myAnim.GetCurrentAnimatorStateInfo(0).IsName("RunAcceleration") && accelCanPlay)
		{ 
			AccelSfx.Play();
			accelCanPlay = false;
        }
		

		//Set Run Full Speed Sound Effect
		if (myAnim.GetCurrentAnimatorStateInfo(0).IsName("RunFullSpeed"))
        {
			runSfx.UnPause();
			accelCanPlay = true;
		}
		else
			runSfx.Pause();
		#endregion

		#region JUMP
		//set Jump Ascending Sound Effect
		
	
		if (playerMv.isJumping)
			jumpSfx.Play();
		playerMv.isJumping = false;
		
		//&& Input.GetButtonDown("Jump")
		//set jump landing sound effect
		if (myAnim.GetCurrentAnimatorStateInfo(0).IsName("JumpLandingRun") || myAnim.GetCurrentAnimatorStateInfo(0).IsName("JumpLandingIdle"))
			jumpLandingCanPlay = true;

		if ((myAnim.GetCurrentAnimatorStateInfo(0).IsName("JumpLandingRun") || myAnim.GetCurrentAnimatorStateInfo(0).IsName("JumpLandingIdle")) && jumpLandingCanPlay)
        {
			jumpLanding.Play();
			jumpLandingCanPlay = false;
		}
			
        #endregion

        #region Attack
        //Basic Attack

		//Basic Attack01
        if (!myAnim.GetCurrentAnimatorStateInfo(0).IsName("BasicAttack01"))
			basicAttack01CanPlay = true;
        
		if (myAnim.GetCurrentAnimatorStateInfo(0).IsName("BasicAttack01") && basicAttack01CanPlay)
        {
			basicAttack01.Play();
			basicAttack01CanPlay = false;
        }

		//Basic Attack02
		if (!myAnim.GetCurrentAnimatorStateInfo(0).IsName("BasicAttack02"))
			basicAttack02CanPlay = true;
		

		if (myAnim.GetCurrentAnimatorStateInfo(0).IsName("BasicAttack02") && basicAttack02CanPlay)
		{
			basicAttack02.Play();
			basicAttack02CanPlay = false;
		}

		//Basic Attack03
		if (!myAnim.GetCurrentAnimatorStateInfo(0).IsName("BasicAttack03"))
			basicAttack03CanPlay = true;
		

		if (myAnim.GetCurrentAnimatorStateInfo(0).IsName("BasicAttack03") && basicAttack03CanPlay)
		{
			basicAttack03.Play();
			basicAttack03CanPlay = false;
		}

		//Skill Attack

		//Attacking
		if (!myAnim.GetCurrentAnimatorStateInfo(0).IsName("Attacking"))
			skillAttackAttackingCanPlay = true;

		if(myAnim.GetCurrentAnimatorStateInfo(0).IsName("Attacking") && skillAttackAttackingCanPlay)
        {
			skillAttackAttacking.Play();
			skillAttackAttackingCanPlay = false;
        }

		//Sheath Katana
		if (!myAnim.GetCurrentAnimatorStateInfo(0).IsName("KatanaSheath"))
			skillAttackSheathKatanaCanPlay = true;

		if (myAnim.GetCurrentAnimatorStateInfo(0).IsName("KatanaSheath") && skillAttackSheathKatanaCanPlay)
		{
			skillAttackSheatKatana.Play();
			skillAttackSheathKatanaCanPlay = false;
		}

		//Cool Pose
		if (!myAnim.GetCurrentAnimatorStateInfo(0).IsName("CoolPose"))
			skillAttackCoolPoseCanPlay = true;

		if (myAnim.GetCurrentAnimatorStateInfo(0).IsName("CoolPose") && skillAttackCoolPoseCanPlay)
        {
			skillAttackCoolPose.Play();
			skillAttackCoolPoseCanPlay = false;
        }
		#endregion

		#region WALL JUMP AND SLIDING
		//Wall Jump
		if (!(myAnim.GetCurrentAnimatorStateInfo(0).IsName("WallJumpingRightFacingRight") || myAnim.GetCurrentAnimatorStateInfo(0).IsName("WallJumpingLeftFacingRight") || myAnim.GetCurrentAnimatorStateInfo(0).IsName("WallJumpingRightFacingLeft") || myAnim.GetCurrentAnimatorStateInfo(0).IsName("WallJumpingLeftFacingLeft")))
			wallJumpCanPlay = true;
        
			
		if (wallJumpCanPlay && (myAnim.GetCurrentAnimatorStateInfo(0).IsName("WallJumpingRightFacingRight") || myAnim.GetCurrentAnimatorStateInfo(0).IsName("WallJumpingLeftFacingRight") || myAnim.GetCurrentAnimatorStateInfo(0).IsName("WallJumpingRightFacingLeft") || myAnim.GetCurrentAnimatorStateInfo(0).IsName("WallJumpingLeftFacingLeft")))
        {
			wallJump.Play();
			wallJumpCanPlay = false;
        }
		#endregion

		#region DEATH
		if (!myAnim.GetCurrentAnimatorStateInfo(0).IsName("Death"))
			deathCanPlay = true;

		if(myAnim.GetCurrentAnimatorStateInfo(0).IsName("Death") && deathCanPlay)
        {
			death.Play();
			deathCanPlay = false;
        }
		#endregion
	}
}
