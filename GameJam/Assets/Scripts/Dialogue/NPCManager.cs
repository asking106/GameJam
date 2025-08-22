using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class NPCManager : MonoBehaviour
{
    [Header("References")]
    public NPCSpawnData spawnData;
    public DialogueController dialogueController;
    public Transform player;
    
    [Header("UI References")]
    public GameObject interactionHintPrefab;
    public Transform canvasTransform;
    
    [Header("Events")]
    public UnityEvent<NPCSpawnInfo> OnNPCSpawned;
    public UnityEvent<NPCSpawnInfo> OnNPCDestroyed;
    public UnityEvent<string> OnDialogueCompleted;
    
    private Dictionary<string, GameObject> activeNPCs = new Dictionary<string, GameObject>();
    private Dictionary<string, GameObject> interactionHints = new Dictionary<string, GameObject>();
    private Coroutine spawnCheckCoroutine;
    
    void Start()
    {
        if (spawnData == null)
        {
            Debug.LogError("NPCManager: spawnData is not assigned!");
            return;
        }
        
        if (dialogueController == null)
        {
            dialogueController = FindObjectOfType<DialogueController>();
        }
        
        if (player == null)
        {
            var playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                player = playerObj.transform;
        }
        
        if (dialogueController != null)
        {
            dialogueController.OnDialogueEnded += OnDialogueEnded;
        }
        
        if (spawnData.enableNPCSpawning)
        {
            StartSpawnCheck();
        }
        
        SpawnInitialNPCs();
    }
    
    void OnDestroy()
    {
        if (dialogueController != null)
        {
            dialogueController.OnDialogueEnded -= OnDialogueEnded;
        }
        
        if (spawnCheckCoroutine != null)
        {
            StopCoroutine(spawnCheckCoroutine);
        }
    }
    
    void StartSpawnCheck()
    {
        if (spawnCheckCoroutine != null)
        {
            StopCoroutine(spawnCheckCoroutine);
        }
        spawnCheckCoroutine = StartCoroutine(CheckNPCSpawnConditions());
    }
    
    IEnumerator CheckNPCSpawnConditions()
    {
        while (true)
        {
            CheckAllNPCSpawnConditions();
            yield return new WaitForSeconds(spawnData.spawnCheckInterval);
        }
    }
    
    void CheckAllNPCSpawnConditions()
    {
        foreach (var npcInfo in spawnData.npcSpawns)
        {
            if (npcInfo.hasSpawned || npcInfo.spawnedNPC != null) continue;
            
            if (ShouldSpawnNPC(npcInfo))
            {
                StartCoroutine(SpawnNPC(npcInfo));
            }
        }
    }
    
    bool ShouldSpawnNPC(NPCSpawnInfo npcInfo)
    {
        if (npcInfo.triggerDialogue == null)
        {
            return npcInfo.spawnOnStart;
        }
        
        return spawnData.IsDialogueCompleted(GetDialogueId(npcInfo.triggerDialogue));
    }
    
    string GetDialogueId(DialogueData dialogue)
    {
        if (dialogue != null)
        {
            return dialogue.name;
        }
        return "";
    }
    
    void SpawnInitialNPCs()
    {
        foreach (var npcInfo in spawnData.npcSpawns)
        {
            if (npcInfo.spawnOnStart && !npcInfo.hasSpawned)
            {
                StartCoroutine(SpawnNPC(npcInfo));
            }
        }
    }
    
    IEnumerator SpawnNPC(NPCSpawnInfo npcInfo)
    {
        if (npcInfo.spawnDelay > 0)
        {
            yield return new WaitForSeconds(npcInfo.spawnDelay);
        }
        
        if (npcInfo.spawnEffect != null)
        {
            var effect = Instantiate(npcInfo.spawnEffect, npcInfo.spawnPosition, Quaternion.identity);
            Destroy(effect, 3f);
        }
        
        if (npcInfo.spawnSound != null)
        {
            AudioSource.PlayClipAtPoint(npcInfo.spawnSound, npcInfo.spawnPosition);
        }
        
        if (npcInfo.npcPrefab != null)
        {
            var npc = Instantiate(npcInfo.npcPrefab, npcInfo.spawnPosition, Quaternion.identity);
            npcInfo.spawnedNPC = npc;
            npcInfo.hasSpawned = true;
            
            string npcKey = GetNPCKey(npcInfo);
            activeNPCs[npcKey] = npc;
            
            if (!string.IsNullOrEmpty(npcInfo.npcName))
            {
                npc.name = npcInfo.npcName;
            }
            
            var npcInteraction = npc.GetComponent<NPCDialogueInteraction>();
            if (npcInteraction == null)
            {
                npcInteraction = npc.AddComponent<NPCDialogueInteraction>();
            }
            npcInteraction.Initialize(npcInfo, this);
            
            OnNPCSpawned?.Invoke(npcInfo);
            
            Debug.Log($"NPC spawned: {npcInfo.npcName} at {npcInfo.spawnPosition}");
        }
    }
    
    string GetNPCKey(NPCSpawnInfo npcInfo)
    {
        return $"{npcInfo.npcName}_{npcInfo.spawnPosition}";
    }
    
    void OnDialogueEnded()
    {
        CheckAllNPCSpawnConditions();
    }
    
    public void MarkDialogueCompleted(DialogueData dialogue)
    {
        if (dialogue != null)
        {
            string dialogueId = GetDialogueId(dialogue);
            spawnData.MarkDialogueCompleted(dialogueId);
            OnDialogueCompleted?.Invoke(dialogueId);
            
            CheckAllNPCSpawnConditions();
        }
    }
    
    public DialogueData GetNPCCurrentDialogue(NPCSpawnInfo npcInfo)
    {
        if (npcInfo.dialogueStates == null || npcInfo.dialogueStates.Length == 0)
            return null;
        
        for (int i = 0; i < npcInfo.dialogueStates.Length; i++)
        {
            var dialogueState = npcInfo.dialogueStates[i];
            if (dialogueState.isAvailable && !dialogueState.isCompleted)
            {
                if (CheckDialogueConditions(dialogueState))
                {
                    return dialogueState.dialogueData;
                }
            }
        }
        
        return null;
    }
    
    bool CheckDialogueConditions(NPCDialogueState dialogueState)
    {
        if (dialogueState.requiredDialogues == null || dialogueState.requiredDialogues.Length == 0)
            return true;
        
        if (dialogueState.requireAllConditions)
        {
            foreach (var requiredDialogue in dialogueState.requiredDialogues)
            {
                if (!spawnData.IsDialogueCompleted(GetDialogueId(requiredDialogue)))
                    return false;
            }
            return true;
        }
        else
        {
            foreach (var requiredDialogue in dialogueState.requiredDialogues)
            {
                if (spawnData.IsDialogueCompleted(GetDialogueId(requiredDialogue)))
                    return true;
            }
            return false;
        }
    }
    
    public void CompleteNPCDialogue(NPCSpawnInfo npcInfo, DialogueData completedDialogue)
    {
        for (int i = 0; i < npcInfo.dialogueStates.Length; i++)
        {
            var dialogueState = npcInfo.dialogueStates[i];
            if (dialogueState.dialogueData == completedDialogue)
            {
                dialogueState.isCompleted = true;
                
                if (dialogueState.flagsToSet != null)
                {
                    foreach (var flag in dialogueState.flagsToSet)
                    {
                        spawnData.SetGameFlag(flag);
                    }
                }
                
                if (dialogueState.unlockNextDialogue && i + 1 < npcInfo.dialogueStates.Length)
                {
                    npcInfo.dialogueStates[i + 1].isAvailable = true;
                }
                
                MarkDialogueCompleted(completedDialogue);
                
                if (npcInfo.destroyOnDialogueEnd)
                {
                    StartCoroutine(DestroyNPC(npcInfo));
                }
                
                break;
            }
        }
    }
    
    IEnumerator DestroyNPC(NPCSpawnInfo npcInfo)
    {
        yield return new WaitForSeconds(0.5f);
        
        if (npcInfo.spawnedNPC != null)
        {
            string npcKey = GetNPCKey(npcInfo);
            activeNPCs.Remove(npcKey);
            
            if (interactionHints.ContainsKey(npcKey))
            {
                Destroy(interactionHints[npcKey]);
                interactionHints.Remove(npcKey);
            }
            
            OnNPCDestroyed?.Invoke(npcInfo);
            Destroy(npcInfo.spawnedNPC);
            npcInfo.spawnedNPC = null;
        }
    }
    
    public void ShowInteractionHint(NPCSpawnInfo npcInfo, bool show)
    {
        if (!npcInfo.showInteractionHint) return;
        
        string npcKey = GetNPCKey(npcInfo);
        
        if (show)
        {
            if (!interactionHints.ContainsKey(npcKey) && interactionHintPrefab != null && canvasTransform != null)
            {
                var hint = Instantiate(interactionHintPrefab, canvasTransform);
                var hintText = hint.GetComponentInChildren<UnityEngine.UI.Text>();
                if (hintText != null)
                {
                    hintText.text = npcInfo.interactionHintText;
                }
                interactionHints[npcKey] = hint;
            }
        }
        else
        {
            if (interactionHints.ContainsKey(npcKey))
            {
                Destroy(interactionHints[npcKey]);
                interactionHints.Remove(npcKey);
            }
        }
    }
}
