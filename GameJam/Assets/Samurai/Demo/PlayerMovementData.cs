using UnityEngine;

[CreateAssetMenu(menuName = "Player Run Data")]
public class PlayerMovementData : ScriptableObject 
{


	[Header("Run")]
	public float runMaxSpeed; //Target speed we want the player to reach.
	public float runAcceleration; //Time (approx.) time we want it to take for the player to accelerate from 0 to the runMaxSpeed.
	[HideInInspector] public float runAccelAmount; //The actual force (multiplied with speedDiff) applied to the player.
	public float runDecceleration; //Time (approx.) we want it to take for the player to accelerate from runMaxSpeed to 0.
	[HideInInspector] public float runDeccelAmount; //Actual force (multiplied with speedDiff) applied to the player .
	[Space(10)]
	[Range(0.01f, 1)] public float accelInAir; //Multipliers applied to acceleration rate when airborne.
	[Range(0.01f, 1)] public float deccelInAir;
	public bool doConserveMomentum;

	[Space(20)]

	[Header("Jump")]
	public float jumpHeight; //Height of the player's jump

	[Space(20)]

	[Header("WallJump")]
	public float wallJumpTime = 0.2f;
	public float wallSlideSpeed = 0.3f;
	public float wallDistance = 0.55f;
	public float wallJumpXpower = 20f;
	public float wallJumpYpower = 3f;
	
	[Space(20)]

	[Header("Dash")]
	public float dashPower = 30f;
	public float dashingCoolDown = 1f;
	public float dashingTime = 0.2f;

	
	

	private void OnValidate()
	{
		
		runAccelAmount = (50 * runAcceleration) / runMaxSpeed;
		runDeccelAmount = (50 * runDecceleration) / runMaxSpeed;

	

		#region Variable Ranges
		runAcceleration = Mathf.Clamp(runAcceleration, 0.01f, runMaxSpeed);
		runDecceleration = Mathf.Clamp(runDecceleration, 0.01f, runMaxSpeed);
		#endregion
	}
}
