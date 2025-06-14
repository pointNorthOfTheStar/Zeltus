using UnityEngine;
using System.Collections.Generic;

public class CharacterManager : MonoBehaviour
{
    [System.Serializable]
    public class CharacterData
    {
        public GameObject characterPrefab;
        public string characterName;
        public float switchCooldown = 1f;
    }

    [Header("角色设置")]
    [SerializeField] private List<CharacterData> characters = new List<CharacterData>();
    [SerializeField] private Transform characterSpawnPoint;
    [SerializeField] private float switchDuration = 0.5f;

    private List<GameObject> activeCharacters = new List<GameObject>();
    private int currentCharacterIndex = 0;
    private float lastSwitchTime;
    private bool isSwitching = false;

    private void Start()
    {
        InitializeCharacters();
    }

    private void InitializeCharacters()
    {
        // 初始化所有角色
        foreach (var charData in characters)
        {
            GameObject character = Instantiate(charData.characterPrefab, characterSpawnPoint.position, Quaternion.identity);
            character.SetActive(false);
            activeCharacters.Add(character);
        }

        // 激活第一个角色
        if (activeCharacters.Count > 0)
        {
            activeCharacters[0].SetActive(true);
        }
    }

    public void SwitchCharacter()
    {
        if (isSwitching || Time.time < lastSwitchTime + characters[currentCharacterIndex].switchCooldown)
            return;

        isSwitching = true;
        lastSwitchTime = Time.time;

        // 获取当前角色和目标角色
        GameObject currentChar = activeCharacters[currentCharacterIndex];
        int nextIndex = (currentCharacterIndex + 1) % activeCharacters.Count;
        GameObject nextChar = activeCharacters[nextIndex];

        // 保存当前角色的位置和朝向
        Vector3 currentPosition = currentChar.transform.position;
        bool isFacingRight = currentChar.transform.localScale.x > 0;

        // 设置新角色的位置和朝向
        nextChar.transform.position = currentPosition;
        nextChar.transform.localScale = new Vector3(
            isFacingRight ? 1 : -1,
            nextChar.transform.localScale.y,
            nextChar.transform.localScale.z
        );

        // 切换角色
        currentChar.SetActive(false);
        nextChar.SetActive(true);
        currentCharacterIndex = nextIndex;

        // 重置切换状态
        Invoke(nameof(ResetSwitchState), switchDuration);
    }

    private void ResetSwitchState()
    {
        isSwitching = false;
    }

    public GameObject GetCurrentCharacter()
    {
        return activeCharacters[currentCharacterIndex];
    }

    public bool CanSwitch()
    {
        return !isSwitching && Time.time >= lastSwitchTime + characters[currentCharacterIndex].switchCooldown;
    }
} 