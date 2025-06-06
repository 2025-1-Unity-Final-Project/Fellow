
using System.Collections.Generic;

[System.Serializable]
public class PlayerChoice
{
    public string choiceText;          // 플레이어가 선택할 문구
    public int nextSegmentIndex = -1;  // 이 선택지를 골랐을 때 이동할 다음 DialogueSegment의 인덱스 (-1이면 대화 종료)
   
}

[System.Serializable]
public class DialogueSegment
{
    public string npcLine;              // NPC의 대사
    public List<PlayerChoice> choices;  // 플레이어에게 주어질 선택지 목록 (비어있을 수 있음)
    public bool endsDialogueAfterLine = false; // 이 대사 후 선택지 없이 대화가 종료되는지 여부 (선택지가 있으면 무시됨)
}
