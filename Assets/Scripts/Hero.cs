using UnityEngine;
using System.Collections;

public class Hero : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 8f;
    public float acceleration = 15f;
    public float airControl = 0.6f;

    [Header("Jump")]
    public float jumpForce = 12f;
    public float jumpTimeThreshold = 0.1f;
    public float fallMultiplier = 2.5f;
    public float lowJumpMultiplier = 2f;
    public LayerMask groundLayer;
    public bool canJump = true;

    [Header("Setting")]
    public float health = 100f;

    private Rigidbody2D rb;
    private Transform groundCheck;
    public bool isGrounded;
    private float jumpTimeCounter;
    private bool isJumping;
    private float horizontalInput;
    private SpriteRenderer sr; // ����������������Ⱦ��
    private Color originalColor; // �洢ԭʼ��ɫ

    [Header("Weapons")]
    public Weapon[] weapons;
    public int currentWeaponIndex = 0;

    [Header("Animation")]
    public Animator animator; // ���Animator���������

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        groundCheck = transform.Find("GroundCheck");
        animator = GetComponentInChildren<Animator>();
        SwitchWeapon(0);
        Transform child = transform.Find("Animator");
        sr = child.GetComponent<SpriteRenderer>(); // ��ȡSpriteRenderer���
        if (sr != null)
        {
            originalColor = sr.color; // �洢ԭʼ��ɫ
        }
    }

    void Update()
    {
        // �����⣨Բ�μ��뾶0.2f��
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);

        // ��Ծ������
        if (Input.GetButtonDown("Jump") && isGrounded && canJump)
        {
            isJumping = true;
            jumpTimeCounter = jumpTimeThreshold;
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }

        // ������Ծ����
        if (Input.GetButton("Jump") && isJumping)
        {
            if (jumpTimeCounter > 0)
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
                jumpTimeCounter -= Time.deltaTime;
            }
            else
            {
                isJumping = false;
            }
        }

        if (Input.GetButtonUp("Jump"))
        {
            isJumping = false;
        }

        // ��Ծ�����Ż�
        if (rb.velocity.y < 0)
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }
        else if (rb.velocity.y > 0 && !Input.GetButton("Jump"))
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
        }

        horizontalInput = Input.GetAxisRaw("Horizontal");

        // ���¶�������
        if (animator != null)
        {
            animator.SetBool("isMoving", Mathf.Abs(horizontalInput) > 0.1f);
        }

        // �����л�
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SwitchWeapon(0);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SwitchWeapon(1);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SwitchWeapon(2);
        }

        // ����
        if (Input.GetButtonDown("Fire1"))
        {
            weapons[currentWeaponIndex].Fire();
        }
    }

    void FixedUpdate()
    {
        // �ƶ����ƣ������м��٣�
        float targetSpeed = horizontalInput * moveSpeed;
        float currentSpeed = rb.velocity.x;
        float controlFactor = isGrounded ? 1f : airControl;

        float speedDiff = targetSpeed - currentSpeed;
        float movement = speedDiff * acceleration * controlFactor;

        rb.AddForce(Vector2.right * movement);

        // ��ɫ��ת
        if (horizontalInput != 0)
        {
            transform.localScale = new Vector3(Mathf.Sign(horizontalInput), 1, 1);
        }
    }

    private void SwitchWeapon(int index)
    {
        if (index < 0 || index >= weapons.Length) return;

        // ������������ģ��
        foreach (var weapon in weapons)
        {
            weapon.gameObject.SetActive(false);
        }

        currentWeaponIndex = index;
        weapons[currentWeaponIndex].gameObject.SetActive(true); // ��ʾ��ǰ����
        Debug.Log("Switched to weapon: " + weapons[currentWeaponIndex].type);
    }

    private void OnDrawGizmos()
    {
        if(!isGrounded) return;
        Gizmos.color = Color.red; // ������ɫ
        Gizmos.DrawWireSphere(groundCheck.position, 0.2f); // ���ư뾶Ϊ0.2f��Բ��
    }

    public void TakeDamage(float damage)
    {
        AudioManager.PlayOneShotStatic("Hit");
        health -= damage;
        if (health <= 0)
        {
            EnemySpawner.instance.GameOver();
        }
        else
        {
            StartCoroutine(FlashRed()); // �ܵ��˺���δ����ʱ������˸Ч��
        }
    }

    private IEnumerator FlashRed()
    {
        if (sr != null)
        {
            sr.color = Color.red; // ��Ϊ��ɫ
            yield return new WaitForSeconds(0.2f); // �ȴ�0.2��
            sr.color = originalColor; // �ָ�ԭʼ��ɫ
        }
    }


}