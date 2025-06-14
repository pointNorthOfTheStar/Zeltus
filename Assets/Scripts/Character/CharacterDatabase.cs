using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "CharacterDatabase", menuName = "Character/Character Database")]
public class CharacterDatabase : ScriptableObject
{
    public List<CharacterData> availableCharacters = new List<CharacterData>();
    
    private Dictionary<string, CharacterData> characterDictionary;
    
    public void Initialize()
    {
        characterDictionary = new Dictionary<string, CharacterData>();
        foreach (var character in availableCharacters)
        {
            if (!characterDictionary.ContainsKey(character.characterID))
            {
                characterDictionary.Add(character.characterID, character);
            }
            else
            {
                Debug.LogWarning($"重复的角色ID: {character.characterID}");
            }
        }
    }
    
    public CharacterData GetCharacterData(string characterID)
    {
        if (characterDictionary == null)
        {
            Initialize();
        }
        
        if (characterDictionary.TryGetValue(characterID, out CharacterData character))
        {
            return character;
        }
        
        Debug.LogError($"找不到角色ID: {characterID}");
        return null;
    }
    
    public List<CharacterData> GetRandomCharacters(int count)
    {
        if (count > availableCharacters.Count)
        {
            Debug.LogWarning($"请求的角色数量({count})大于可用角色数量({availableCharacters.Count})");
            count = availableCharacters.Count;
        }
        
        List<CharacterData> result = new List<CharacterData>();
        List<CharacterData> tempList = new List<CharacterData>(availableCharacters);
        
        for (int i = 0; i < count; i++)
        {
            int randomIndex = Random.Range(0, tempList.Count);
            result.Add(tempList[randomIndex]);
            tempList.RemoveAt(randomIndex);
        }
        
        return result;
    }
} 