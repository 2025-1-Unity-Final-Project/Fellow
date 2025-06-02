using UnityEngine;
using TMPro;
using System;

public class TimeDisplay : MonoBehaviour
{
    public TextMeshProUGUI timeText;
    
    void Start()
    {
        // 1초마다 시간 업데이트
        InvokeRepeating("UpdateTime", 0f, 1f);
    }

    void UpdateTime()
    {
        // 한국 시간 (UTC+9) 직접 계산
        DateTime utcTime = DateTime.UtcNow;
        DateTime koreaTime = utcTime.AddHours(9);

        // 시간 포맷 설정 (HH:mm 또는 HH:mm:ss)
        string timeString = koreaTime.ToString("HH:mm");
        timeText.text = timeString;

        // 날짜 포함 버전
        string dateTimeString = koreaTime.ToString("MM/dd HH:mm");
        timeText.text = dateTimeString;
    }
}