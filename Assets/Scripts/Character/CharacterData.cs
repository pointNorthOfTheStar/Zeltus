using UnityEngine;

[CreateAssetMenu(fileName = "NewCharacterData", menuName = "Character/Character Data")]
public class CharacterData : ScriptableObject
{
    [Header("基础信息")]
    public string characterName;
    public string characterID;
    public Sprite characterPortrait;
    public GameObject characterPrefab;
    
    [Header("基础属性")]
    public float maxHealth = 100f;
    public float maxStamina = 100f;
    public float staminaRegenRate = 10f;
    
    [Header("移动参数")]
    public float moveSpeed = 5f;
    public float jumpForce = 10f;
    
    [Header("战斗参数")]
    public float attackDamage = 10f;
    public float attackRange = 2f;
    public float attackCooldown = 0.5f;
    public float attackDuration = 0.3f;
    
    [Header("闪避参数")]
    public float dodgeSpeed = 10f;
    public float dodgeDuration = 0.2f;
    public float dodgeCooldown = 1f;
    public float dodgeInvincibilityDuration = 0.3f;
    
    [Header("技能参数")]
    public float skillCooldown = 5f;
    public float skillDuration = 1f;
    public float skillDamage = 20f;
    
    [Header("特殊属性")]
    public bool canDoubleJump = false;
    public bool hasAirAttack = false;
    public bool hasCounterAttack = false;
} 