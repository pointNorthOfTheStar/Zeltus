using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class CharacterData : MonoBehaviour
{
    private static CharacterData instance;
    public static CharacterData Instance
    {
        get
        {
            if (instance == null)
            {
                var go = new GameObject("CharacterData");
                instance = go.AddComponent<CharacterData>();
                DontDestroyOnLoad(go);
            }
            return instance;
        }
    }

    // 模板数据字典（角色ID -> 模板数据）
    private Dictionary<string, CharacterTemplateData> templateDataDict = new Dictionary<string, CharacterTemplateData>();
    
    // 进度数据字典（角色ID -> 进度数据）
    private Dictionary<string, CharacterArchiveData> archiveDataDict = new Dictionary<string, CharacterArchiveData>();

    // 文件路径
    private string templateDataPath => Path.Combine(Application.streamingAssetsPath, "CharacterTemplates.json");
    private string archiveDataPath => Path.Combine(Application.persistentDataPath, "CharacterArchives.json");

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);

        // 加载模板数据
        LoadTemplateData();
        
        // 加载进度数据
        LoadArchiveData();
    }

    // 加载模板数据（从StreamingAssets）
    private void LoadTemplateData()
    {
        if (File.Exists(templateDataPath))
        {
            string json = File.ReadAllText(templateDataPath);
            var data = JsonUtility.FromJson<CharacterTemplateDataCollection>(json);
            templateDataDict = data.ToDictionary();
        }
        else
        {
            Debug.LogError($"Character template data not found at: {templateDataPath}");
        }
    }

    // 加载进度数据（从PersistentDataPath）
    private void LoadArchiveData()
    {
        if (File.Exists(archiveDataPath))
        {
            string json = File.ReadAllText(archiveDataPath);
            var data = JsonUtility.FromJson<CharacterArchiveDataCollection>(json);
            archiveDataDict = data.ToDictionary();
        }
    }

    // 保存进度数据
    private void SaveArchiveData()
    {
        var collection = new CharacterArchiveDataCollection(archiveDataDict);
        string json = JsonUtility.ToJson(collection);
        File.WriteAllText(archiveDataPath, json);
    }

    // 获取模板数据
    public CharacterTemplateData GetTemplateData(string characterId)
    {
        if (templateDataDict.TryGetValue(characterId, out var data))
        {
            return data;
        }
        Debug.LogError($"Template data not found for character: {characterId}");
        return null;
    }

    // 获取进度数据
    public CharacterArchiveData GetArchiveData(string characterId)
    {
        if (!archiveDataDict.TryGetValue(characterId, out var data))
        {
            // 如果不存在，创建新的进度数据
            data = new CharacterArchiveData();
            archiveDataDict[characterId] = data;
            SaveArchiveData(); // 保存新创建的进度数据
        }
        return data;
    }

    // 更新进度数据
    public void UpdateArchiveData(string characterId, CharacterArchiveData newData)
    {
        archiveDataDict[characterId] = newData;
        SaveArchiveData(); // 保存更新后的进度数据
    }

    // 升级角色等级并重新计算属性
    public void UpgradeCharacter(string characterId, int addLevel)
    {
        var archive = GetArchiveData(characterId);
        var template = GetTemplateData(characterId);
        if (archive == null || template == null) return;
        Upgrade.UpgradeCharacter(archive, template, addLevel);
        UpdateArchiveData(characterId, archive);
    }

    // 在应用退出时保存进度数据
    private void OnApplicationQuit()
    {
        SaveArchiveData();
    }
}

// 用于序列化模板数据集合
[System.Serializable]
public class CharacterTemplateDataCollection
{
    public List<CharacterTemplateData> templates = new List<CharacterTemplateData>();

    public Dictionary<string, CharacterTemplateData> ToDictionary()
    {
        var dict = new Dictionary<string, CharacterTemplateData>();
        foreach (var template in templates)
        {
            dict[template.characterId] = template;
        }
        return dict;
    }
}

// 用于序列化进度数据集合
[System.Serializable]
public class CharacterArchiveDataCollection
{
    public List<CharacterArchiveData> archives = new List<CharacterArchiveData>();

    public CharacterArchiveDataCollection(Dictionary<string, CharacterArchiveData> dict)
    {
        archives = new List<CharacterArchiveData>(dict.Values);
    }

    public Dictionary<string, CharacterArchiveData> ToDictionary()
    {
        var dict = new Dictionary<string, CharacterArchiveData>();
        foreach (var archive in archives)
        {
            dict[archive.characterId] = archive;
        }
        return dict;
    }
} 