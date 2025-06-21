using UnityEngine;
using System;

[Serializable]
public class CharacterArchiveData
{
    public int Level;           // 当前等级
    // 现有属性
    public int HP;              // 生命值
    public int ATK;             // 攻击力
    public int DEF;             // 防御力
    public int Impact;          // 冲击力
    public float CritRate;      // 暴击率
    public float CritDMG;       // 暴击伤害
    public int DebuffRate;      // 异常掌控
    public int DebuffHit;       // 异常精通
    public float Penetration;   // 穿透率
    public float EnergyRegen;   // 能量自动回复
} 