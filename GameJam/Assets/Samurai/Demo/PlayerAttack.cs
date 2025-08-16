using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
	#region FIELD
	private Playermovement playerMv;
	private PlayerAnimation playerAnim;
	private Rigidbody2D myBody;
	private SpriteRenderer mySprite;
	private Animator myAnim;



	//Basic Attack
	[Header("Basic Attack")]
	public int noOfClick = 0;
	public float maxComboDelay = 1f;
	private float lastClickedTime = 0f;
	private bool basicAttackButtonPressed;


	//SkillAttack
	[Header("Skill Attack")]
	public bool skillAttack;
	public float attackPower = 50f;
	[HideInInspector] public bool skillAttackCharge;
	[HideInInspector] public bool skillAttacking;
	private bool skillAttackButtonPressed;
	private bool skillAttackButtonReleased;
    #endregion

    // Use this for initialization
    void Awake() 
	{
		myBody = GetComponent<Rigidbody2D>();
		playerMv = GetComponent<Playermovement>();
		mySprite = GetComponent<SpriteRenderer>();
		playerAnim = GetComponent<PlayerAnimation>();
		myAnim = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (skillAttacking || playerAnim.katanaSheathIsPlaying || playerAnim.coolPoseIsPlaying || playerMv.isWallSliding || playerMv.isDashing || playerAnim.deathIsPlaying)
        {
			noOfClick = 0;
			return;
		}
			

		//Input Handle
		basicAttackButtonPressed = Input.GetMouseButtonDown(0);
		skillAttackButtonPressed = Input.GetMouseButtonDown(1);
		skillAttackButtonReleased = Input.GetMouseButtonUp(1);

		//Basic Attack
		ComboMethod();

		//Skill Attack
		if (skillAttackButtonPressed && playerMv.grounded)
        {
			skillAttackCharge = true;
			skillAttack = true;
		}

		if (skillAttackButtonReleased && skillAttackCharge)
			StartCoroutine(skillAttackMethod());
	}

	private void FixedUpdate()
    {
		if (skillAttacking || playerAnim.katanaSheathIsPlaying || playerMv.isWallSliding || playerMv.isDashing)
			return;

		//Basic Attack
		BasicAttackMethod();

		//Skill Attack
		if (skillAttackCharge)
			myBody.velocity = new Vector2(0f, 0f);
	}

    #region BASIC_ATTACK
	private void ComboMethod()
    {
		if (Time.time - lastClickedTime > maxComboDelay)
		{
			noOfClick = 0;
		}

		if (basicAttackButtonPressed && !skillAttackCharge && !myAnim.GetCurrentAnimatorStateInfo(0).IsName("attack1") && !myAnim.GetCurrentAnimatorStateInfo(0).IsName("attack2") && !myAnim.GetCurrentAnimatorStateInfo(0).IsName("attack3"))	
		{
			lastClickedTime = Time.time;
			
			noOfClick++;
			
			noOfClick = Mathf.Clamp(noOfClick, 0, 4);
			playerAnim.SetBasicAttackAnimation();
		}
		
		if (noOfClick >= 4 && Input.GetMouseButtonDown(0))
		{
			noOfClick = 0;
		}
	}

	private void BasicAttackMethod()
    {
		if (!mySprite.flipX)
		{
			if (myAnim.GetCurrentAnimatorStateInfo(0).IsName("BasicAttack01"))
			{
				myBody.velocity = new Vector2(2f, myBody.velocity.y);
			}

			if (myAnim.GetCurrentAnimatorStateInfo(0).IsName("BasicAttack02"))
			{
				myBody.velocity = new Vector2(2f, myBody.velocity.y);
			}

			if (myAnim.GetCurrentAnimatorStateInfo(0).IsName("BasicAttack03"))
			{
				myBody.velocity = new Vector2(4f, myBody.velocity.y);
			}
		}
		else
		{
			if (myAnim.GetCurrentAnimatorStateInfo(0).IsName("BasicAttack01"))
			{
				myBody.velocity = new Vector2(-2f, myBody.velocity.y);
			}

			if (myAnim.GetCurrentAnimatorStateInfo(0).IsName("BasicAttack02"))
			{
				myBody.velocity = new Vector2(-2f, myBody.velocity.y);
			}

			if (myAnim.GetCurrentAnimatorStateInfo(0).IsName("BasicAttack03"))
			{
				myBody.velocity = new Vector2(-4f, myBody.velocity.y);
			}
		}
	}
    #endregion

    #region SKILL_ATTACK
    private IEnumerator skillAttackMethod()
    {
		skillAttackCharge = false;
		skillAttacking = true;
		if (!mySprite.flipX)
			myBody.velocity = new Vector2(attackPower, 0f);
		else
			myBody.velocity = new Vector2(-attackPower, 0f);
		yield return new WaitForSeconds(0.2f);
		myBody.velocity = new Vector2(0f, 0f);
		skillAttacking = false;
	}
	#endregion


}

