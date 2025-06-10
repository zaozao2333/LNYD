using UnityEngine;

public class FlameProjectile : MonoBehaviour
{
    private float fallSpeed;
    private float damage;
    private float lifetime;
    private Rigidbody2D rb;

    public void Initialize(float speed, float dmg, float life)
    {
        fallSpeed = speed;
        damage = dmg;
        lifetime = life;

        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
        }

        rb.gravityScale = 0; // ���������������Լ���������
        rb.velocity = Vector2.down * fallSpeed;

        // �����Զ�����
        Destroy(gameObject, lifetime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // ���������ң�����˺�
        if (other.CompareTag("Hero"))
        {
            // ����Ӧ�õ�����ҵ����˷���
            // ����: other.GetComponent<PlayerHealth>().TakeDamage(damage);
            other.GetComponent<Hero>().TakeDamage(damage);
            Destroy(gameObject);
        }
        // ����������棬���ٻ���
        else if (other.CompareTag("Ground"))
        {
            Destroy(gameObject);
        }
    }
}