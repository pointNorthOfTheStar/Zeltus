using UnityEngine;
using System.Collections.Generic;

public class InputBuffer : MonoBehaviour
{
    [System.Serializable]
    public class BufferedInput
    {
        public string actionName;
        public float timeStamp;
        public float validTime;

        public BufferedInput(string action, float time, float valid)
        {
            actionName = action;
            timeStamp = time;
            validTime = valid;
        }
    }

    [Header("预输入设置")]
    [SerializeField] private float bufferTime = 0.2f; // 预输入有效时间
    [SerializeField] private int maxBufferSize = 3;   // 最大缓存指令数

    private Queue<BufferedInput> inputBuffer = new Queue<BufferedInput>();
    private float currentTime;

    private void Update()
    {
        currentTime = Time.time;
        CleanExpiredInputs();
    }

    // 添加预输入指令
    public void BufferInput(string actionName)
    {
        if (inputBuffer.Count >= maxBufferSize)
        {
            inputBuffer.Dequeue(); // 移除最旧的输入
        }

        inputBuffer.Enqueue(new BufferedInput(actionName, currentTime, bufferTime));
    }

    // 检查是否有特定的预输入指令
    public bool HasBufferedInput(string actionName)
    {
        foreach (var input in inputBuffer)
        {
            if (input.actionName == actionName && !IsInputExpired(input))
            {
                return true;
            }
        }
        return false;
    }

    // 获取并移除特定的预输入指令
    public bool ConsumeBufferedInput(string actionName)
    {
        var tempBuffer = new Queue<BufferedInput>();
        bool found = false;

        while (inputBuffer.Count > 0)
        {
            var input = inputBuffer.Dequeue();
            if (!found && input.actionName == actionName && !IsInputExpired(input))
            {
                found = true;
                continue; // 跳过这个输入，相当于移除它
            }
            tempBuffer.Enqueue(input);
        }

        // 恢复其他输入
        inputBuffer = tempBuffer;
        return found;
    }

    // 清理过期的输入
    private void CleanExpiredInputs()
    {
        while (inputBuffer.Count > 0 && IsInputExpired(inputBuffer.Peek()))
        {
            inputBuffer.Dequeue();
        }
    }

    // 检查输入是否过期
    private bool IsInputExpired(BufferedInput input)
    {
        return currentTime - input.timeStamp > input.validTime;
    }

    // 清空所有预输入
    public void ClearBuffer()
    {
        inputBuffer.Clear();
    }
} 