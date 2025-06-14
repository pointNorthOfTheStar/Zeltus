using UnityEngine;
using UnityEngine.Events;

public class PlayerHealth : HealthSystem
{
    [Header("玩家特有属性")]
    [SerializeField] private float maxStamina = 100f;
    [SerializeField] private float staminaRegenRate = 10f;
    
    private float currentStamina;
    
    // 玩家特有事件
    public UnityEvent onStaminaDepleted;
    public UnityEvent<float> onStaminaChanged;
    
    protected override void Start()
    {
        base.Start();
        currentStamina = maxStamina;
    }
    
    private void Update()
    {
        // 自动恢复耐力
        if (currentStamina < maxStamina)
        {
            currentStamina = Mathf.Min(maxStamina, currentStamina + staminaRegenRate * Time.deltaTime);
            onStaminaChanged?.Invoke(currentStamina / maxStamina);
        }
    }
    
    public bool UseStamina(float amount)
    {
        if (currentStamina >= amount)
        {
            currentStamina -= amount;
            onStaminaChanged?.Invoke(currentStamina / maxStamina);
            
            if (currentStamina <= 0)
            {
                onStaminaDepleted?.Invoke();
            }
            
            return true;
        }
        return false;
    }
    
    public float GetStaminaPercentage()
    {
        return currentStamina / maxStamina;
    }
    
    // 重写受伤方法，可以添加玩家特有的受伤逻辑
    public override void TakeDamage(float damage)
    {
        // 如果处于无敌状态，不受到伤害
        if (GetComponent<PlayerController>()?.IsInvincible() == true)
            return;
            
        base.TakeDamage(damage);
    }
} 