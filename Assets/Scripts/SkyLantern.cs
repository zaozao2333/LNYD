using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SkyLantern : MonoBehaviour
{
    BoundsCheck boundsCheck;
    [SerializeField] private int damage = 10; // �����õ��˺�ֵ
    [SerializeField] private string enemyTag = "SkyEnemy"; // �����õ�Ŀ��tag
    [SerializeField] private float damageInterval = 0.5f; // �˺����ʱ��

    private HashSet<Enemy> damagedEnemies = new HashSet<Enemy>(); // ��¼�����ܵ������˺��ĵ���

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
        // ����������ڽ��е��˺�Э��
        foreach (var enemy in damagedEnemies)
        {
            if (enemy != null)
            {
                enemy.StopAllCoroutines();
            }
        }
    }
}