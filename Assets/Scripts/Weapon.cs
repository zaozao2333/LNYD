using UnityEngine;

public enum WeaponType
{
    SkyLanternGun,    // 向上飞的武器
    FirecrackerRepeater,  // 平飞的武器
    HongbaoBomb   // 向下掉的武器
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

        // 根据武器类型设置不同的弹道
        switch (type)
        {
            case WeaponType.SkyLanternGun:
                rb.velocity = transform.up * bulletSpeed;
                break;
            case WeaponType.FirecrackerRepeater:
                rb.velocity = transform.right * bulletSpeed * Mathf.Sign(transform.root.localScale.x);
                break;
            case WeaponType.HongbaoBomb:
                rb.velocity = -transform.up * bulletSpeed;
                break;
        }
    }
}