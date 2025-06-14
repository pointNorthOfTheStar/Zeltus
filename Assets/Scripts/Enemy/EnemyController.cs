using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Header("基础属性")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float attackRange = 1.5f;
    [SerializeField] private float attackDamage = 10f;
    
    [Header("AI设置")]
    [SerializeField] private float detectionRange = 5f;
    [SerializeField] private float attackCooldown = 1f;
    
    private float currentHealth;
    private Transform player;
    private Rigidbody2D rb;
    private Animator animator;
    private float lastAttackTime;
    private bool isFacingRight = true;
    private bool isStunned;  // 添加硬直状态标志
    
    private void Start()
    {
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }
    
    private void Update()
    {
        if (player == null || isStunned) return;  // 如果处于硬直状态，不执行AI逻辑
        
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        
        if (distanceToPlayer <= detectionRange)
        {
            // 追踪玩家
            Vector2 direction = (player.position - transform.position).normalized;
            rb.velocity = new Vector2(direction.x * moveSpeed, rb.velocity.y);
            
            // 更新朝向
            if (direction.x > 0 && !isFacingRight)
            {
                Flip();
            }
            else if (direction.x < 0 && isFacingRight)
            {
                Flip();
            }
            
            // 攻击检测
            if (distanceToPlayer <= attackRange && Time.time >= lastAttackTime + attackCooldown)
            {
                Attack();
            }
        }
        else
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
        }
    }
    
    private void Attack()
    {
        lastAttackTime = Time.time;
        animator?.SetTrigger("Attack");
        
        // 检测攻击范围内的玩家
        Collider2D[] hitPlayers = Physics2D.OverlapCircleAll(transform.position, attackRange);
        foreach (Collider2D hitPlayer in hitPlayers)
        {
            if (hitPlayer.CompareTag("Player"))
            {
                hitPlayer.GetComponent<PlayerHealth>()?.TakeDamage(attackDamage);
            }
        }
    }
    
    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        animator?.SetTrigger("Hit");
        
        if (currentHealth <= 0)
        {
            Die();
        }
    }
    
    private void Die()
    {
        animator?.SetTrigger("Die");
        // 禁用碰撞器和刚体
        GetComponent<Collider2D>().enabled = false;
        rb.velocity = Vector2.zero;
        rb.isKinematic = true;
        // 销毁游戏对象
        Destroy(gameObject, 1f);
    }
    
    private void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    // 添加SetStunned方法
    public void SetStunned(bool stunned)
    {
        isStunned = stunned;
        if (stunned)
        {
            rb.velocity = Vector2.zero;  // 硬直时停止移动
            animator?.SetTrigger("Stunned");  // 播放硬直动画
        }
        else
        {
            animator?.SetTrigger("Recover");  // 播放恢复动画
        }
    }
} 