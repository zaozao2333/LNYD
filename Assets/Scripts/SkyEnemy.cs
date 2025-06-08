using UnityEngine;
using System.Collections;

public class SkyEnemy : Enemy
{
    [Header("Sky Enemy Settings")]
    public GameObject flamePrefab; // 火焰预制体
    public float flameSpawnDelay = 1f; // 火焰生成延迟时间
    public float flameFallSpeed = 5f; // 火焰下落速度
    public float flameDamage = 2f; // 火焰伤害值
    public float flameLifetime = 3f; // 火焰存在时间
    private Coroutine attackCoroutine;

    protected override void HandleAttackState()
    {
        // 停止移动
        rb.velocity = Vector2.zero;

        // 如果不在攻击中，开始攻击
        if (!isAttacking && Vector2.Distance(transform.position, player.position) <= attackRange)
        {
            attackCoroutine = StartCoroutine(AttackRoutine());
        }
    }

    private IEnumerator AttackRoutine()
    {
        isAttacking = true;

        // 等待延迟时间
        yield return new WaitForSeconds(flameSpawnDelay);

        // 生成火焰
        SpawnFlame();

        isAttacking = false;
    }

    private void SpawnFlame()
    {
        if (flamePrefab != null)
        {
            // 在敌人上方生成火焰
            Vector3 spawnPosition = transform.position + Vector3.up * 2f;
            GameObject flame = Instantiate(flamePrefab, spawnPosition, Quaternion.identity);

            // 设置火焰属性
            FlameProjectile flameScript = flame.GetComponent<FlameProjectile>();
            if (flameScript == null)
            {
                flameScript = flame.AddComponent<FlameProjectile>();
            }

            flameScript.Initialize(flameFallSpeed, flameDamage, flameLifetime);
        }
    }
}