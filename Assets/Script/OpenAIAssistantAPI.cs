using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

[System.Serializable]
public class CreateThreadResponse
{
    public string id;
    public string @object;
    public long created_at;
}

[System.Serializable]
public class CreateMessageRequest
{
    public string role;
    public string content;
    
    public CreateMessageRequest(string role, string content)
    {
        this.role = role;
        this.content = content;
    }
}

[System.Serializable]
public class CreateRunRequest
{
    public string assistant_id;
    
    public CreateRunRequest(string assistantId)
    {
        this.assistant_id = assistantId;
    }
}

[System.Serializable]
public class RunResponse
{
    public string id;
    public string thread_id;
    public string assistant_id;
    public string status;
    public long created_at;
}

[System.Serializable]
public class MessageContent
{
    public string type;
    public MessageText text;
}

[System.Serializable]
public class MessageText
{
    public string value;
}

[System.Serializable]
public class MessageData
{
    public string id;
    public string role;
    public MessageContent[] content;
    public long created_at;
}

[System.Serializable]
public class ListMessagesResponse
{
    public string @object;
    public MessageData[] data;
}

public class OpenAIAssistantAPI : MonoBehaviour
{
    [Header("API Settings - 인스펙터에서 직접 입력하세요")]
    [SerializeField] private string apiKey = ""; // 인스펙터에서 API 키 입력
    [SerializeField] private string assistantId = ""; // 인스펙터에서 Assistant ID 입력
    
    private const string BASE_URL = "https://api.openai.com/v1";
    private string currentThreadId = "";
    
    void Start()
    {
        // API 키 검증
        if (string.IsNullOrEmpty(apiKey) || string.IsNullOrEmpty(assistantId))
        {
            Debug.LogError("API 키와 Assistant ID를 인스펙터에서 설정해주세요!");
            return;
        }
        
        // 스레드 생성
        StartCoroutine(CreateThread());
    }
    
    private IEnumerator CreateThread()
    {
        string url = $"{BASE_URL}/threads";
        
        using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
        {
            request.downloadHandler = new DownloadHandlerBuffer();
            request.uploadHandler = new UploadHandlerRaw(new byte[0]);
            
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Authorization", $"Bearer {apiKey}");
            request.SetRequestHeader("OpenAI-Beta", "assistants=v2");
            
            yield return request.SendWebRequest();
            
            if (request.result == UnityWebRequest.Result.Success)
            {
                CreateThreadResponse response = JsonUtility.FromJson<CreateThreadResponse>(request.downloadHandler.text);
                currentThreadId = response.id;
                Debug.Log($"스레드 생성 완료: {currentThreadId}");
            }
            else
            {
                Debug.LogError($"스레드 생성 실패: {request.error}");
                Debug.LogError($"응답: {request.downloadHandler.text}");
            }
        }
    }
    
    public void SendMessage(string userMessage, System.Action<string> onResponse, System.Action<string> onError = null)
    {
        if (string.IsNullOrEmpty(currentThreadId))
        {
            onError?.Invoke("스레드가 생성되지 않았습니다.");
            return;
        }
        
        StartCoroutine(SendMessageCoroutine(userMessage, onResponse, onError));
    }
    
    private IEnumerator SendMessageCoroutine(string userMessage, System.Action<string> onResponse, System.Action<string> onError)
    {
        // 1. 메시지 추가
        yield return StartCoroutine(AddMessageToThread(userMessage));
        
        // 2. Run 생성
        string runId = "";
        yield return StartCoroutine(CreateRun((id) => runId = id, onError));
        
        if (string.IsNullOrEmpty(runId))
        {
            onError?.Invoke("Run 생성에 실패했습니다.");
            yield break;
        }
        
        // 3. Run 완료 대기
        yield return StartCoroutine(WaitForRunCompletion(runId, onError));
        
        // 4. 메시지 가져오기
        yield return StartCoroutine(GetMessages(onResponse, onError));
    }
    
    private IEnumerator AddMessageToThread(string message)
    {
        string url = $"{BASE_URL}/threads/{currentThreadId}/messages";
        
        CreateMessageRequest messageRequest = new CreateMessageRequest("user", message);
        string jsonData = JsonUtility.ToJson(messageRequest);
        
        using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Authorization", $"Bearer {apiKey}");
            request.SetRequestHeader("OpenAI-Beta", "assistants=v2");
            
            yield return request.SendWebRequest();
            
            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"메시지 추가 실패: {request.error}");
                Debug.LogError($"응답: {request.downloadHandler.text}");
            }
        }
    }
    
    private IEnumerator CreateRun(System.Action<string> onRunCreated, System.Action<string> onError)
    {
        string url = $"{BASE_URL}/threads/{currentThreadId}/runs";
        
        CreateRunRequest runRequest = new CreateRunRequest(assistantId);
        string jsonData = JsonUtility.ToJson(runRequest);
        
        using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Authorization", $"Bearer {apiKey}");
            request.SetRequestHeader("OpenAI-Beta", "assistants=v2");
            
            yield return request.SendWebRequest();
            
            if (request.result == UnityWebRequest.Result.Success)
            {
                RunResponse response = JsonUtility.FromJson<RunResponse>(request.downloadHandler.text);
                onRunCreated?.Invoke(response.id);
            }
            else
            {
                Debug.LogError($"Run 생성 실패: {request.error}");
                Debug.LogError($"응답: {request.downloadHandler.text}");
                onError?.Invoke("Run 생성에 실패했습니다.");
            }
        }
    }
    
    private IEnumerator WaitForRunCompletion(string runId, System.Action<string> onError)
    {
        string url = $"{BASE_URL}/threads/{currentThreadId}/runs/{runId}";
        
        while (true)
        {
            using (UnityWebRequest request = UnityWebRequest.Get(url))
            {
                request.SetRequestHeader("Authorization", $"Bearer {apiKey}");
                request.SetRequestHeader("OpenAI-Beta", "assistants=v2");
                
                yield return request.SendWebRequest();
                
                if (request.result == UnityWebRequest.Result.Success)
                {
                    RunResponse response = JsonUtility.FromJson<RunResponse>(request.downloadHandler.text);
                    
                    if (response.status == "completed")
                    {
                        break;
                    }
                    else if (response.status == "failed" || response.status == "cancelled" || response.status == "expired")
                    {
                        onError?.Invoke($"Run 실패: {response.status}");
                        yield break;
                    }
                }
                else
                {
                    Debug.LogError($"Run 상태 확인 실패: {request.error}");
                }
            }
            
            yield return new WaitForSeconds(1f);
        }
    }
    
    private IEnumerator GetMessages(System.Action<string> onResponse, System.Action<string> onError)
    {
        string url = $"{BASE_URL}/threads/{currentThreadId}/messages?order=desc&limit=1";
        
        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            request.SetRequestHeader("Authorization", $"Bearer {apiKey}");
            request.SetRequestHeader("OpenAI-Beta", "assistants=v2");
            
            yield return request.SendWebRequest();
            
            if (request.result == UnityWebRequest.Result.Success)
            {
                ListMessagesResponse response = JsonUtility.FromJson<ListMessagesResponse>(request.downloadHandler.text);
                
                if (response.data != null && response.data.Length > 0)
                {
                    MessageData latestMessage = response.data[0];
                    if (latestMessage.role == "assistant" && latestMessage.content != null && latestMessage.content.Length > 0)
                    {
                        string aiResponse = latestMessage.content[0].text.value;
                        onResponse?.Invoke(aiResponse);
                    }
                    else
                    {
                        onError?.Invoke("Assistant 응답을 찾을 수 없습니다.");
                    }
                }
                else
                {
                    onError?.Invoke("메시지를 찾을 수 없습니다.");
                }
            }
            else
            {
                Debug.LogError($"메시지 조회 실패: {request.error}");
                Debug.LogError($"응답: {request.downloadHandler.text}");
                onError?.Invoke("메시지 조회에 실패했습니다.");
            }
        }
    }
    
    public void ClearConversation()
    {
        StartCoroutine(CreateThread());
    }
}