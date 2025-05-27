using UnityEngine;

public class UnderGroundEnemy : Enemy
{
    [Header("Bullet Settings")]
    public GameObject bulletPrefab; // �ӵ�Ԥ����
    public float bulletSpeed = 5f; // �ӵ��ٶ�
    public float shootInterval = 2f; // ������
    private float lastShootTime; // �ϴ����ʱ��

    protected override void HandleMoveState()
    {
        base.HandleMoveState(); // ����ԭ���ƶ��߼�

        // �������߼�
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

            // ���Ϸ����ӵ���Y��������
            if (bulletRb != null)
            {
                Vector2 shootDirection = Vector2.up;
                bulletRb.velocity = shootDirection * bulletSpeed;
            }
        }
    }
}