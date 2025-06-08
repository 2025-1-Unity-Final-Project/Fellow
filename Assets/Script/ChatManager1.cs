using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;

public class ChatManager1 : MonoBehaviour
{
    public GameObject YellowArea, WhiteArea, DateArea;
    public RectTransform ContentRect;
    public Scrollbar scrollBar;
    //public Toggle MineToggle;
    
    [Header("새로고침 버튼 ⭐ 새로 추가")]
    public Button refreshButton;  // 새로고침 버튼
    public Button clearButton;    // 내역 삭제 버튼 (선택사항)
    
    AreaScript LastArea;

    void Start()
    {
        // 새로고침 버튼 이벤트 연결 ⭐ 새로 추가
        if (refreshButton != null)
            refreshButton.onClick.AddListener(LoadChatHistory);
            
        if (clearButton != null)
            clearButton.onClick.AddListener(ClearChatHistory);
        
        // 씬 시작 시 자동으로 채팅 내역 로드
        LoadChatHistory();
    }

    /// <summary>
    /// 저장된 채팅 내역 로드 및 표시 ⭐ 새로 추가
    /// </summary>
    public void LoadChatHistory()
    {
        Debug.Log("채팅 내역 로드 시작");
        
        // 기존 메시지들 모두 삭제
        ClearCurrentMessages();
        
        // 저장된 데이터 로드
        ChatHistoryData historyData = ChatDataManager.Instance.LoadChatHistory();
        
        if (historyData.messages.Count == 0)
        {
            Debug.Log("표시할 채팅 내역이 없습니다.");
            return;
        }
        
        // 각 메시지를 순서대로 표시
        StartCoroutine(DisplayMessagesSequentially(historyData.messages));
    }
    
// LoadChatHistory 메서드 수정 - DisplayMessagesSequentially 부분만

/// <summary>
/// 메시지들을 순차적으로 표시 (데이터 형식 변환 포함)
/// </summary>
IEnumerator DisplayMessagesSequentially(List<ChatMessage> messages)
{
    foreach (ChatMessage msg in messages)
    {
        // 데이터 형식 변환 ⭐ 수정된 부분
        bool isSend = msg.isUser;  // isUser를 isSend로 사용
        string userName = msg.isUser ? "나" : "프리렌";  // AI → 타인으로 변환
        
        // ChatMessage를 Chat 메서드 형식에 맞게 변환
        Chat(isSend, msg.message, userName, null);
        yield return new WaitForSeconds(0.1f);
    }
    
    Debug.Log($"총 {messages.Count}개 메시지 로드 완료");
    
    // 스크롤을 맨 아래로
    Invoke("ScrollDelay", 0.1f);
}
    
    /// <summary>
    /// 현재 표시된 모든 메시지 삭제
    /// </summary>
    void ClearCurrentMessages()
    {
        // ContentRect의 모든 자식 오브젝트 삭제
        for (int i = ContentRect.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(ContentRect.GetChild(i).gameObject);
        }
        
        LastArea = null;
        Debug.Log("현재 표시된 메시지들을 모두 삭제했습니다.");
    }
    
    /// <summary>
    /// 채팅 내역 완전 삭제 ⭐ 새로 추가
    /// </summary>
    public void ClearChatHistory()
    {
        ChatDataManager.Instance.ClearChatHistory();
        ClearCurrentMessages();
        Debug.Log("채팅 내역이 완전히 삭제되었습니다.");
    }

    // ============ 기존 메서드들 (수정 없음) ============
    
    public void ReceiveMessage(string text)
    {
        //if (MineToggle.isOn) Chat(true, text, "나", null);
        //else Chat(false, text, "타인", null);
    }

    public void LayoutVisible(bool b)
    {
        AndroidJavaClass kotlin = new AndroidJavaClass("com.unity3d.player.SubActivity");
        kotlin.CallStatic("LayoutVisible", b);
    }

    public void Chat(bool isSend, string text, string user, Texture2D picture) 
    {
        if (text.Trim() == "") return;

        bool isBottom = scrollBar.value <= 0.00001f;

        //보내는 사람은 노랑, 받는 사람은 흰색영역을 크게 만들고 텍스트 대입
        AreaScript Area = Instantiate(isSend ? YellowArea : WhiteArea).GetComponent<AreaScript>();
        Area.transform.SetParent(ContentRect.transform, false);
        Area.BoxRect.sizeDelta = new Vector2(600, Area.BoxRect.sizeDelta.y);
        Area.TextRect.GetComponent<TMPro.TextMeshProUGUI>().text = text;
        Fit(Area.BoxRect);

        // 두 줄 이상이면 크기를 줄여가면서, 한 줄이 아래로 내려가면 바로 전 크기를 대입 
        float X = Area.TextRect.sizeDelta.x + 42;
        float Y = Area.TextRect.sizeDelta.y;
        if (Y > 49)
        {
            for (int i = 0; i < 200; i++)
            {
                Area.BoxRect.sizeDelta = new Vector2(X - i * 2, Area.BoxRect.sizeDelta.y);
                Fit(Area.BoxRect);

                if (Y != Area.TextRect.sizeDelta.y) { Area.BoxRect.sizeDelta = new Vector2(X - (i * 2) + 2, Y); break; }
            }
        }
        else Area.BoxRect.sizeDelta = new Vector2(X, Y);

        // 현재 것에 분까지 나오는 날짜와 유저이름 대입
        DateTime t = DateTime.Now;
        Area.Time = t.ToString("yyyy-MM-dd-HH-mm");
        Area.User = user;

        // 현재 것은 항상 새로운 시간 대입
        int hour = t.Hour;
        if (t.Hour == 0) hour = 12;
        else if (t.Hour > 12) hour -= 12;
        Area.TimeText.text = (t.Hour > 12 ? "PM " : "AM ") + hour + ":" + t.Minute.ToString("D2");

        // 이전 것과 같으면 이전 시간, 꼬리 없애기
        bool isSame = LastArea != null && LastArea.Time == Area.Time && LastArea.User == Area.User;
        if (isSame) LastArea.TimeText.text = "";
        Area.Tail.SetActive(!isSame);

        // 타인이 이전 것과 같으면 이미지, 이름 없애기
        if (!isSend)
        {
            Area.UserImage.gameObject.SetActive(!isSame);
            Area.UserText.gameObject.SetActive(!isSame);
            Area.UserText.text = Area.User;
            if(picture != null) Area.UserImage.sprite = Sprite.Create(picture, new Rect(0, 0, picture.width, picture.height), new Vector2(0.5f, 0.5f));
        }

        // 이전 것과 날짜가 다르면 날짜영역 보이기
        if (LastArea != null && LastArea.Time.Substring(0, 10) != Area.Time.Substring(0, 10))
        {
            Transform CurDateArea = Instantiate(DateArea).transform;
            CurDateArea.SetParent(ContentRect.transform, false);
            CurDateArea.SetSiblingIndex(CurDateArea.GetSiblingIndex() - 1);

            string week = "";
            switch (t.DayOfWeek)
                {
                    case DayOfWeek.Sunday: week = "일"; break;
                    case DayOfWeek.Monday: week = "월"; break;
                    case DayOfWeek.Tuesday: week = "화"; break;
                    case DayOfWeek.Wednesday: week = "수"; break;
                    case DayOfWeek.Thursday: week = "목"; break;
                    case DayOfWeek.Friday: week = "금"; break;
                    case DayOfWeek.Saturday: week = "토"; break;
                }
                CurDateArea.GetComponent<AreaScript>().DateText.text = t.Year + "년 " + t.Month + "월 " + t.Day + "일 " + week + "요일";
        }

        Fit(Area.BoxRect);
        Fit(Area.AreaRect);
        Fit(ContentRect);
        LastArea = Area;

        // 스크롤바가 위로 올라간 상태에서 메시지를 받으면 맨 아래로 내리지 않음
        if (!isSend && !isBottom) return;
        Invoke("ScrollDelay", 0.03f);
    }

    void Fit(RectTransform Rect) => LayoutRebuilder.ForceRebuildLayoutImmediate(Rect);

    void ScrollDelay() => scrollBar.value = 0;
}