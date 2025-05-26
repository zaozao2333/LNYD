using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SkyLantern : MonoBehaviour
{
    BoundsCheck boundsCheck;
    [SerializeField] private int damage = 10; // 可配置的伤害值
    [SerializeField] private string enemyTag = "SkyEnemy"; // 可配置的目标tag
    [SerializeField] private float damageInterval = 0.5f; // 伤害间隔时间

    private HashSet<Enemy> damagedEnemies = new HashSet<Enemy>(); // 记录正在受到持续伤害的敌人

    void Start()
    {
        boundsCheck = GetComponent<BoundsCheck>();
    }

    void Update()
    {
        if (boundsCheck != null && !boundsCheck.isOnScreen && !boundsCheck.keepOnScreen)
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        GameObject root = other.transform.root.gameObject;
        if (root.CompareTag(enemyTag))
        {
            Enemy enemy = root.GetComponent<Enemy>();
            if (enemy != null && !damagedEnemies.Contains(enemy))
            {
                damagedEnemies.Add(enemy);
                StartCoroutine(ApplyDamageOverTime(enemy));
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        GameObject root = other.transform.root.gameObject;
        if (root.CompareTag(enemyTag))
        {
            Enemy enemy = root.GetComponent<Enemy>();
            if (enemy != null && damagedEnemies.Contains(enemy))
            {
                damagedEnemies.Remove(enemy);
            }
        }
    }

    IEnumerator ApplyDamageOverTime(Enemy enemy)
    {
        while (damagedEnemies.Contains(enemy))
        {
            enemy.TakeDamage(damage);
            yield return new WaitForSeconds(damageInterval);
        }
    }

    void OnDestroy()
    {
        // 清除所有正在进行的伤害协程
        foreach (var enemy in damagedEnemies)
        {
            if (enemy != null)
            {
                enemy.StopAllCoroutines();
            }
        }
    }
}