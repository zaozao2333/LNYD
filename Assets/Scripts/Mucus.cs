using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class Mucus : MonoBehaviour
{
    // ��̬�ֵ����ÿ��Hero�ļ���Ч������
    private static Dictionary<Hero, int> heroDebuffCount = new Dictionary<Hero, int>();
    // ��̬�ֵ䱣��Hero��ԭʼ���ԣ������α��棩
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
        // ��ʼ������
        if (!heroDebuffCount.ContainsKey(hero))
        {
            heroDebuffCount[hero] = 0;
            // ����һ�α���ԭʼֵ
            heroOriginalValues[hero] = (hero.moveSpeed, hero.acceleration, hero.airControl, hero.canJump);
        }

        heroDebuffCount[hero]++;
        UpdateDebuff(hero); // ���¼���Ч��
    }

    private void RemoveDebuff(Hero hero)
    {
        if (heroDebuffCount.ContainsKey(hero))
        {
            heroDebuffCount[hero]--;
            if (heroDebuffCount[hero] <= 0)
            {
                // �޼���Ч��ʱ�ָ�ԭʼ����
                var original = heroOriginalValues[hero];
                hero.moveSpeed = original.moveSpeed;
                hero.acceleration = original.acceleration;
                hero.airControl = original.airControl;
                hero.canJump = original.canJump;

                // �����ֵ�
                heroDebuffCount.Remove(hero);
                heroOriginalValues.Remove(hero);
            }
            else
            {
                UpdateDebuff(hero); // ����ʣ�����Ч��
            }
        }
    }

    // ���㵱ǰ����Ч����֧�ֶ��Mucus���ӣ�
    private void UpdateDebuff(Hero hero)
    {
        if (!heroOriginalValues.ContainsKey(hero)) return;

        var original = heroOriginalValues[hero];
        float totalSpeedReduction = Mathf.Pow(speedReduction, heroDebuffCount[hero]);

        hero.moveSpeed = original.moveSpeed * totalSpeedReduction;
        hero.acceleration = original.acceleration * totalSpeedReduction;
        hero.airControl = original.airControl * totalSpeedReduction;
        hero.canJump = !disableJump || (heroDebuffCount[hero] == 0); // ��һMucus������Ծ����Ч
    }
}