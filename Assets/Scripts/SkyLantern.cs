using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SkyLantern : MonoBehaviour
{
    BoundsCheck boundsCheck;
    [SerializeField] private int damage = 10; // 可配置的伤害值
    [SerializeField] private string enemyTag = "SkyEnemy"; // 可配置的目标tag
    [SerializeField] private float damageInterval = 0.5f; // 伤害间隔时间

    [SerializeField] private float swingSpeed = 1.0f; // 摆动速度
    [SerializeField] private float swingAmount = 0.5f; // 摆动幅度

    private HashSet<Enemy> damagedEnemies = new HashSet<Enemy>();
    private Vector3 initialPosition; // 初始位置
    private float spawnTime; // 记录生成时间

    void Awake()
    {
        boundsCheck = GetComponent<BoundsCheck>();
        initialPosition = transform.position;
        spawnTime = Time.time; // 初始化独立计时器
    }

    void Update()
    {
        if (boundsCheck != null && !boundsCheck.isOnScreen && !boundsCheck.keepOnScreen)
        {
            Destroy(gameObject);
        }

        // 正弦曲线摆动效果
        float swingOffset = Mathf.Sin((Time.time - spawnTime) * swingSpeed) * swingAmount;
        transform.position = new Vector3(
            initialPosition.x + swingOffset, // 始终基于initialPosition.x
            transform.position.y,            // 保持原有y轴（上升逻辑）
            transform.position.z             // 保持原有z轴
        );
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