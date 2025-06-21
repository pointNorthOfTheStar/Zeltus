using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public class CharacterTemplateData
{
    // ================== 基本信息 ==================
    public string Name;         // 姓名
    public Rarity Rarity;       // 稀有度
    public Element Element;     // 属性
    public Class Class;         // 职业
    public Faction Faction;     // 阵营

    // ================== 基础属性 ==================
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

    

    // ================== 节点数据 ==================
    public List<LevelStat> LevelStats = new List<LevelStat>(6); // 等级属性节点
}