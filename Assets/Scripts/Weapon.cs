using UnityEngine;

public enum WeaponType
{
    SkyLanternGun,    // ���Ϸɵ�����
    FirecrackerRepeater,  // ƽ�ɵ�����
    HongbaoBomb   // ���µ�������
}

[System.Serializable]
public class Weapon : MonoBehaviour
{
    public WeaponType type;
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float fireRate = 0.5f;
    public float bulletSpeed = 10f;

    private float nextFireTime = 0f;

    public void Fire()
    {
        if (Time.time < nextFireTime) return;

        nextFireTime = Time.time + fireRate;

        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();

        // ���������������ò�ͬ�ĵ���
        switch (type)
        {
            case WeaponType.SkyLanternGun:
                rb.velocity = (Vector2)transform.up * bulletSpeed + transform.root.GetComponent<Rigidbody2D>().velocity * 0.2f;
                break;
            case WeaponType.FirecrackerRepeater:
            case WeaponType.HongbaoBomb:
                rb.velocity = (Vector2)transform.right * bulletSpeed * Mathf.Sign(transform.root.localScale.x) + transform.root.GetComponent<Rigidbody2D>().velocity * 0.5f ;
                break;
        }
    }
}