using UnityEngine;
using System.Collections;

public class SkyEnemy : Enemy
{
    [Header("Sky Enemy Settings")]
    public GameObject flamePrefab; // ����Ԥ����
    public float flameSpawnDelay = 1f; // ���������ӳ�ʱ��
    public float flameFallSpeed = 5f; // ���������ٶ�
    public float flameDamage = 2f; // �����˺�ֵ
    public float flameLifetime = 3f; // �������ʱ��
    private Coroutine attackCoroutine;

    protected override void HandleAttackState()
    {
        // ֹͣ�ƶ�
        rb.velocity = Vector2.zero;

        // ������ڹ����У���ʼ����
        if (!isAttacking && Vector2.Distance(transform.position, player.position) <= attackRange)
        {
            attackCoroutine = StartCoroutine(AttackRoutine());
        }
    }

    private IEnumerator AttackRoutine()
    {
        isAttacking = true;

        // �ȴ��ӳ�ʱ��
        yield return new WaitForSeconds(flameSpawnDelay);

        // ���ɻ���
        SpawnFlame();

        isAttacking = false;
    }

    private void SpawnFlame()
    {
        if (flamePrefab != null)
        {
            // �ڵ����Ϸ����ɻ���
            Vector3 spawnPosition = transform.position + Vector3.up * 2f;
            GameObject flame = Instantiate(flamePrefab, spawnPosition, Quaternion.identity);

            // ���û�������
            FlameProjectile flameScript = flame.GetComponent<FlameProjectile>();
            if (flameScript == null)
            {
                flameScript = flame.AddComponent<FlameProjectile>();
            }

            flameScript.Initialize(flameFallSpeed, flameDamage, flameLifetime);
        }
    }
}