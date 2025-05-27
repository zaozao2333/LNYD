using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngineInternal;

public class HongBao : MonoBehaviour
{
    BoundsCheck boundsCheck;
    private Renderer rend;
    private Color originalColor;
    private float timer = 0f;
    private bool isFlashing = false;
    private float flashInterval = 1f;
    private bool isExploded = false;
    public float explosionRadius = 5f;
    public int explosionDamage = 50;
    public string enemyTag = "UnderGroundEnemy";
    public GameObject explosionEffect;

    void Awake()
    {
        boundsCheck = GetComponent<BoundsCheck>();
        rend = GetComponent<Renderer>();
        if (rend != null)
        {
            originalColor = rend.material.color;
        }
    }

    void Update()
    {
        if (boundsCheck != null)
        {
            if (!boundsCheck.isOnScreen && !boundsCheck.keepOnScreen)
            {
                Destroy(gameObject);
            }
        }

        timer += Time.deltaTime;

        // 1秒后开始闪烁
        if (timer >= 1f && !isFlashing)
        {
            isFlashing = true;
            StartCoroutine(FlashRed());
        }

        // 3秒后爆炸
        if (timer >= 3f && !isExploded)
        {
            isExploded = true;
            Explode();
        }
    }

    IEnumerator FlashRed()
    {
        while (timer < 3f && rend != null)
        {
            // 随时间加快闪烁速度
            flashInterval = Mathf.Lerp(1f, 0.1f, (timer - 1f) / 2f);

            rend.material.color = Color.red;
            yield return new WaitForSeconds(flashInterval / 2);
            rend.material.color = originalColor;
            yield return new WaitForSeconds(flashInterval / 2);
        }
    }

    void Explode()
    {
        // 创建爆炸特效
        if (explosionEffect != null)
        {
            Instantiate(explosionEffect, transform.position, transform.rotation);
        }

        // 对范围内的敌人造成伤害
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
        foreach (Collider2D hit in colliders)
        {
            if (hit.transform.root.CompareTag(enemyTag))
            {
                hit.GetComponent<Enemy>().TakeDamage(explosionDamage);
            }
        }

        Destroy(gameObject);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}