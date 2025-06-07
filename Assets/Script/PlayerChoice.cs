
using System.Collections.Generic;

[System.Serializable]
public class PlayerChoice
{
    public string choiceText;          // �÷��̾ ������ ����
    public int nextSegmentIndex = -1;  // �� �������� ����� �� �̵��� ���� DialogueSegment�� �ε��� (-1�̸� ��ȭ ����)
   
}

[System.Serializable]
public class DialogueSegment
{
    public string npcLine;              // NPC�� ���
    public List<PlayerChoice> choices;  // �÷��̾�� �־��� ������ ��� (������� �� ����)
    public bool endsDialogueAfterLine = false; // �� ��� �� ������ ���� ��ȭ�� ����Ǵ��� ���� (�������� ������ ���õ�)
}
