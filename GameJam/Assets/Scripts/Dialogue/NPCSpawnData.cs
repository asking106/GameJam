using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NPCDialogueState
{
    [Header("Dialogue Configuration")]
    public DialogueData dialogueData;           // 对话数据
    public string dialogueName;                 // 对话名称（用于调试）
    public bool isCompleted = false;            // 是否已完成
    public bool isAvailable = true;             // 是否可用
    
    [Header("Conditions")]
    public DialogueData[] requiredDialogues;    // 需要先完成的对话（前置条件）
    public bool requireAllConditions = true;    // 是否需要满足所有条件
    
    [Header("Effects")]
    public bool unlockNextDialogue = true;      // 完成后是否解锁下一个对话
    public GameObject[] itemsToGive;            // 对话完成后给予的物品
    public string[] flagsToSet;                 // 对话完成后设置的标记
}

[System.Serializable]
public class NPCSpawnInfo
{
    [Header("NPC Settings")]
    public GameObject npcPrefab;           // NPC预制体
    public Vector3 spawnPosition;          // 生成位置
    public string npcName;                 // NPC名称（用于调试）
    public bool destroyOnDialogueEnd = false; // 对话结束后是否销毁NPC
    
    [Header("Dialogue Settings")]
    public NPCDialogueState[] dialogueStates;  // 对话状态列表
    public int currentDialogueIndex = 0;        // 当前对话索引
    
    [Header("Trigger Settings")]
    public DialogueData triggerDialogue;        // 触发该NPC出现的对话
    public bool hasSpawned = false;             // 是否已经生成过
    public bool spawnOnStart = false;           // 是否在游戏开始时生成
    
    [Header("Spawn Effects")]
    public GameObject spawnEffect;               // 生成特效
    public AudioClip spawnSound;                // 生成音效
    public float spawnDelay = 0f;               // 生成延迟
    
    [Header("Interaction")]
    public float interactionRange = 2f;         // 交互范围
    public KeyCode interactionKey = KeyCode.E;   // 交互按键
    public bool showInteractionHint = true;     // 是否显示交互提示
    public string interactionHintText = "按 E 对话"; // 交互提示文本
    
    // 运行时状态
    [System.NonSerialized] public GameObject spawnedNPC;
    [System.NonSerialized] public bool isInteracting = false;
}

[CreateAssetMenu(fileName = "NewNPCSpawnData", menuName = "Dialogue/NPC Spawn Data")]
public class NPCSpawnData : ScriptableObject
{
    [Header("NPC Spawn Configuration")]
    public List<NPCSpawnInfo> npcSpawns = new List<NPCSpawnInfo>();
    
    [Header("Global Settings")]
    public bool enableNPCSpawning = true;       // 是否启用NPC生成
    public float spawnCheckInterval = 0.5f;     // 检查生成条件的间隔时间
    
    [Header("Dialogue Progress")]
    public List<string> completedDialogues = new List<string>(); // 已完成的对话ID列表
    public List<string> gameFlags = new List<string>();          // 游戏标记列表
    
    // 检查对话是否已完成
    public bool IsDialogueCompleted(string dialogueId)
    {
        return completedDialogues.Contains(dialogueId);
    }
    
    // 设置对话为已完成
    public void MarkDialogueCompleted(string dialogueId)
    {
        if (!completedDialogues.Contains(dialogueId))
        {
            completedDialogues.Add(dialogueId);
        }
    }
    
    // 检查游戏标记
    public bool HasGameFlag(string flag)
    {
        return gameFlags.Contains(flag);
    }
    
    // 设置游戏标记
    public void SetGameFlag(string flag)
    {
        if (!gameFlags.Contains(flag))
        {
            gameFlags.Add(flag);
        }
    }
    
    // 清除游戏标记
    public void ClearGameFlag(string flag)
    {
        gameFlags.Remove(flag);
    }
}
