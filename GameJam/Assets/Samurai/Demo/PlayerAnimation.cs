using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
	#region FIELD
	private Playermovement PlayerMv;
	private PlayerAttack PlayerAtt;
	private Animator myAnim;
	private Rigidbody2D myBody;
	private SpriteRenderer mySprite;

	private const string speed = "Speed";
	private const string xvelocity = "Xvelocity";
	private const string runDecceleration = "RunDecceleration";
	private const string dash = "Dashing";
	private const string grounded = "Grounded";
	private const string yvelocity = "Yvelocity";
	private const string jumplandingidle = "JumpLandingIdleCheck";
	private const string jumpLandingRun = "JumpLandingRunCheck";
	private const string wallSlidingRight = "IsWallSlidingRight";
	private const string wallSlidingLeft = "IsWallSlidingLeft";
	private const string wallJumpingRight = "IsWallJumpingRight";
	private const string wallJumpingLeft = "IsWallJumpingLeft";
	private const string facing = "FacingRight";
	private const string attacking = "SkillAttacking";
	private const string charge = "ChargeSkillAttack";
	private const string skillAttack_CoolPose = "SkillAttack_CoolPose";
	private const string katanaSheath = "KatanaSheath";
	private const string basicAttack01 = "BasicAttack01";
	private const string basicAttack02 = "BasicAttack02";
	private const string basicAttack03 = "BasicAttack03";
	private const string death = "Death";

	private bool runDeccelerationPlayying;
	private bool JumpLandingIdlePlaying;
	private bool JumpLandingRun;

	[HideInInspector] public bool coolPoseIsPlaying;
	[HideInInspector] public bool katanaSheathIsPlaying;

	[HideInInspector] public bool deathIsPlaying;

	#endregion
	void Awake () {
		myAnim = GetComponent<Animator>();
		PlayerMv = GetComponent<Playermovement>();
		myBody = GetComponent<Rigidbody2D>();
		mySprite = GetComponent<SpriteRenderer>();
		PlayerAtt = GetComponent<PlayerAttack>();
	}
	
	void Update () 
	{
        #region DASH
        myAnim.SetBool(dash, PlayerMv.isDashing);
        #endregion

        #region RUN AND IDLE
        // set Run and Idle animation
        myAnim.SetFloat(speed, Mathf.Abs(PlayerMv.move.x));
		myAnim.SetFloat(xvelocity, Mathf.Abs(myBody.velocity.x));

        if (myAnim.GetCurrentAnimatorStateInfo(0).IsName("RunDecceleration"))
        {
			runDeccelerationPlayying = true;
			if (myAnim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
            {
				runDeccelerationPlayying = false;
            }
        }
		myAnim.SetBool(runDecceleration, runDeccelerationPlayying);
        #endregion

        #region JUMP
        myAnim.SetBool(grounded, PlayerMv.grounded);
		myAnim.SetFloat(yvelocity, myBody.velocity.y);

		//set jump landing idle Animation
		if (this.myAnim.GetCurrentAnimatorStateInfo(0).IsName("JumpLandingIdle"))
        {
			JumpLandingIdlePlaying = true;
			if (myAnim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
				JumpLandingIdlePlaying = false;
		}
		else
			JumpLandingIdlePlaying = false;
		myAnim.SetBool(jumplandingidle, JumpLandingIdlePlaying);

		//Set jump landing run animation
		if (this.myAnim.GetCurrentAnimatorStateInfo(0).IsName("JumpLandingRun"))
		{
			JumpLandingRun = true;
			if (myAnim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
			{
				JumpLandingRun = false;
			}
		}
		else
			JumpLandingRun = false;
		myAnim.SetBool(jumpLandingRun, JumpLandingRun);
        #endregion

        #region WALL SLIDING AND WALL JUMP
        //Set sliding animation
        myAnim.SetBool(wallSlidingRight, PlayerMv.isWallSlidingRight);
		myAnim.SetBool(wallSlidingLeft, PlayerMv.isWallSlidingLeft);

		//Set wall jump animation
		myAnim.SetBool(wallJumpingRight, PlayerMv.isWallJumpingRight);
		myAnim.SetBool(wallJumpingLeft, PlayerMv.isWallJumpingLeft);
		myAnim.SetBool(facing, !mySprite.flipX);
		#endregion

		#region ATTACK
		//Basic Attack 

		//Check if basic attack animation is end playying
		if (PlayerMv.isDashing || PlayerAtt.skillAttack || PlayerMv.isWallSliding)
        {
			myAnim.SetBool(basicAttack01, false);
			myAnim.SetBool(basicAttack02, false);
			myAnim.SetBool(basicAttack03, false);
		}
		if (myAnim.GetCurrentAnimatorStateInfo(0).IsName("BasicAttack01"))
		{
			if (myAnim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
			{
				myAnim.SetBool(basicAttack01, false);
			}
		}

		if (myAnim.GetCurrentAnimatorStateInfo(0).IsName("BasicAttack02"))
		{
			if (myAnim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
			{
				myAnim.SetBool(basicAttack02, false);
			}
		}

		if (myAnim.GetCurrentAnimatorStateInfo(0).IsName("BasicAttack03"))
		{
			if (myAnim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
			{
				myAnim.SetBool(basicAttack03, false);
			}
		}


		//Skill Attack

		//Charge
		myAnim.SetBool(charge, PlayerAtt.skillAttackCharge);

		//Attacking
		myAnim.SetBool(attacking, PlayerAtt.skillAttacking);

		//Katana Sheath
		if (this.myAnim.GetCurrentAnimatorStateInfo(0).IsName("KatanaSheath"))
		{
			katanaSheathIsPlaying = true;
			if (myAnim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
            {
				katanaSheathIsPlaying = false;
			}
				
		}
		myAnim.SetBool(katanaSheath, katanaSheathIsPlaying);

        //Cool Pose
        if (myAnim.GetCurrentAnimatorStateInfo(0).IsName("CoolPose"))
        {
			coolPoseIsPlaying = true;
			if(myAnim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
            {
				coolPoseIsPlaying = false;
				PlayerAtt.skillAttack = false;
			}
        }
		myAnim.SetBool(skillAttack_CoolPose, coolPoseIsPlaying);
		#endregion

		#region DEATH
		if (Input.GetKeyDown(KeyCode.X) && !deathIsPlaying)
			deathIsPlaying = true;
		
		if (myAnim.GetCurrentAnimatorStateInfo(0).IsName("Death") && myAnim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
		{
			deathIsPlaying = false;
		}
		myAnim.SetBool(death, deathIsPlaying);
		#endregion
	}

    #region BASICATTACK
    public void SetBasicAttackAnimation()
    {
		if(PlayerAtt.noOfClick == 1)
        {
			myAnim.SetBool(basicAttack01, true);

			myAnim.SetBool(basicAttack02, false);
			myAnim.SetBool(basicAttack03, false);
		}
		else if (PlayerAtt.noOfClick == 2)
		{
			myAnim.SetBool(basicAttack02, true);

			myAnim.SetBool(basicAttack01, false);
			myAnim.SetBool(basicAttack03, false);
		}
		else if (PlayerAtt.noOfClick == 3)
		{
			myAnim.SetBool(basicAttack03, true);

			myAnim.SetBool(basicAttack02, false);
			myAnim.SetBool(basicAttack01, false);
		}
	}
    #endregion
}
