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
            if (!boundsCheck.isOnScreen)
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
            AudioManager.Play("Explosion");
            Enemy enemy = root.GetComponent<Enemy>();
            if (enemy != null) enemy.TakeDamage(damage);
            Destroy(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}