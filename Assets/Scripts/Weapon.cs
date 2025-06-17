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

    [System.Serializable]
    public class RecoilSettings
    {
        public float verticalRecoil = 5f;
        public float horizontalRecoil = 1f;
        public float recoverySpeed = 3f;
        public float positionOffset = 0.05f;
        public float positionRecovery = 8f;
    }

    public RecoilSettings recoilSettings;
    private Vector2 currentRecoil;
    private Vector3 originalPosition;

    private void Start()
    {
        originalPosition = transform.localPosition;
    }

    private void Update()
    {
        // 恢复旋转后坐力
        currentRecoil = Vector2.Lerp(currentRecoil, Vector2.zero, recoilSettings.recoverySpeed * Time.deltaTime);
        transform.localRotation = Quaternion.Euler(-currentRecoil.y, 0, -currentRecoil.x);

        // 恢复位置
        transform.localPosition = Vector3.Lerp(transform.localPosition, originalPosition, recoilSettings.positionRecovery * Time.deltaTime);
    }

    public void Fire()
    {
        if (Time.time < nextFireTime) return;

        nextFireTime = Time.time + fireRate;

        // 添加旋转后坐力
        currentRecoil.x += Random.Range(-recoilSettings.horizontalRecoil, 0);
        currentRecoil.y += recoilSettings.verticalRecoil;

        // 添加位置后坐力
        transform.localPosition -= (Vector3)transform.right * recoilSettings.positionOffset * Mathf.Sign(transform.root.localScale.x);

        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();

        // 根据武器类型设置不同的弹道
        switch (type)
        {
            case WeaponType.SkyLanternGun:
                AudioManager.PlayOneShotStatic("Lantern");
                rb.velocity = (Vector2)transform.up * bulletSpeed + transform.root.GetComponent<Rigidbody2D>().velocity * 0.2f;
                break;
            case WeaponType.FirecrackerRepeater:
                AudioManager.Play("Firework");
                bullet.transform.right = transform.right * Mathf.Sign(transform.root.localScale.x);
                rb.velocity = (Vector2)transform.right * bulletSpeed * Mathf.Sign(transform.root.localScale.x) + transform.root.GetComponent<Rigidbody2D>().velocity * 0.5f;
                break;
            case WeaponType.HongbaoBomb:
                rb.velocity = (Vector2)transform.right * bulletSpeed * Mathf.Sign(transform.root.localScale.x) + transform.root.GetComponent<Rigidbody2D>().velocity * 0.5f ;
                break;
        }
    }
}