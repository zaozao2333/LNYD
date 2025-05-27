using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mucus : MonoBehaviour
{
    [Header("Debuff Settings")]
    public float speedReduction = 0.5f; // 移动速度减少比例（0.5表示减半）
    public bool disableJump = true;    // 是否禁用跳跃
    public float disappearTime = 3f;  // 3秒后消失

    // 新增：存储原始值
    private Dictionary<Hero, (float moveSpeed, float acceleration, float airControl, bool canJump)> originalValues = new();

    private void Start()
    {
        // 启动协程，3秒后销毁自身
        StartCoroutine(DestroyAfterTime());
    }

    IEnumerator DestroyAfterTime()
    {
        yield return new WaitForSeconds(disappearTime);

        // 在销毁前恢复所有英雄的原始属性
        foreach (var heroEntry in originalValues)
        {
            Hero hero = heroEntry.Key;
            var values = heroEntry.Value;
            hero.moveSpeed = values.moveSpeed;
            hero.acceleration = values.acceleration;
            hero.airControl = values.airControl;
        }

        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Hero hero = other.transform.root.GetComponent<Hero>();
        if (hero != null)
        {
            ApplyDebuff(hero);
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        print(other.transform.root.tag);
        Hero hero = other.transform.root.GetComponent<Hero>();
        if (hero != null)
        {
            ApplyDebuff(hero);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        Hero hero = other.transform.root.GetComponent<Hero>();
        if (hero != null)
        {
            RemoveDebuff(hero);
        }
    }

    private void ApplyDebuff(Hero hero)
    {
        // 首次触发时保存原始值
        if (!originalValues.ContainsKey(hero))
        {
            originalValues[hero] = (hero.moveSpeed, hero.acceleration, hero.airControl, hero.canJump);
        }

        // 应用减速
        hero.moveSpeed = originalValues[hero].moveSpeed * speedReduction;
        hero.acceleration = originalValues[hero].acceleration * speedReduction;
        hero.airControl = originalValues[hero].airControl * speedReduction;

        if (disableJump) hero.canJump = false;
    }

    private void RemoveDebuff(Hero hero)
    {
        if (originalValues.TryGetValue(hero, out var values))
        {
            // 恢复原始值
            hero.moveSpeed = values.moveSpeed;
            hero.acceleration = values.acceleration;
            hero.airControl = values.airControl;
            hero.canJump = values.canJump; // 恢复跳跃能力
            originalValues.Remove(hero);
        }
    }
}