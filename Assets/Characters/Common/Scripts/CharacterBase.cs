using UnityEngine;
using System;

public class CharacterBase<T> : MonoBehaviour where T : CharacterBattleData
{
    // 角色ID
    protected string characterId;
    
    // 角色数据
    protected CharacterArchiveData archiveData;
    protected T battleData;  // 使用泛型T，T必须是CharacterBattleData的子类

    // 初始化方法
    public virtual void Initialize(string characterId)
    {
        this.characterId = characterId;
        
        // 从CharacterDataManager获取图鉴数据
        archiveData = CharacterDataManager.Instance.GetArchiveData(characterId);
        
        // 创建战斗数据
        battleData = (T)Activator.CreateInstance(typeof(T), archiveData);
    }
} 