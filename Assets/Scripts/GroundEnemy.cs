using UnityEngine;

public class GroundEnemy : Enemy
{
    [Header("Ground Enemy Settings")]
    public float attackDamage = 2f; // 攻击伤害值
    public float attackDelay = 1f; // 攻击延迟时间(秒)
    public BoxCollider2D attackZone; // 攻击区域触发器

    private float attackTimer = 0f;
    private bool hasDealtDamage = false;

    protected override void Awake()
    {
        base.Awake();

        // 确保攻击区域设置为触发器
        if (attackZone != null)
        {
            attackZone.isTrigger = true;
        }
    }

    protected override void HandleAttackState()
    {
        base.HandleAttackState();

        // 攻击计时逻辑
        if (!hasDealtDamage)
        {
            isAttacking = true;
            attackTimer += Time.deltaTime;

            if (attackTimer >= attackDelay)
            {
                DealDamageToPlayer();
                hasDealtDamage = true;
                attackTimer = 0f;
                isAttacking = false;
            }
        }
        else
        {
            hasDealtDamage = false;
        }
    }

    void DealDamageToPlayer()
    {
        // 检测攻击区域内是否有玩家
        Collider2D[] hits = Physics2D.OverlapBoxAll(
            attackZone.bounds.center,
            attackZone.size,
            0f);

        foreach (var hit in hits)
        {
            if (hit.CompareTag("Hero"))
            {
                // 假设玩家有TakeDamage方法
                hit.GetComponent<Hero>().TakeDamage(attackDamage);
                break;
            }
        }
    }

    protected override void CheckStateTransitions()
    {
        base.CheckStateTransitions();

        // 重置攻击状态标志
        if (currentState != AIState.Attack)
        {
            hasDealtDamage = false;
            attackTimer = 0f;
        }
    }

    // 可视化攻击区域
    private void OnDrawGizmos()
    {
        if (attackZone != null)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireCube(attackZone.bounds.center, attackZone.bounds.size);
        }
    }
}