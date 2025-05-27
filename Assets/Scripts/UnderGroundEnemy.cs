using UnityEngine;

public class UnderGroundEnemy : Enemy
{
    [Header("Bullet Settings")]
    public GameObject bulletPrefab; // 子弹预制体
    public float bulletSpeed = 5f; // 子弹速度
    public float shootInterval = 2f; // 射击间隔
    private float lastShootTime; // 上次射击时间

    protected override void HandleMoveState()
    {
        base.HandleMoveState(); // 保持原有移动逻辑

        // 添加射击逻辑
        if (Time.time - lastShootTime >= shootInterval)
        {
            lastShootTime = Time.time;
            ShootUpwards();
        }
    }

    private void ShootUpwards()
    {
        if (bulletPrefab != null)
        {
            GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
            Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();

            // 向上发射子弹（Y轴正方向）
            if (bulletRb != null)
            {
                Vector2 shootDirection = Vector2.up;
                bulletRb.velocity = shootDirection * bulletSpeed;
            }
        }
    }
}