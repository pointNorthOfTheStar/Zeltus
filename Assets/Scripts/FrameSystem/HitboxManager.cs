using UnityEngine;
using System.Collections.Generic;

public class HitboxManager : MonoBehaviour
{
    [Header("判定框设置")]
    [SerializeField] private bool showHitboxes = true;                // 是否显示判定框
    [SerializeField] private Color attackHitboxColor = Color.red;     // 攻击判定框颜色
    [SerializeField] private Color hurtHitboxColor = Color.blue;      // 受击判定框颜色
    [SerializeField] private Color guardHitboxColor = Color.green;    // 防御判定框颜色
    
    private FrameManager frameManager;                                // 帧管理器引用
    private Dictionary<HitboxType, List<HitboxFrame>> activeHitboxes; // 当前激活的判定框
    
    private void Start()
    {
        frameManager = GetComponent<FrameManager>();
        activeHitboxes = new Dictionary<HitboxType, List<HitboxFrame>>();
        
        // 初始化判定框字典
        foreach (HitboxType type in System.Enum.GetValues(typeof(HitboxType)))
        {
            activeHitboxes[type] = new List<HitboxFrame>();
        }
        
        // 订阅帧变化事件
        frameManager.OnFrameChanged += UpdateHitboxes;
    }
    
    private void UpdateHitboxes(int currentFrame)
    {
        // 清空当前判定框
        foreach (var hitboxList in activeHitboxes.Values)
        {
            hitboxList.Clear();
        }
        
        // 获取当前帧的所有判定框
        List<HitboxFrame> currentHitboxes = frameManager.GetCurrentHitboxes();
        
        // 按类型分类判定框
        foreach (var hitbox in currentHitboxes)
        {
            activeHitboxes[hitbox.type].Add(hitbox);
        }
    }
    
    // 检查攻击判定是否命中
    public bool CheckHit(HitboxFrame attackHitbox, HitboxFrame hurtHitbox)
    {
        // 获取攻击判定框的世界坐标
        Vector3 attackPosition = transform.position + attackHitbox.offset;
        Vector3 attackSize = attackHitbox.size;
        
        // 获取受击判定框的世界坐标
        Vector3 hurtPosition = transform.position + hurtHitbox.offset;
        Vector3 hurtSize = hurtHitbox.size;
        
        // 检查两个判定框是否相交
        return CheckBoxIntersection(attackPosition, attackSize, hurtPosition, hurtSize);
    }
    
    // 检查两个盒子是否相交
    private bool CheckBoxIntersection(Vector3 pos1, Vector3 size1, Vector3 pos2, Vector3 size2)
    {
        // 计算两个盒子的边界
        Vector3 min1 = pos1 - size1 / 2;
        Vector3 max1 = pos1 + size1 / 2;
        Vector3 min2 = pos2 - size2 / 2;
        Vector3 max2 = pos2 + size2 / 2;
        
        // 检查是否相交
        return (min1.x <= max2.x && max1.x >= min2.x) &&
               (min1.y <= max2.y && max1.y >= min2.y) &&
               (min1.z <= max2.z && max1.z >= min2.z);
    }
    
    // 在Scene视图中绘制判定框
    private void OnDrawGizmos()
    {
        if (!showHitboxes || activeHitboxes == null)
            return;
            
        foreach (var hitboxList in activeHitboxes.Values)
        {
            foreach (var hitbox in hitboxList)
            {
                // 设置判定框颜色
                switch (hitbox.type)
                {
                    case HitboxType.Attack:
                        Gizmos.color = attackHitboxColor;
                        break;
                    case HitboxType.Hurt:
                        Gizmos.color = hurtHitboxColor;
                        break;
                    case HitboxType.Guard:
                        Gizmos.color = guardHitboxColor;
                        break;
                    default:
                        Gizmos.color = Color.white;
                        break;
                }
                
                // 绘制判定框
                Vector3 position = transform.position + hitbox.offset;
                Gizmos.DrawWireCube(position, hitbox.size);
            }
        }
    }
} 