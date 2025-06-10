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

        rb.gravityScale = 0; // 禁用重力，我们自己控制下落
        rb.velocity = Vector2.down * fallSpeed;

        // 设置自动销毁
        Destroy(gameObject, lifetime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // 如果碰到玩家，造成伤害
        if (other.CompareTag("Hero"))
        {
            // 这里应该调用玩家的受伤方法
            // 例如: other.GetComponent<PlayerHealth>().TakeDamage(damage);
            other.GetComponent<Hero>().TakeDamage(damage);
            Destroy(gameObject);
        }
        // 如果碰到地面，销毁火焰
        else if (other.CompareTag("Ground"))
        {
            Destroy(gameObject);
        }
    }
}