using UnityEngine;

public class Bullet : MonoBehaviour
{
    BoundsCheck boundsCheck;
    [SerializeField] private int damage = 10; // �����õ��˺�ֵ
    [SerializeField] private string enemyTag = "Enemy"; // �����õ�Ŀ��tag

    void Start()
    {
        boundsCheck = GetComponent<BoundsCheck>();
    }

    // Update is called once per frame
    void Update()
    {
        if (boundsCheck != null)
        {
            if (!boundsCheck.isOnScreen && !boundsCheck.keepOnScreen)
            {
                Destroy(gameObject);
            }
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        GameObject root = collision.gameObject.transform.root.gameObject;
        if (root.CompareTag(enemyTag))
        {
            Enemy enemy = root.GetComponent<Enemy>();
            if (enemy != null) enemy.TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}