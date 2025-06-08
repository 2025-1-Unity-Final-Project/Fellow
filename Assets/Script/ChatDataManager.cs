using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

[System.Serializable]
public class ChatMessage
{
    public bool isUser;           // 사용자 메시지인지
    public string message;        // 메시지 내용
    public string timestamp;      // 시간 정보 (yyyy-MM-dd-HH-mm)
    public string user;           // 사용자명
    
    public ChatMessage(bool isUser, string message, string user)
    {
        this.isUser = isUser;
        this.message = message;
        this.user = user;
        this.timestamp = DateTime.Now.ToString("yyyy-MM-dd-HH-mm");
    }
}

[System.Serializable]
public class ChatHistoryData
{
    public List<ChatMessage> messages = new List<ChatMessage>();
}

public class ChatDataManager : MonoBehaviour
{
    private static ChatDataManager instance;
    public static ChatDataManager Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject go = new GameObject("ChatDataManager");
                instance = go.AddComponent<ChatDataManager>();
                DontDestroyOnLoad(go);
            }
            return instance;
        }
    }
    
    private ChatHistoryData chatHistory = new ChatHistoryData();
    private string filePath;
    
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            filePath = Application.persistentDataPath + "/chatHistory.json";
            Debug.Log($"채팅 데이터 저장 경로: {filePath}");
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }
    
// AddMessage 메서드 수정

/// <summary>
/// 새 메시지 추가 및 자동 저장 (사용자명 형식 맞춤)
/// </summary>
public void AddMessage(bool isUser, string message, string user)
{
    // ChatManager1 형식에 맞게 사용자명 변환 ⭐ 수정된 부분
    string convertedUser = isUser ? "나" : "프리렌";
    
    ChatMessage newMessage = new ChatMessage(isUser, message, convertedUser);
    chatHistory.messages.Add(newMessage);
    
    SaveChatHistory();
    Debug.Log($"메시지 저장됨: [{convertedUser}] {message}");
}
    
    /// <summary>
    /// 채팅 내역을 파일에 저장
    /// </summary>
    public void SaveChatHistory()
    {
        try
        {
            string json = JsonUtility.ToJson(chatHistory, true);
            File.WriteAllText(filePath, json);
            Debug.Log($"채팅 내역 저장 완료. 총 {chatHistory.messages.Count}개 메시지");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"채팅 내역 저장 실패: {e.Message}");
        }
    }
    
    /// <summary>
    /// 파일에서 채팅 내역 로드
    /// </summary>
    public ChatHistoryData LoadChatHistory()
    {
        try
        {
            if (File.Exists(filePath))
            {
                string json = File.ReadAllText(filePath);
                chatHistory = JsonUtility.FromJson<ChatHistoryData>(json);
                
                if (chatHistory == null)
                    chatHistory = new ChatHistoryData();
                    
                Debug.Log($"채팅 내역 로드 완료. 총 {chatHistory.messages.Count}개 메시지");
                return chatHistory;
            }
            else
            {
                Debug.Log("저장된 채팅 내역이 없습니다.");
                chatHistory = new ChatHistoryData();
                return chatHistory;
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"채팅 내역 로드 실패: {e.Message}");
            chatHistory = new ChatHistoryData();
            return chatHistory;
        }
    }
    
    /// <summary>
    /// 채팅 내역 전체 삭제
    /// </summary>
    public void ClearChatHistory()
    {
        chatHistory.messages.Clear();
        SaveChatHistory();
        Debug.Log("채팅 내역이 삭제되었습니다.");
    }
    
    /// <summary>
    /// 현재 메모리의 채팅 내역 반환
    /// </summary>
    public List<ChatMessage> GetCurrentMessages()
    {
        return chatHistory.messages;
    }
    
    /// <summary>
    /// 저장된 메시지 개수 반환
    /// </summary>
    public int GetMessageCount()
    {
        return chatHistory.messages.Count;
    }
}