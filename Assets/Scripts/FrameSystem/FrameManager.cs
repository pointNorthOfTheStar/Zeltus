using UnityEngine;
using System.Collections.Generic;
using System;

public class FrameManager : MonoBehaviour
{
    [Header("帧系统设置")]
    [SerializeField] private float frameRate = 60f;                    // 游戏帧率
    [SerializeField] private bool showDebugInfo = true;                // 是否显示调试信息
    
    [Header("帧数据")]
    [SerializeField] private List<FrameData> frameDataList;            // 所有动作的帧数据
    
    private Dictionary<string, FrameData> frameDataDict;               // 帧数据字典
    private float frameTime;                                          // 每帧的时间
    private int currentFrame;                                         // 当前帧
    private string currentAction;                                     // 当前动作
    private bool isActionActive;                                      // 动作是否激活
    
    // 事件系统
    public event Action<int> OnFrameChanged;                          // 帧变化事件
    public event Action<string> OnActionStarted;                      // 动作开始事件
    public event Action<string> OnActionEnded;                        // 动作结束事件
    
    private void Awake()
    {
        InitializeFrameSystem();
    }
    
    private void InitializeFrameSystem()
    {
        frameTime = 1f / frameRate;
        frameDataDict = new Dictionary<string, FrameData>();
        
        // 初始化帧数据字典
        foreach (var data in frameDataList)
        {
            frameDataDict[data.actionName] = data;
        }
    }
    
    private void Update()
    {
        if (isActionActive)
        {
            UpdateCurrentFrame();
        }
    }
    
    public void StartAction(string actionName)
    {
        if (!frameDataDict.ContainsKey(actionName))
        {
            Debug.LogWarning($"动作 {actionName} 不存在！");
            return;
        }
        
        currentAction = actionName;
        currentFrame = 0;
        isActionActive = true;
        
        OnActionStarted?.Invoke(actionName);
    }
    
    private void UpdateCurrentFrame()
    {
        currentFrame++;
        OnFrameChanged?.Invoke(currentFrame);
        
        FrameData currentFrameData = frameDataDict[currentAction];
        
        // 检查是否到达动作结束帧
        if (currentFrame >= currentFrameData.totalFrames)
        {
            EndCurrentAction();
        }
    }
    
    private void EndCurrentAction()
    {
        isActionActive = false;
        OnActionEnded?.Invoke(currentAction);
    }
    
    // 检查当前帧是否在特定范围内
    public bool IsInFrameRange(int startFrame, int endFrame)
    {
        return currentFrame >= startFrame && currentFrame <= endFrame;
    }
    
    // 检查当前是否处于无敌帧
    public bool IsInvincible()
    {
        if (!isActionActive || !frameDataDict.ContainsKey(currentAction))
            return false;
            
        FrameData currentFrameData = frameDataDict[currentAction];
        foreach (var invincibilityFrame in currentFrameData.invincibilityFrames)
        {
            if (IsInFrameRange(invincibilityFrame.startFrame, invincibilityFrame.endFrame))
            {
                return true;
            }
        }
        return false;
    }
    
    // 检查当前帧是否可以取消到指定动作
    public bool CanCancelTo(string targetAction)
    {
        if (!isActionActive || !frameDataDict.ContainsKey(currentAction))
            return false;
            
        FrameData currentFrameData = frameDataDict[currentAction];
        foreach (var cancelFrame in currentFrameData.cancelFrames)
        {
            if (IsInFrameRange(cancelFrame.startFrame, cancelFrame.endFrame) &&
                cancelFrame.cancelableActions.Contains(targetAction))
            {
                return true;
            }
        }
        return false;
    }
    
    // 获取当前帧的判定框
    public List<HitboxFrame> GetCurrentHitboxes()
    {
        if (!isActionActive || !frameDataDict.ContainsKey(currentAction))
            return new List<HitboxFrame>();
            
        FrameData currentFrameData = frameDataDict[currentAction];
        List<HitboxFrame> activeHitboxes = new List<HitboxFrame>();
        
        foreach (var hitbox in currentFrameData.hitboxFrames)
        {
            if (IsInFrameRange(hitbox.startFrame, hitbox.endFrame))
            {
                activeHitboxes.Add(hitbox);
            }
        }
        
        return activeHitboxes;
    }
    
    // 调试信息
    private void OnGUI()
    {
        if (!showDebugInfo)
            return;
            
        GUI.Label(new Rect(10, 10, 200, 100), 
            $"当前动作: {currentAction}\n" +
            $"当前帧: {currentFrame}\n" +
            $"是否无敌: {IsInvincible()}\n" +
            $"动作激活: {isActionActive}");
    }
} 