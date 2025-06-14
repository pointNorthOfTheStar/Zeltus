using UnityEngine;
using System;
using System.Collections.Generic;

[System.Serializable]
public class FrameData
{
    [Header("基础帧数据")]
    public string actionName;                    // 动作名称
    public int totalFrames;                      // 总帧数
    public int startupFrames;                    // 前摇帧
    public int activeFrames;                     // 有效帧
    public int recoveryFrames;                   // 后摇帧

    [Header("特殊帧数据")]
    public List<InvincibilityFrame> invincibilityFrames;    // 无敌帧
    public List<CancelFrame> cancelFrames;                  // 可取消帧
    public List<HitboxFrame> hitboxFrames;                  // 判定框帧

    [Header("动作属性")]
    public float damage;                         // 伤害值
    public float stunTime;                       // 硬直时间
    public bool canCombo;                        // 是否可以连段
    public List<string> cancelableActions;       // 可取消的动作列表
}

[System.Serializable]
public class InvincibilityFrame
{
    public int startFrame;                       // 开始帧
    public int endFrame;                         // 结束帧
    public InvincibilityType type;               // 无敌类型
}

[System.Serializable]
public class CancelFrame
{
    public int startFrame;                       // 开始帧
    public int endFrame;                         // 结束帧
    public List<string> cancelableActions;       // 可取消的动作列表
}

[System.Serializable]
public class HitboxFrame
{
    public int startFrame;                       // 开始帧
    public int endFrame;                         // 结束帧
    public Vector3 size;                         // 判定框大小
    public Vector3 offset;                       // 判定框偏移
    public HitboxType type;                      // 判定框类型
}

public enum InvincibilityType
{
    None,           // 无无敌
    Full,           // 完全无敌
    UpperBody,      // 上半身无敌
    LowerBody       // 下半身无敌
}

public enum HitboxType
{
    Attack,         // 攻击判定
    Hurt,           // 受击判定
    Guard,          // 防御判定
    Special         // 特殊判定
} 