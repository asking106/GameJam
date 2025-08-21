using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class DialogueTrigger : MonoBehaviour
{
	[Header("Dialogue")]
	public DialogueData dialogueData;                 // 该物体对应的对话
	public DialogueController dialogueController;     // 场景里的 DialogueUI 控制器

	[Header("Trigger Settings")]
	public string playerTag = "Player";              // 触发的对象 Tag
	public bool triggerOnce = true;                   // 只触发一次
	public float retriggerCooldown = 1.0f;            // 可重复触发时的冷却
	
	[Header("Player Control")]
	public bool disablePlayerMovementDuringDialogue = true; // 对话期间禁用玩家移动

	private bool hasTriggered = false;
	private float nextAllowedTime = 0f;
	private Playermovement cachedPlayerMove = null;

	private void Reset()
	{
		var col = GetComponent<Collider2D>();
		if (col != null) col.isTrigger = true;
	}

	private void OnTriggerEnter2D(Collider2D other)
	{
		if (!other.CompareTag(playerTag)) return;
		if (dialogueData == null || dialogueController == null) return;
		if (triggerOnce && hasTriggered) return;
		if (Time.unscaledTime < nextAllowedTime) return;

		cachedPlayerMove = other.GetComponent<Playermovement>();
		if (disablePlayerMovementDuringDialogue && cachedPlayerMove != null)
			cachedPlayerMove.enabled = false;

		dialogueController.OnDialogueEnded += HandleDialogueEnded;
		dialogueController.Begin(dialogueData);

		hasTriggered = true;
		nextAllowedTime = Time.unscaledTime + retriggerCooldown;
	}

	private void HandleDialogueEnded()
	{
		if (disablePlayerMovementDuringDialogue && cachedPlayerMove != null)
			cachedPlayerMove.enabled = true;
		cachedPlayerMove = null;
		dialogueController.OnDialogueEnded -= HandleDialogueEnded;
	}
}
