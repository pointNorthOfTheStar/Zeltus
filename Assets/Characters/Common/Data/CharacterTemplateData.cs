using UnityEngine;
using System;

[Serializable]
public class CharacterTemplateData
{
    [Serializable]
    // 基础属性
    public int baseHp;           // 基础生命值
    public int baseAttack;       // 基础攻击力
    public int baseDefense;      // 基础防御力
    public int impactForce;      // 冲击力
    public float critRate;       // 暴击率（百分比）
    public float critDamage;     // 暴击伤害（百分比）
    public int abnormalControl;  // 异常掌控
    public int abnormalMastery;  // 异常精通
    public float penetrationRate;// 穿透率（百分比）
    public float energyRecovery; // 能量自动回复

    [Serializable]
    // 等级属性节点
    public struct LevelStat
    {
        public int hp;
        public int attack;
        public int defense;
    }

    public LevelStat[] levelStats = new LevelStat[6]; // 10,20,...,60级的属性节点
}