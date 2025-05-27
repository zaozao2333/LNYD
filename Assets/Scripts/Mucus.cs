using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mucus : MonoBehaviour
{
    [Header("Debuff Settings")]
    public float speedReduction = 0.5f; // �ƶ��ٶȼ��ٱ�����0.5��ʾ���룩
    public bool disableJump = true;    // �Ƿ������Ծ
    public float disappearTime = 3f;  // 3�����ʧ

    // �������洢ԭʼֵ
    private Dictionary<Hero, (float moveSpeed, float acceleration, float airControl, bool canJump)> originalValues = new();

    private void Start()
    {
        // ����Э�̣�3�����������
        StartCoroutine(DestroyAfterTime());
    }

    IEnumerator DestroyAfterTime()
    {
        yield return new WaitForSeconds(disappearTime);

        // ������ǰ�ָ�����Ӣ�۵�ԭʼ����
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
        // �״δ���ʱ����ԭʼֵ
        if (!originalValues.ContainsKey(hero))
        {
            originalValues[hero] = (hero.moveSpeed, hero.acceleration, hero.airControl, hero.canJump);
        }

        // Ӧ�ü���
        hero.moveSpeed = originalValues[hero].moveSpeed * speedReduction;
        hero.acceleration = originalValues[hero].acceleration * speedReduction;
        hero.airControl = originalValues[hero].airControl * speedReduction;

        if (disableJump) hero.canJump = false;
    }

    private void RemoveDebuff(Hero hero)
    {
        if (originalValues.TryGetValue(hero, out var values))
        {
            // �ָ�ԭʼֵ
            hero.moveSpeed = values.moveSpeed;
            hero.acceleration = values.acceleration;
            hero.airControl = values.airControl;
            hero.canJump = values.canJump; // �ָ���Ծ����
            originalValues.Remove(hero);
        }
    }
}