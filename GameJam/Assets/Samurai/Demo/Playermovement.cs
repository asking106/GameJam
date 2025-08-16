using System.Collections;
using UnityEngine;

public class Playermovement : MonoBehaviour
{
	#region FIELD

	private PlayerAttack playerAttack;
	private PlayerAnimation playerAnim;

	[SerializeField] private PlayerMovementData data;
	[SerializeField] private float lastOnGroundTime;
	[SerializeField] private Transform groundCheckPoint;
	[SerializeField] private Transform rightWallCheck;
	[SerializeField] private Transform leftWallCheck;
	[SerializeField] private Vector2 groundCheckSize = new Vector2(0.50f, 0.07f);
	[SerializeField] private LayerMask groundLayer;
	[SerializeField] private LayerMask wallLayer;

	[HideInInspector] public Vector2 move;

	private Rigidbody2D myBody;
	private SpriteRenderer mySprite;

	//dash
	[HideInInspector] public bool isDashing;
	private bool canDash = true;
	private bool dashButtonPressed;

	//Jump
	[HideInInspector] public bool grounded;
	[HideInInspector] public bool isJumping;
	private bool jumpButtonPressed;

	//WallJump
	[HideInInspector] public bool isWallSlidingRight;
	[HideInInspector] public bool isWallSlidingLeft;
	[HideInInspector] public bool isWallJumpingRight;
	[HideInInspector] public bool isWallJumpingLeft;
	[HideInInspector] public bool isWallSliding = false;
	private bool isWallJump;
	private RaycastHit2D rightWallCheckHit;
	private RaycastHit2D leftWallChekHit;
	private float jumpTime;

	#endregion
	void Awake() 
	{
		myBody = GetComponent<Rigidbody2D>();
		mySprite = GetComponent<SpriteRenderer>();
		playerAttack = GetComponent<PlayerAttack>();
		playerAnim = GetComponent<PlayerAnimation>();
	}
	void Update()
	{	
		if (isDashing || isWallJump || playerAttack.skillAttack || playerAnim.deathIsPlaying)
			return;

		//Input Handler
		move.x = Input.GetAxisRaw("Horizontal");
		dashButtonPressed = Input.GetKeyDown(KeyCode.W);
		jumpButtonPressed = Input.GetButtonDown("Jump");

		if (jumpButtonPressed && grounded)
			jump();

		lastOnGroundTime -= Time.deltaTime;
		//Debug.Log(grounded);		

		if ((move.x != 0) && !isWallSliding && !isWallJump)
			CheckDirectionToFace(move.x > 0);
        
        if (dashButtonPressed && canDash && !isWallSliding)
			StartCoroutine(dash());

		
		if (isWallSliding && jumpButtonPressed)
			StartCoroutine(walljump());
	}

	void FixedUpdate()
    {
		if (isDashing || isWallJump || playerAttack.skillAttack || playerAnim.deathIsPlaying)
			return;

		run(1);

		//checks if set box overlaps with ground
		if (Physics2D.OverlapBox(groundCheckPoint.position, groundCheckSize, 0, groundLayer))
		{
			lastOnGroundTime = 0.1f;
			grounded = true;
		}
		else
			grounded = false;
		
		wallJumpSliding();
	}

    #region RUN
    private void run(float lerpAmount)
    {
		float targetSpeed = move.x * data.runMaxSpeed;

		float accelRate;

		targetSpeed = Mathf.Lerp(myBody.velocity.x, targetSpeed, lerpAmount);

		//Calculate Acceleration and Decceleration
		if (lastOnGroundTime > 0)
			accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? data.runAccelAmount : data.runDeccelAmount;
		else
			accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? data.runAccelAmount * data.accelInAir : data.runDeccelAmount * data.deccelInAir;
		
		if(data.doConserveMomentum && Mathf.Abs(myBody.velocity.x) > Mathf.Abs(targetSpeed) && Mathf.Sign(myBody.velocity.x) == Mathf.Sign(targetSpeed) && Mathf.Abs(targetSpeed) > 0.01f && lastOnGroundTime < 0)
			accelRate = 0;

		float speedDif = targetSpeed - myBody.velocity.x;
		float movement = speedDif * accelRate;

		//Implementing run
			myBody.AddForce(movement * Vector2.right, ForceMode2D.Force);
	}
    #endregion

    #region DASH
    private IEnumerator dash()
	{
		canDash = false;
		isDashing = true;
		float oriGrav = myBody.gravityScale;
		myBody.gravityScale = 0f;
		float tem;
		if (mySprite.flipX)
			tem = -1f;
		else
			tem = 1f;
		myBody.velocity = new Vector2(tem * data.dashPower, 0f);
		yield return new WaitForSeconds(data.dashingTime);
		if(move.x > 0)
        {
			myBody.velocity = new Vector2(data.runMaxSpeed, myBody.velocity.y);
		}
        else if(move.x < 0)
        {
			myBody.velocity = new Vector2(-data.runMaxSpeed, myBody.velocity.y);
		}
		myBody.gravityScale = oriGrav;
		isDashing = false;
		yield return new WaitForSeconds(data.dashingCoolDown);
		canDash = true;
	}
    #endregion

    #region JUMP
	private void jump()
    {
		myBody.velocity = new Vector2(myBody.velocity.x, data.jumpHeight);
		isJumping = true;
		GetComponent<CreateVfx>().jumpLandingVfxShouldPlay = true;
		GetComponent<CreateVfx>().jumpVfxPosition = myBody.transform.position;	
	}
    #endregion

    #region WALLJUMP
	private void wallJumpSliding()
    {
		//make raycast
		rightWallCheckHit = Physics2D.Raycast(rightWallCheck.position, new Vector2(data.wallDistance, 0f), data.wallDistance, wallLayer);
		Debug.DrawRay(rightWallCheck.position, new Vector2(data.wallDistance, 0f), Color.red);
		leftWallChekHit = Physics2D.Raycast(leftWallCheck.position, new Vector2(-data.wallDistance, 0f), data.wallDistance, wallLayer);
		Debug.DrawRay(leftWallCheck.position, new Vector2(-data.wallDistance, 0f), Color.blue);
		
     

		//Check if ray cast collide with wall
		if ((rightWallCheckHit || leftWallChekHit) && !grounded)
		{
			isWallSliding = true;
			jumpTime = Time.time + data.wallJumpTime;
		}
		else if (jumpTime < Time.time)
			isWallSliding = false;
		else
			isWallSliding = false;

		//Implementing Sliding 
		if (isWallSliding)
			myBody.velocity = new Vector2(myBody.velocity.x, Mathf.Clamp(myBody.velocity.y, data.wallSlideSpeed, float.MaxValue));

		if (rightWallCheckHit)
			isWallSlidingRight = true;
		else
			isWallSlidingRight = false;

		if (leftWallChekHit)
			isWallSlidingLeft = true;
		else
			isWallSlidingLeft = false;
	}

	//Implementing wall jump
	private IEnumerator walljump()
    {
		isWallSliding = false;
		isWallJump = true;
		float origrav = myBody.gravityScale;
		myBody.gravityScale = 0f;
		if (rightWallCheckHit)
        {
			myBody.velocity = new Vector2(-data.wallJumpXpower, data.wallJumpYpower);
			isWallJumpingRight = true;
		}
        if (leftWallChekHit)
        {
			myBody.velocity = new Vector2(data.wallJumpXpower, data.wallJumpYpower);
			isWallJumpingLeft = true;
		}
		isWallSlidingLeft = false;
		isWallSlidingRight = false;
		yield return new WaitForSeconds(0.1f);
		myBody.gravityScale = origrav;
		isWallJump = false;
		isWallJumpingRight = false;
		isWallJumpingLeft = false;
	}
    #endregion

    #region OTHER
    public void CheckDirectionToFace(bool isMovingRight)
	{
		if (!isMovingRight && !isWallSliding && !isWallJump && !mySprite.flipX)
			mySprite.flipX = true;
		else if(isMovingRight && !isWallSliding && !isWallJump && mySprite.flipX)
			mySprite.flipX = false;
	}
    #endregion
}
