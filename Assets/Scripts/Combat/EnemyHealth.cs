using UnityEngine;
using UnityEngine.Events;

public class EnemyHealth : HealthSystem
{
    [Header("敌人特有属性")]
    [SerializeField] private float stunThreshold = 50f;    // 硬直阈值
    [SerializeField] private float stunDuration = 2f;      // 硬直持续时间
    
    private bool isStunned;
    private float stunEndTime;
    
    // 敌人特有事件
    public UnityEvent onStunned;
    public UnityEvent onStunEnd;
    
    protected override void Start()
    {
        base.Start();
        
        // 订阅生命值变化事件
        onHealthChanged.AddListener(CheckStun);
    }
    
    private void Update()
    {
        // 检查硬直状态
        if (isStunned && Time.time >= stunEndTime)
        {
            EndStun();
        }
    }
    
    private void CheckStun(float healthPercentage)
    {
        // 如果受到足够伤害，进入硬直状态
        if (currentHealth <= stunThreshold && !isStunned)
        {
            StartStun();
        }
    }
    
    private void StartStun()
    {
        isStunned = true;
        stunEndTime = Time.time + stunDuration;
        onStunned?.Invoke();
        
        // 可以在这里添加硬直动画或效果
        if (TryGetComponent<EnemyController>(out var controller))
        {
            controller.SetStunned(true);
        }
    }
    
    private void EndStun()
    {
        isStunned = false;
        onStunEnd?.Invoke();
        
        // 可以在这里添加恢复动画或效果
        if (TryGetComponent<EnemyController>(out var controller))
        {
            controller.SetStunned(false);
        }
    }
    
    public bool IsStunned()
    {
        return isStunned;
    }
    
    // 重写死亡方法，可以添加敌人特有的死亡逻辑
    protected override void Die()
    {
        // 可以在这里添加死亡动画、掉落物等
        base.Die();
    }
} 