using UnityEngine;

public class SimpleNPCSpawner : MonoBehaviour
{
    [Header("NPC References")]
    public GameObject[] npcsToShow;      // 要显示的NPC数组
    public GameObject[] npcsToHide;      // 要隐藏的NPC数组
    
    [Header("Settings")]
    public bool triggerOnce = true;      // 是否只触发一次
    public bool showOnTrigger = true;    // 是否在触发时显示NPC
    
    [Header("Debug")]
    public bool showDebugInfo = true;    // 是否显示调试信息
    
    private bool hasTriggered = false;
    
    void Start()
    {
        // 确保有碰撞器且设置为触发器
        var collider = GetComponent<Collider2D>();
        if (collider != null)
        {
            collider.isTrigger = true;
        }
        else
        {
            Debug.LogWarning("SimpleNPCSpawner: 没有找到Collider2D组件！");
        }
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (triggerOnce && hasTriggered) return;
        
        if (showDebugInfo)
        {
            Debug.Log($"玩家触发了 {gameObject.name}，开始处理NPC显示/隐藏");
        }
        
        // 处理NPC显示
        if (npcsToShow != null && npcsToShow.Length > 0)
        {
            foreach (var npc in npcsToShow)
            {
                if (npc != null)
                {
                    npc.SetActive(showOnTrigger);
                    if (showDebugInfo)
                    {
                        Debug.Log($"NPC {npc.name} 设置为 {(showOnTrigger ? "显示" : "隐藏")}");
                    }
                }
            }
        }
        
        // 处理NPC隐藏
        if (npcsToHide != null && npcsToHide.Length > 0)
        {
            foreach (var npc in npcsToHide)
            {
                if (npc != null)
                {
                    npc.SetActive(!showOnTrigger);
                    if (showDebugInfo)
                    {
                        Debug.Log($"NPC {npc.name} 设置为 {(!showOnTrigger ? "显示" : "隐藏")}");
                    }
                }
            }
        }
        
        hasTriggered = true;
        
        if (showDebugInfo)
        {
            Debug.Log($"NPC显示/隐藏处理完成！");
        }
    }
    
    // 公共方法：手动触发NPC显示/隐藏
    public void TriggerNPCSpawn()
    {
        if (triggerOnce && hasTriggered) return;
        
        // 处理NPC显示
        if (npcsToShow != null && npcsToShow.Length > 0)
        {
            foreach (var npc in npcsToShow)
            {
                if (npc != null)
                {
                    npc.SetActive(showOnTrigger);
                }
            }
        }
        
        // 处理NPC隐藏
        if (npcsToHide != null && npcsToHide.Length > 0)
        {
            foreach (var npc in npcsToHide)
            {
                if (npc != null)
                {
                    npc.SetActive(!showOnTrigger);
                }
            }
        }
        
        hasTriggered = true;
    }
    
    // 公共方法：重置触发状态
    public void ResetTrigger()
    {
        hasTriggered = false;
        if (showDebugInfo)
        {
            Debug.Log($"重置了 {gameObject.name} 的触发状态");
        }
    }
    
    // 公共方法：手动设置NPC状态
    public void SetNPCState(GameObject npc, bool active)
    {
        if (npc != null)
        {
            npc.SetActive(active);
            if (showDebugInfo)
            {
                Debug.Log($"手动设置NPC {npc.name} 为 {(active ? "显示" : "隐藏")}");
            }
        }
    }
    
    // 在Scene视图中绘制触发器范围（仅用于调试）
    void OnDrawGizmosSelected()
    {
        var collider = GetComponent<Collider2D>();
        if (collider != null)
        {
            Gizmos.color = Color.green;
            if (collider is BoxCollider2D boxCollider)
            {
                Gizmos.DrawWireCube(transform.position, boxCollider.size);
            }
            else if (collider is CircleCollider2D circleCollider)
            {
                Gizmos.DrawWireSphere(transform.position, circleCollider.radius);
            }
        }
    }
}
