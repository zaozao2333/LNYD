using UnityEngine;
using System.Collections;
public class Enemy : MonoBehaviour
{
    // 状态枚举
    public enum AIState
    {
        Move,    // 移动状态（包含巡逻和追击）
        Attack   // 攻击状态
    }

    [Header("Settings")]
    public float health = 10f;
    public float patrolSpeed = 6f;  // 巡逻速度略低于Hero的8f
    public float chaseSpeed = 8f;   // 追击速度与Hero相同
    public float attackRange = 1.5f;
    public float detectionRange = 5f;
    public Transform[] patrolPoints;
    public string patrolPointsParentName = "PatrolPoints"; // 巡逻点父对象名称

    [Header("Debug")]
    public AIState currentState = AIState.Move;
    public Transform currentTarget;

    protected Rigidbody2D rb;
    protected Transform player;
    private int currentPatrolIndex = 0;
    private bool isFacingRight;
    protected bool isAttacking = false;
    private SpriteRenderer sr; // 新增变量：精灵渲染器
    private Color originalColor; // 存储原始颜色

    virtual protected void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Hero").transform;
        sr = GetComponentInChildren<SpriteRenderer>(); // 获取SpriteRenderer组件
        if (sr != null)
        {
            originalColor = sr.color; // 存储原始颜色
        }

        // 初始状态为移动（巡逻）
        currentState = AIState.Move;

        // 通过名称查找巡逻点父对象
        GameObject patrolParent = GameObject.Find(patrolPointsParentName);
        if (patrolParent != null)
        {
            // 获取所有子对象作为巡逻点
            patrolPoints = new Transform[patrolParent.transform.childCount];
            for (int i = 0; i < patrolParent.transform.childCount; i++)
            {
                patrolPoints[i] = patrolParent.transform.GetChild(i);
            }
        }

        if (patrolPoints.Length > 0)
        {
            currentTarget = patrolPoints[0];
            isFacingRight = patrolPoints[0].position.x - transform.position.x > 0;
            if(!isFacingRight)
                transform.Rotate(0, 180, 0);
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

    protected virtual void HandleMoveState()
    {
        // 巡逻逻辑
        if (patrolPoints.Length > 0)
        {
            Vector2 direction = (currentTarget.position - transform.position).normalized;
            float targetSpeed = (currentTarget == player) ? chaseSpeed : patrolSpeed;
            rb.velocity = new Vector2(direction.x * targetSpeed, rb.velocity.y);

            // 新增：追击玩家时实时检测朝向
            if (currentTarget == player)
            {
                bool shouldFaceRight = direction.x > 0;
                if (shouldFaceRight != isFacingRight)
                {
                    Flip();
                }
            }

            // 到达巡逻点后切换下一个点
            if (Vector2.Distance(transform.position, currentTarget.position) < 0.5f)
            {
                currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
                currentTarget = patrolPoints[currentPatrolIndex];
                Flip();
            }
        }
    }

    virtual protected void HandleAttackState()
    {
        // 停止移动进行攻击
        rb.velocity = Vector2.zero;

        // 实际攻击逻辑（示例：简单伤害计算）
        if (Vector2.Distance(transform.position, player.position) <= attackRange)
        {
            // 这里添加实际攻击逻辑，如调用玩家的受伤方法
        }
    }

    virtual protected void CheckStateTransitions()
    {
        if (isAttacking) return;
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        // 从移动切换到攻击的条件
        if (currentState == AIState.Move && distanceToPlayer <= attackRange)
        {
            currentState = AIState.Attack;
            return;
        }

        // 从攻击切换回移动的条件
        if (currentState == AIState.Attack && distanceToPlayer > attackRange && !isAttacking)
        {
            currentState = AIState.Move;
        }

        // 如果玩家跑出检测范围，回到巡逻
        if (distanceToPlayer > detectionRange && patrolPoints.Length > 0)
        {
            currentState = AIState.Move;
            currentTarget = patrolPoints[currentPatrolIndex];
            bool shouldFaceRight = currentTarget.transform.position.x - transform.position.x > 0;
            if (shouldFaceRight != isFacingRight)
            {
                Flip();
            }
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
        transform.Rotate(0, 180, 0);
    }

    // 可视化检测范围（仅在编辑器中显示）
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Destroy(gameObject);
        }
        else
        {
            StartCoroutine(FlashRed()); // 受到伤害但未死亡时触发闪烁效果
        }
    }

    private IEnumerator FlashRed()
    {
        if (sr != null)
        {
            sr.color = Color.red; // 变为红色
            yield return new WaitForSeconds(0.2f); // 等待0.2秒
            sr.color = originalColor; // 恢复原始颜色
        }
    }
}



