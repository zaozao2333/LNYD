using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class Mucus : MonoBehaviour
{
    // 静态字典跟踪每个Hero的减速效果计数
    private static Dictionary<Hero, int> heroDebuffCount = new Dictionary<Hero, int>();
    // 静态字典保存Hero的原始属性（避免多次保存）
    private static Dictionary<Hero, (float moveSpeed, float acceleration, float airControl, bool canJump)> heroOriginalValues = new Dictionary<Hero, (float, float, float, bool)>();

    [Header("Debuff Settings")]
    public float speedReduction = 0.5f;
    public bool disableJump = true;
    public float disappearTime = 3f;

    private void Start()
    {
        StartCoroutine(DestroyAfterTime());
    }

    IEnumerator DestroyAfterTime()
    {
        yield return new WaitForSeconds(disappearTime);
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
        // 初始化计数
        if (!heroDebuffCount.ContainsKey(hero))
        {
            heroDebuffCount[hero] = 0;
            // 仅第一次保存原始值
            heroOriginalValues[hero] = (hero.moveSpeed, hero.acceleration, hero.airControl, hero.canJump);
        }

        heroDebuffCount[hero]++;
        UpdateDebuff(hero); // 更新减速效果
    }

    private void RemoveDebuff(Hero hero)
    {
        if (heroDebuffCount.ContainsKey(hero))
        {
            heroDebuffCount[hero]--;
            if (heroDebuffCount[hero] <= 0)
            {
                // 无减速效果时恢复原始属性
                var original = heroOriginalValues[hero];
                hero.moveSpeed = original.moveSpeed;
                hero.acceleration = original.acceleration;
                hero.airControl = original.airControl;
                hero.canJump = original.canJump;

                // 清理字典
                heroDebuffCount.Remove(hero);
                heroOriginalValues.Remove(hero);
            }
            else
            {
                UpdateDebuff(hero); // 更新剩余减速效果
            }
        }
    }

    // 计算当前减速效果（支持多个Mucus叠加）
    private void UpdateDebuff(Hero hero)
    {
        if (!heroOriginalValues.ContainsKey(hero)) return;

        var original = heroOriginalValues[hero];
        float totalSpeedReduction = Mathf.Pow(speedReduction, heroDebuffCount[hero]);

        hero.moveSpeed = original.moveSpeed * totalSpeedReduction;
        hero.acceleration = original.acceleration * totalSpeedReduction;
        hero.airControl = original.airControl * totalSpeedReduction;
        hero.canJump = !disableJump || (heroDebuffCount[hero] == 0); // 任一Mucus禁用跳跃则生效
    }
}