using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider2D))]
public class NPCDialogueInteraction : MonoBehaviour
{
    [Header("References")]
    private NPCSpawnInfo npcInfo;
    private NPCManager npcManager;
    private DialogueController dialogueController;
    private Transform player;
    
    [Header("Interaction")]
    private bool playerInRange = false;
    private bool isInteracting = false;
    
    void Start()
    {
        // 获取对话控制器
        dialogueController = FindObjectOfType<DialogueController>();
        if (dialogueController == null)
        {
            Debug.LogError("NPCDialogueInteraction: No DialogueController found in scene!");
        }
        
        // 获取玩家引用
        var playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
        
        // 设置碰撞器为触发器
        var collider = GetComponent<Collider2D>();
        if (collider != null)
        {
            collider.isTrigger = true;
        }
    }
    
    public void Initialize(NPCSpawnInfo info, NPCManager manager)
    {
        npcInfo = info;
        npcManager = manager;
    }
    
    void Update()
    {
        if (npcInfo == null || npcManager == null) return;
        
        // 检查玩家是否在交互范围内
        if (player != null)
        {
            float distance = Vector2.Distance(transform.position, player.position);
            bool inRange = distance <= npcInfo.interactionRange;
            
            if (inRange != playerInRange)
            {
                playerInRange = inRange;
                if (npcInfo.showInteractionHint)
                {
                    npcManager.ShowInteractionHint(npcInfo, playerInRange);
                }
            }
            
            // 检查交互输入
            if (playerInRange && !isInteracting && Input.GetKeyDown(npcInfo.interactionKey))
            {
                StartInteraction();
            }
        }
    }
    
    void StartInteraction()
    {
        if (isInteracting || dialogueController == null) return;
        
        isInteracting = true;
        npcInfo.isInteracting = true;
        
        // 获取当前可用的对话
        DialogueData currentDialogue = npcManager.GetNPCCurrentDialogue(npcInfo);
        
        if (currentDialogue != null)
        {
            // 订阅对话结束事件
            dialogueController.OnDialogueEnded += OnDialogueEnded;
            
            // 开始对话
            dialogueController.Begin(currentDialogue);
            
            // 隐藏交互提示
            if (npcInfo.showInteractionHint)
            {
                npcManager.ShowInteractionHint(npcInfo, false);
            }
        }
        else
        {
            // 没有可用对话
            Debug.Log($"NPC {npcInfo.npcName} has no available dialogue");
            isInteracting = false;
            npcInfo.isInteracting = false;
        }
    }
    
    void OnDialogueEnded()
    {
        if (dialogueController != null)
        {
            dialogueController.OnDialogueEnded -= OnDialogueEnded;
        }
        
        // 完成对话
        if (npcInfo != null && npcManager != null)
        {
            DialogueData completedDialogue = GetCurrentDialogue();
            if (completedDialogue != null)
            {
                npcManager.CompleteNPCDialogue(npcInfo, completedDialogue);
            }
        }
        
        // 重置状态
        isInteracting = false;
        if (npcInfo != null)
        {
            npcInfo.isInteracting = false;
        }
        
        // 重新显示交互提示（如果玩家仍在范围内）
        if (playerInRange && npcInfo != null && npcInfo.showInteractionHint)
        {
            npcManager.ShowInteractionHint(npcInfo, true);
        }
    }
    
    DialogueData GetCurrentDialogue()
    {
        if (npcInfo == null || npcInfo.dialogueStates == null) return null;
        
        // 找到当前正在进行的对话
        for (int i = 0; i < npcInfo.dialogueStates.Length; i++)
        {
            var dialogueState = npcInfo.dialogueStates[i];
            if (dialogueState.dialogueData != null && !dialogueState.isCompleted)
            {
                return dialogueState.dialogueData;
            }
        }
        
        return null;
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            if (npcInfo != null && npcInfo.showInteractionHint)
            {
                npcManager.ShowInteractionHint(npcInfo, true);
            }
        }
    }
    
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            if (npcInfo != null && npcInfo.showInteractionHint)
            {
                npcManager.ShowInteractionHint(npcInfo, false);
            }
        }
    }
    
    // 在Scene视图中绘制交互范围（仅用于调试）
    void OnDrawGizmosSelected()
    {
        if (npcInfo != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, npcInfo.interactionRange);
        }
    }
}
