using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("移动参数")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 10f;
    
    [Header("战斗参数")]
    [SerializeField] private float attackDamage = 10f;
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private float attackCooldown = 0.5f;
    [SerializeField] private float attackDuration = 0.3f;
    
    [Header("闪避参数")]
    [SerializeField] private float dodgeSpeed = 10f;
    [SerializeField] private float dodgeDuration = 0.2f;
    [SerializeField] private float dodgeCooldown = 1f;
    [SerializeField] private float dodgeInvincibilityDuration = 0.3f;
    
    [Header("技能参数")]
    [SerializeField] private float skillCooldown = 5f;
    [SerializeField] private float skillDuration = 1f;
    
    private Rigidbody2D rb;
    private Animator animator;
    private InputBuffer inputBuffer;
    private CharacterManager characterManager;
    private bool isGrounded;
    private bool isFacingRight = true;
    private bool isAttacking;
    private bool isDodging;
    private bool isUsingSkill;
    private bool isInvincible;
    private float lastAttackTime;
    private float lastDodgeTime;
    private float lastSkillTime;
    
    // 动作状态
    private enum ActionState
    {
        Idle,
        Moving,
        Jumping,
        Attacking,
        Dodging,
        UsingSkill
    }
    private ActionState currentState = ActionState.Idle;
    
    private CharacterData characterData;
    
    public void InitializeWithData(CharacterData data)
    {
        characterData = data;
        
        // 更新移动参数
        moveSpeed = data.moveSpeed;
        jumpForce = data.jumpForce;
        
        // 更新战斗参数
        attackDamage = data.attackDamage;
        attackRange = data.attackRange;
        attackCooldown = data.attackCooldown;
        attackDuration = data.attackDuration;
        
        // 更新闪避参数
        dodgeSpeed = data.dodgeSpeed;
        dodgeDuration = data.dodgeDuration;
        dodgeCooldown = data.dodgeCooldown;
        dodgeInvincibilityDuration = data.dodgeInvincibilityDuration;
        
        // 更新技能参数
        skillCooldown = data.skillCooldown;
        skillDuration = data.skillDuration;
        
        // 更新特殊属性
        canDoubleJump = data.canDoubleJump;
        hasAirAttack = data.hasAirAttack;
        hasCounterAttack = data.hasCounterAttack;
    }
    
    private void Awake()
    {
        // 获取必要组件
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.LogError("Rigidbody2D component is missing!");
            rb = gameObject.AddComponent<Rigidbody2D>();
        }

        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogWarning("Animator component is missing!");
        }

        inputBuffer = GetComponent<InputBuffer>();
        if (inputBuffer == null)
        {
            Debug.LogWarning("InputBuffer component is missing!");
            inputBuffer = gameObject.AddComponent<InputBuffer>();
        }

        characterManager = FindObjectOfType<CharacterManager>();
        if (characterManager == null)
        {
            Debug.LogWarning("CharacterManager not found in scene!");
        }
    }
    
    private void Update()
    {
        if (rb == null) return; // 如果缺少必要组件，不执行更新
        
        HandleInput();
        UpdateState();
        UpdateAnimation();
    }
    
    private void HandleInput()
    {
        if (isDodging || isUsingSkill)
            return;

        // 移动输入
        float moveInput = Input.GetAxisRaw("Horizontal");
        
        // 跳跃预输入
        if (Input.GetButtonDown("Jump") && inputBuffer != null)
        {
            inputBuffer.BufferInput("Jump");
        }
        
        // 攻击预输入
        if (Input.GetMouseButtonDown(0) && inputBuffer != null)
        {
            inputBuffer.BufferInput("Attack");
        }

        // 闪避预输入
        if (Input.GetKeyDown(KeyCode.LeftShift) && inputBuffer != null)
        {
            inputBuffer.BufferInput("Dodge");
        }

        // 技能预输入
        if (Input.GetKeyDown(KeyCode.E) && inputBuffer != null)
        {
            inputBuffer.BufferInput("Skill");
        }

        // 切人
        if (Input.GetKeyDown(KeyCode.Space) && characterManager != null && characterManager.CanSwitch())
        {
            characterManager.SwitchCharacter();
            return;
        }
        
        // 处理移动
        if (currentState != ActionState.Attacking)
        {
            Vector2 movement = new Vector2(moveInput * moveSpeed, rb.velocity.y);
            rb.velocity = movement;
            
            // 更新朝向
            if (moveInput > 0 && !isFacingRight)
            {
                Flip();
            }
            else if (moveInput < 0 && isFacingRight)
            {
                Flip();
            }
        }
        
        // 处理预输入的跳跃
        if (isGrounded && inputBuffer != null && inputBuffer.ConsumeBufferedInput("Jump"))
        {
            Jump();
        }
        
        // 处理预输入的攻击
        if (!isAttacking && Time.time >= lastAttackTime + attackCooldown && 
            inputBuffer != null && inputBuffer.ConsumeBufferedInput("Attack"))
        {
            Attack();
        }

        // 处理预输入的闪避
        if (!isDodging && Time.time >= lastDodgeTime + dodgeCooldown && 
            inputBuffer != null && inputBuffer.ConsumeBufferedInput("Dodge"))
        {
            Dodge();
        }

        // 处理预输入的技能
        if (!isUsingSkill && Time.time >= lastSkillTime + skillCooldown && 
            inputBuffer != null && inputBuffer.ConsumeBufferedInput("Skill"))
        {
            UseSkill();
        }
    }
    
    private void UpdateState()
    {
        if (isUsingSkill)
        {
            currentState = ActionState.UsingSkill;
        }
        else if (isDodging)
        {
            currentState = ActionState.Dodging;
        }
        else if (isAttacking)
        {
            currentState = ActionState.Attacking;
        }
        else if (!isGrounded)
        {
            currentState = ActionState.Jumping;
        }
        else if (Mathf.Abs(rb.velocity.x) > 0.1f)
        {
            currentState = ActionState.Moving;
        }
        else
        {
            currentState = ActionState.Idle;
        }
    }
    
    private void UpdateAnimation()
    {
        animator?.SetFloat("Speed", Mathf.Abs(rb.velocity.x));
        animator?.SetBool("IsGrounded", isGrounded);
        animator?.SetBool("IsAttacking", isAttacking);
        animator?.SetBool("IsDodging", isDodging);
        animator?.SetBool("IsUsingSkill", isUsingSkill);
    }
    
    private void Jump()
    {
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        animator?.SetTrigger("Jump");
    }
    
    private void Attack()
    {
        isAttacking = true;
        lastAttackTime = Time.time;
        
        // 检测攻击范围内的敌人
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, attackRange);
        foreach (Collider2D enemy in hitEnemies)
        {
            if (enemy.CompareTag("Enemy"))
            {
                enemy.GetComponent<EnemyHealth>()?.TakeDamage(attackDamage);
            }
        }
        
        animator?.SetTrigger("Attack");
        
        // 攻击动作结束后重置状态
        Invoke(nameof(ResetAttackState), attackDuration);
    }

    private void Dodge()
    {
        isDodging = true;
        isInvincible = true;
        lastDodgeTime = Time.time;

        // 设置闪避方向
        float dodgeDirection = isFacingRight ? 1f : -1f;
        rb.velocity = new Vector2(dodgeDirection * dodgeSpeed, rb.velocity.y);

        animator?.SetTrigger("Dodge");

        // 闪避结束后重置状态
        Invoke(nameof(ResetDodgeState), dodgeDuration);
        // 无敌时间结束后重置无敌状态
        Invoke(nameof(ResetInvincibility), dodgeInvincibilityDuration);
    }

    private void UseSkill()
    {
        isUsingSkill = true;
        lastSkillTime = Time.time;

        // 技能逻辑
        animator?.SetTrigger("Skill");

        // 技能结束后重置状态
        Invoke(nameof(ResetSkillState), skillDuration);
    }
    
    private void ResetAttackState()
    {
        isAttacking = false;
    }

    private void ResetDodgeState()
    {
        isDodging = false;
    }

    private void ResetSkillState()
    {
        isUsingSkill = false;
    }

    private void ResetInvincibility()
    {
        isInvincible = false;
    }
    
    private void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }
    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }
    
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }

    // 用于检测是否处于无敌状态
    public bool IsInvincible()
    {
        return isInvincible;
    }
} 