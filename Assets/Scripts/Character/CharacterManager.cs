using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class CharacterManager : MonoBehaviour
{
    [Header("角色配置")]
    [SerializeField] private CharacterDatabase characterDatabase;
    [SerializeField] private int maxTeamSize = 3;
    [SerializeField] private float switchCooldown = 1f;
    
    [Header("场景引用")]
    [SerializeField] private Transform characterSpawnPoint;
    
    private List<CharacterData> currentTeam = new List<CharacterData>();
    private List<GameObject> activeCharacters = new List<GameObject>();
    private int currentCharacterIndex = 0;
    private float lastSwitchTime;
    
    private void Start()
    {
        if (characterDatabase == null)
        {
            Debug.LogError("CharacterDatabase未设置！");
            return;
        }
        
        // 初始化角色数据库
        characterDatabase.Initialize();
        
        // 获取随机角色组成队伍
        currentTeam = characterDatabase.GetRandomCharacters(maxTeamSize);
        
        // 生成第一个角色
        if (currentTeam.Count > 0)
        {
            SpawnCharacter(currentTeam[0]);
        }
    }
    
    private void SpawnCharacter(CharacterData characterData)
    {
        if (characterData == null || characterData.characterPrefab == null)
        {
            Debug.LogError("角色数据或预制体为空！");
            return;
        }
        
        // 清理当前角色
        foreach (var character in activeCharacters)
        {
            if (character != null)
            {
                Destroy(character);
            }
        }
        activeCharacters.Clear();
        
        // 生成新角色
        GameObject newCharacter = Instantiate(characterData.characterPrefab, 
            characterSpawnPoint.position, 
            characterSpawnPoint.rotation);
            
        // 设置角色数据
        PlayerController playerController = newCharacter.GetComponent<PlayerController>();
        if (playerController != null)
        {
            playerController.InitializeWithData(characterData);
        }
        
        activeCharacters.Add(newCharacter);
    }
    
    public bool CanSwitch()
    {
        return currentTeam.Count > 1 && 
               Time.time >= lastSwitchTime + switchCooldown;
    }
    
    public void SwitchCharacter()
    {
        if (!CanSwitch()) return;
        
        // 更新索引
        currentCharacterIndex = (currentCharacterIndex + 1) % currentTeam.Count;
        
        // 生成新角色
        SpawnCharacter(currentTeam[currentCharacterIndex]);
        
        // 更新切换时间
        lastSwitchTime = Time.time;
    }
    
    public CharacterData GetCurrentCharacterData()
    {
        if (currentTeam.Count == 0) return null;
        return currentTeam[currentCharacterIndex];
    }
    
    public List<CharacterData> GetCurrentTeam()
    {
        return new List<CharacterData>(currentTeam);
    }
    
    public bool IsCharacterInTeam(string characterID)
    {
        return currentTeam.Any(c => c.characterID == characterID);
    }
    
    public void AddCharacterToTeam(string characterID)
    {
        if (currentTeam.Count >= maxTeamSize)
        {
            Debug.LogWarning("队伍已满！");
            return;
        }
        
        CharacterData character = characterDatabase.GetCharacterData(characterID);
        if (character != null && !IsCharacterInTeam(characterID))
        {
            currentTeam.Add(character);
        }
    }
    
    public void RemoveCharacterFromTeam(string characterID)
    {
        CharacterData character = currentTeam.Find(c => c.characterID == characterID);
        if (character != null)
        {
            currentTeam.Remove(character);
            
            // 如果移除的是当前角色，切换到下一个角色
            if (currentTeam.Count > 0)
            {
                currentCharacterIndex = Mathf.Clamp(currentCharacterIndex, 0, currentTeam.Count - 1);
                SpawnCharacter(currentTeam[currentCharacterIndex]);
            }
        }
    }
} 