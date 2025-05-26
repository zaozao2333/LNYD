using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    // 状态枚举
    public enum AIState
    {
        Move,    // 移动状态（包含巡逻和追击）
        Attack   // 攻击状态
    }

    [Header("Settings")]
    public float patrolSpeed = 6f;  // 巡逻速度略低于Hero的8f
    public float chaseSpeed = 8f;   // 追击速度与Hero相同
    public float attackRange = 1.5f;
    public float detectionRange = 5f;
    public Transform[] patrolPoints;

    [Header("Debug")]
    public AIState currentState = AIState.Move;
    public Transform currentTarget;

    private Rigidbody2D rb;
    private Transform player;
    private int currentPatrolIndex = 0;
    private bool isFacingRight = true;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Hero").transform;

        // 初始状态为移动（巡逻）
        currentState = AIState.Move;
        if (patrolPoints.Length > 0)
        {
            currentTarget = patrolPoints[0];
        }
    }

    void Update()
    {
        // 状态机核心逻辑
        switch (currentState)
        {
            case AIState.Move:
                HandleMoveState();
                break;

            case AIState.Attack:
                HandleAttackState();
                break;
        }

        // 状态转换检测
        CheckStateTransitions();
    }

    void HandleMoveState()
    {
        // 巡逻逻辑
        if (patrolPoints.Length > 0)
        {
            Vector2 direction = (currentTarget.position - transform.position).normalized;
            float targetSpeed = (currentTarget == player) ? chaseSpeed : patrolSpeed;
            rb.velocity = new Vector2(direction.x * targetSpeed, rb.velocity.y);

            // 到达巡逻点后切换下一个点
            if (Vector2.Distance(transform.position, currentTarget.position) < 0.5f)
            {
                currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
                currentTarget = patrolPoints[currentPatrolIndex];
                Flip();
            }
        }
    }

    void HandleAttackState()
    {
        // 停止移动进行攻击
        rb.velocity = Vector2.zero;

        // 实际攻击逻辑（示例：简单伤害计算）
        if (Vector2.Distance(transform.position, player.position) <= attackRange)
        {
            Debug.Log("Enemy attacking player!");
            // 这里添加实际攻击逻辑，如调用玩家的受伤方法
        }
    }

    void CheckStateTransitions()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        // 从移动切换到攻击的条件
        if (currentState == AIState.Move && distanceToPlayer <= attackRange)
        {
            currentState = AIState.Attack;
            return;
        }

        // 从攻击切换回移动的条件
        if (currentState == AIState.Attack && distanceToPlayer > attackRange)
        {
            currentState = AIState.Move;
        }

        // 如果玩家跑出检测范围，回到巡逻
        if (distanceToPlayer > detectionRange && patrolPoints.Length > 0)
        {
            currentState = AIState.Move;
            currentTarget = patrolPoints[currentPatrolIndex];
        }
        else if (currentState == AIState.Move && distanceToPlayer <= detectionRange && distanceToPlayer > attackRange)
        {
            currentTarget = player;
        }
    }

    void Flip()
    {
        // 根据移动方向翻转精灵
        isFacingRight = !isFacingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    // 可视化检测范围（仅在编辑器中显示）
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}



