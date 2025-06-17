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
        // �ָ���ת������
        currentRecoil = Vector2.Lerp(currentRecoil, Vector2.zero, recoilSettings.recoverySpeed * Time.deltaTime);
        transform.localRotation = Quaternion.Euler(-currentRecoil.y, 0, -currentRecoil.x);

        // �ָ�λ��
        transform.localPosition = Vector3.Lerp(transform.localPosition, originalPosition, recoilSettings.positionRecovery * Time.deltaTime);
    }

    public void Fire()
    {
        if (Time.time < nextFireTime) return;

        nextFireTime = Time.time + fireRate;

        // �����ת������
        currentRecoil.x += Random.Range(-recoilSettings.horizontalRecoil, 0);
        currentRecoil.y += recoilSettings.verticalRecoil;

        // ���λ�ú�����
        transform.localPosition -= (Vector3)transform.right * recoilSettings.positionOffset * Mathf.Sign(transform.root.localScale.x);

        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();

        // ���������������ò�ͬ�ĵ���
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