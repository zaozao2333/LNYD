using UnityEngine;
using System.Collections;
public class Enemy : MonoBehaviour
{
    // ״̬ö��
    public enum AIState
    {
        Move,    // �ƶ�״̬������Ѳ�ߺ�׷����
        Attack   // ����״̬
    }

    [Header("Settings")]
    public float health = 10f;
    public float patrolSpeed = 6f;  // Ѳ���ٶ��Ե���Hero��8f
    public float chaseSpeed = 8f;   // ׷���ٶ���Hero��ͬ
    public float attackRange = 1.5f;
    public float detectionRange = 5f;
    public Transform[] patrolPoints;
    public string patrolPointsParentName = "PatrolPoints"; // Ѳ�ߵ㸸��������

    [Header("Debug")]
    public AIState currentState = AIState.Move;
    public Transform currentTarget;

    protected Rigidbody2D rb;
    protected Transform player;
    private int currentPatrolIndex = 0;
    private bool isFacingRight;
    protected bool isAttacking = false;
    private SpriteRenderer sr; // ����������������Ⱦ��
    private Color originalColor; // �洢ԭʼ��ɫ

    virtual protected void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Hero").transform;
        sr = GetComponentInChildren<SpriteRenderer>(); // ��ȡSpriteRenderer���
        if (sr != null)
        {
            originalColor = sr.color; // �洢ԭʼ��ɫ
        }

        // ��ʼ״̬Ϊ�ƶ���Ѳ�ߣ�
        currentState = AIState.Move;

        // ͨ�����Ʋ���Ѳ�ߵ㸸����
        GameObject patrolParent = GameObject.Find(patrolPointsParentName);
        if (patrolParent != null)
        {
            // ��ȡ�����Ӷ�����ΪѲ�ߵ�
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

    protected virtual void HandleMoveState()
    {
        // Ѳ���߼�
        if (patrolPoints.Length > 0)
        {
            Vector2 direction = (currentTarget.position - transform.position).normalized;
            float targetSpeed = (currentTarget == player) ? chaseSpeed : patrolSpeed;
            rb.velocity = new Vector2(direction.x * targetSpeed, rb.velocity.y);

            // ������׷�����ʱʵʱ��⳯��
            if (currentTarget == player)
            {
                bool shouldFaceRight = direction.x > 0;
                if (shouldFaceRight != isFacingRight)
                {
                    Flip();
                }
            }

            // ����Ѳ�ߵ���л���һ����
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
        // ֹͣ�ƶ����й���
        rb.velocity = Vector2.zero;

        // ʵ�ʹ����߼���ʾ�������˺����㣩
        if (Vector2.Distance(transform.position, player.position) <= attackRange)
        {
            // �������ʵ�ʹ����߼����������ҵ����˷���
        }
    }

    virtual protected void CheckStateTransitions()
    {
        if (isAttacking) return;
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        // ���ƶ��л�������������
        if (currentState == AIState.Move && distanceToPlayer <= attackRange)
        {
            currentState = AIState.Attack;
            return;
        }

        // �ӹ����л����ƶ�������
        if (currentState == AIState.Attack && distanceToPlayer > attackRange && !isAttacking)
        {
            currentState = AIState.Move;
        }

        // �������ܳ���ⷶΧ���ص�Ѳ��
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
        // �����ƶ�����ת����
        isFacingRight = !isFacingRight;
        transform.Rotate(0, 180, 0);
    }

    // ���ӻ���ⷶΧ�����ڱ༭������ʾ��
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
            StartCoroutine(FlashRed()); // �ܵ��˺���δ����ʱ������˸Ч��
        }
    }

    private IEnumerator FlashRed()
    {
        if (sr != null)
        {
            sr.color = Color.red; // ��Ϊ��ɫ
            yield return new WaitForSeconds(0.2f); // �ȴ�0.2��
            sr.color = originalColor; // �ָ�ԭʼ��ɫ
        }
    }
}



