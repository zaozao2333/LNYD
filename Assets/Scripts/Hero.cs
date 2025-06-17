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
    private SpriteRenderer sr; // 新增变量：精灵渲染器
    private Color originalColor; // 存储原始颜色

    [Header("Weapons")]
    public Weapon[] weapons;
    public int currentWeaponIndex = 0;

    [Header("Animation")]
    public Animator animator; // 添加Animator组件的引用

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        groundCheck = transform.Find("GroundCheck");
        animator = GetComponentInChildren<Animator>();
        SwitchWeapon(0);
        Transform child = transform.Find("Animator");
        sr = child.GetComponent<SpriteRenderer>(); // 获取SpriteRenderer组件
        if (sr != null)
        {
            originalColor = sr.color; // 存储原始颜色
        }
    }

    void Update()
    {
        // 地面检测（圆形检测半径0.2f）
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);

        // 跳跃输入检测
        if (Input.GetButtonDown("Jump") && isGrounded && canJump)
        {
            isJumping = true;
            jumpTimeCounter = jumpTimeThreshold;
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }

        // 长按跳跃更高
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

        // 跳跃物理优化
        if (rb.velocity.y < 0)
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }
        else if (rb.velocity.y > 0 && !Input.GetButton("Jump"))
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
        }

        horizontalInput = Input.GetAxisRaw("Horizontal");

        // 更新动画参数
        if (animator != null)
        {
            animator.SetBool("isMoving", Mathf.Abs(horizontalInput) > 0.1f);
        }

        // 武器切换
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

        // 开火
        if (Input.GetButtonDown("Fire1"))
        {
            weapons[currentWeaponIndex].Fire();
        }
    }

    void FixedUpdate()
    {
        // 移动控制（带空中减速）
        float targetSpeed = horizontalInput * moveSpeed;
        float currentSpeed = rb.velocity.x;
        float controlFactor = isGrounded ? 1f : airControl;

        float speedDiff = targetSpeed - currentSpeed;
        float movement = speedDiff * acceleration * controlFactor;

        rb.AddForce(Vector2.right * movement);

        // 角色翻转
        if (horizontalInput != 0)
        {
            transform.localScale = new Vector3(Mathf.Sign(horizontalInput), 1, 1);
        }
    }

    private void SwitchWeapon(int index)
    {
        if (index < 0 || index >= weapons.Length) return;

        // 隐藏所有武器模型
        foreach (var weapon in weapons)
        {
            weapon.gameObject.SetActive(false);
        }

        currentWeaponIndex = index;
        weapons[currentWeaponIndex].gameObject.SetActive(true); // 显示当前武器
        Debug.Log("Switched to weapon: " + weapons[currentWeaponIndex].type);
    }

    private void OnDrawGizmos()
    {
        if(!isGrounded) return;
        Gizmos.color = Color.red; // 设置颜色
        Gizmos.DrawWireSphere(groundCheck.position, 0.2f); // 绘制半径为0.2f的圆形
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
            StartCoroutine(FlashRed()); // 受到伤害但未死亡时触发闪烁效果
        }
    }

    private IEnumerator FlashRed()
    {
        if (sr != null)
        {
            sr.color = Color.red; // 变为红色
            yield return new WaitForSeconds(0.2f); // 等待0.2秒
            sr.color = originalColor; // 恢复原始颜色
        }
    }


}