using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    // ״̬ö��
    public enum AIState
    {
        Move,    // �ƶ�״̬������Ѳ�ߺ�׷����
        Attack   // ����״̬
    }

    [Header("Settings")]
    public float patrolSpeed = 6f;  // Ѳ���ٶ��Ե���Hero��8f
    public float chaseSpeed = 8f;   // ׷���ٶ���Hero��ͬ
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

        // ��ʼ״̬Ϊ�ƶ���Ѳ�ߣ�
        currentState = AIState.Move;
        if (patrolPoints.Length > 0)
        {
            currentTarget = patrolPoints[0];
        }
    }

    void Update()
    {
        // ״̬�������߼�
        switch (currentState)
        {
            case AIState.Move:
                HandleMoveState();
                break;

            case AIState.Attack:
                HandleAttackState();
                break;
        }

        // ״̬ת�����
        CheckStateTransitions();
    }

    void HandleMoveState()
    {
        // Ѳ���߼�
        if (patrolPoints.Length > 0)
        {
            Vector2 direction = (currentTarget.position - transform.position).normalized;
            float targetSpeed = (currentTarget == player) ? chaseSpeed : patrolSpeed;
            rb.velocity = new Vector2(direction.x * targetSpeed, rb.velocity.y);

            // ����Ѳ�ߵ���л���һ����
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
        // ֹͣ�ƶ����й���
        rb.velocity = Vector2.zero;

        // ʵ�ʹ����߼���ʾ�������˺����㣩
        if (Vector2.Distance(transform.position, player.position) <= attackRange)
        {
            Debug.Log("Enemy attacking player!");
            // �������ʵ�ʹ����߼����������ҵ����˷���
        }
    }

    void CheckStateTransitions()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        // ���ƶ��л�������������
        if (currentState == AIState.Move && distanceToPlayer <= attackRange)
        {
            currentState = AIState.Attack;
            return;
        }

        // �ӹ����л����ƶ�������
        if (currentState == AIState.Attack && distanceToPlayer > attackRange)
        {
            currentState = AIState.Move;
        }

        // �������ܳ���ⷶΧ���ص�Ѳ��
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
        // �����ƶ�����ת����
        isFacingRight = !isFacingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    // ���ӻ���ⷶΧ�����ڱ༭������ʾ��
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}



