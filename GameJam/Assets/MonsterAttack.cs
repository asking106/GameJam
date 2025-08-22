using UnityEngine;

public class EnemyChaser : MonoBehaviour
{
    public enum EnemyState { Idle, Chasing, Attacking, Cooldown }

    [Header("Detection")]
    public float detectionRadius = 5f; // 发现玩家的半径
    public LayerMask playerLayer; // 玩家所在的层

    [Header("Chasing")]
    public float moveSpeed = 3f;
    public float stoppingDistance = 1.5f; // 离玩家多近时停下攻击
    public float chaseCooldownTime = 2f; // 玩家超出范围后继续追击的时间

    [Header("Attacking")]
    public float attackRate = 1f; // 攻击频率（每秒攻击次数）
    public int attackDamage = 10;
    public float attackRange = 1.8f; // 攻击判定范围

    public EnemyState currentState = EnemyState.Idle;
    private Transform playerTarget;
    private float nextAttackTime = 0f;
    private float chaseCooldownTimer = 0f; // 追击冷却计时器
    private Rigidbody2D rb;
    private Animator animator; // 可选：用于控制动画

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (playerTarget == null)
        {
            FindPlayer();
            // 如果找不到玩家且正在追击或冷却状态，切换到闲置
            if (playerTarget == null && (currentState == EnemyState.Chasing || currentState == EnemyState.Cooldown))
            {
                currentState = EnemyState.Idle;
                rb.velocity = Vector2.zero;
                if (animator != null) animator.SetBool("IsChasing", false);
            }
            return;
        }

        // 计算与玩家的距离
        float distanceToPlayer = Vector2.Distance(transform.position, playerTarget.position);

        // --- 状态机逻辑 ---
        switch (currentState)
        {
            case EnemyState.Idle:
                // 如果玩家进入检测范围，切换到追击状态
                if (distanceToPlayer <= detectionRadius)
                {
                    currentState = EnemyState.Chasing;
                    if (animator != null) animator.SetBool("IsChasing", true);
                }
                break;

            case EnemyState.Chasing:
                // 如果玩家超出检测范围，进入冷却状态而不是立即停止
                if (distanceToPlayer > detectionRadius)
                {
                    currentState = EnemyState.Cooldown;
                    chaseCooldownTimer = chaseCooldownTime; // 设置冷却时间
                    break;
                }

                // 如果进入攻击范围，切换到攻击状态
                if (distanceToPlayer <= stoppingDistance)
                {
                    currentState = EnemyState.Attacking;
                    rb.velocity = Vector2.zero; // 停止移动
                    if (animator != null)
                    {
                        animator.SetBool("IsChasing", false);
                        animator.SetTrigger("Attack"); // 触发攻击动画
                    }
                    break;
                }

                // 持续向玩家移动
                ChasePlayer();
                break;

            case EnemyState.Attacking:
                // 如果玩家跑出攻击范围但还在检测范围内，返回追击状态
                if (distanceToPlayer > stoppingDistance && distanceToPlayer <= detectionRadius)
                {
                    currentState = EnemyState.Chasing;
                    if (animator != null) animator.SetBool("IsChasing", true);
                }
                // 如果玩家超出检测范围，进入冷却状态
                else if (distanceToPlayer > detectionRadius)
                {
                    currentState = EnemyState.Cooldown;
                    chaseCooldownTimer = chaseCooldownTime;
                    if (animator != null) animator.SetBool("IsChasing", false);
                }
                // 否则，执行攻击逻辑
                else
                {
                    // 攻击冷却计时
                    if (Time.time >= nextAttackTime)
                    {
                        PerformAttack();
                        animator.SetTrigger("Attack"); // 触发攻击动画
                        nextAttackTime = Time.time + 1f / attackRate;
                    }
                }
                break;

            case EnemyState.Cooldown:
                // 更新冷却计时器
                chaseCooldownTimer -= Time.deltaTime;

                // 如果冷却时间结束，返回闲置状态
                if (chaseCooldownTimer <= 0f)
                {
                    currentState = EnemyState.Idle;
                    rb.velocity = Vector2.zero;
                    if (animator != null) animator.SetBool("IsChasing", false);
                    break;
                }

                // 如果玩家重新进入检测范围，立即返回追击状态
                if (distanceToPlayer <= detectionRadius)
                {
                    currentState = EnemyState.Chasing;
                    if (animator != null) animator.SetBool("IsChasing", true);
                    break;
                }

                // 在冷却期间继续追击玩家
                ChasePlayer();
                break;
        }
    }

    private void ChasePlayer()
    {
        // 持续向玩家移动
        Vector2 direction = (playerTarget.position - transform.position).normalized;
        rb.velocity = new Vector2(direction.x,0) * moveSpeed;

        // 控制敌人朝向
        if (direction.x > 0)
            transform.localScale = new Vector3(-9, 9, 9);
        else if (direction.x < 0)
            transform.localScale = new Vector3(9, 9, 9);
    }

    private void FindPlayer()
    {
        // 使用OverlapCircle检测范围内的玩家
        Collider2D playerCollider = Physics2D.OverlapCircle(transform.position, detectionRadius, playerLayer);
        if (playerCollider != null && playerCollider.CompareTag("Player"))
        {
            playerTarget = playerCollider.transform;
        }
    }

    private void PerformAttack()
    {
        // 实际攻击逻辑
        Collider2D hitPlayer = Physics2D.OverlapCircle(transform.position, attackRange, playerLayer);
        if (hitPlayer != null)
        {
            // 对玩家造成伤害
           // PlayerHealth playerHealth = hitPlayer.GetComponent<PlayerHealth>();
           // if (playerHealth != null)
           // {
           //     playerHealth.TakeDamage(attackDamage);
         //   }
        }
    }

    // 在Scene视图中绘制检测范围，便于调试
    private void OnDrawGizmosSelected()
    {
        // 绘制检测范围（黄色）
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        // 绘制攻击/停止范围（红色）
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, stoppingDistance);
    }

    // 可选：显示冷却时间（用于调试）
    private void OnGUI()
    {
        if (currentState == EnemyState.Cooldown)
        {
            GUI.Label(new Rect(10, 30, 200, 20), $"Chase Cooldown: {chaseCooldownTimer:F1}s");
        }
    }
}