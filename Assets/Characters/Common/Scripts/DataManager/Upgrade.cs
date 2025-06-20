using UnityEngine;

public static class Upgrade
{
    // 升级角色等级并重新计算属性
    public static void UpgradeCharacter(CharacterArchiveData archive, CharacterTemplateData template, int addLevel)
    {
        archive.level += addLevel;
        CalculateStats(archive, template);
    }

    // 根据模板和等级插值计算属性
    public static void CalculateStats(CharacterArchiveData archive, CharacterTemplateData template)
    {
        if (template.levelStats == null || template.levelStats.Length != 6)
            return;
        int idx = Mathf.Clamp((archive.level - 1) / 10, 0, 5);
        int nextIdx = Mathf.Clamp(idx + 1, 0, 5);
        int baseLv = 10 * (idx + 1);
        int nextLv = 10 * (nextIdx + 1);
        float t = (archive.level - baseLv) / (float)(nextLv - baseLv);
        // 生命、攻击、防御插值
        var a = template.levelStats[idx];
        var b = template.levelStats[nextIdx];
        archive.hp = Mathf.RoundToInt(a.hp + (b.hp - a.hp) * t);
        archive.attack = Mathf.RoundToInt(a.attack + (b.attack - a.attack) * t);
        archive.defense = Mathf.RoundToInt(a.defense + (b.defense - a.defense) * t);
        // 其它属性直接取模板
        archive.impactForce = template.impactForce;
        archive.critRate = template.critRate;
        archive.critDamage = template.critDamage;
        archive.abnormalControl = template.abnormalControl;
        archive.abnormalMastery = template.abnormalMastery;
        archive.penetrationRate = template.penetrationRate;
        archive.energyRecovery = template.energyRecovery;
    }
} 