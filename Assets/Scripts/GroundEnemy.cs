using UnityEngine;

public class GroundEnemy : Enemy
{
    [Header("Ground Enemy Settings")]
    public float attackDamage = 2f; // �����˺�ֵ
    public float attackDelay = 1f; // �����ӳ�ʱ��(��)
    public BoxCollider2D attackZone; // �������򴥷���

    private float attackTimer = 0f;
    private bool hasDealtDamage = false;

    protected override void Awake()
    {
        base.Awake();

        // ȷ��������������Ϊ������
        if (attackZone != null)
        {
            attackZone.isTrigger = true;
        }
    }

    protected override void HandleAttackState()
    {
        base.HandleAttackState();

        // ������ʱ�߼�
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
        // ��⹥���������Ƿ������
        Collider2D[] hits = Physics2D.OverlapBoxAll(
            attackZone.bounds.center,
            attackZone.size,
            0f);

        foreach (var hit in hits)
        {
            if (hit.CompareTag("Hero"))
            {
                // ���������TakeDamage����
                hit.GetComponent<Hero>().TakeDamage(attackDamage);
                break;
            }
        }
    }

    protected override void CheckStateTransitions()
    {
        base.CheckStateTransitions();

        // ���ù���״̬��־
        if (currentState != AIState.Attack)
        {
            hasDealtDamage = false;
            attackTimer = 0f;
        }
    }

    // ���ӻ���������
    private void OnDrawGizmos()
    {
        if (attackZone != null)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireCube(attackZone.bounds.center, attackZone.bounds.size);
        }
    }
}