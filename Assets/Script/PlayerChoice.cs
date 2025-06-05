// �� Ŭ�������� NPCDialogue.cs ���� ����̳� ������ ���Ͽ� ������ �� �ֽ��ϴ�.
// [System.Serializable] ��Ʈ����Ʈ�� �߰��ϸ� ����Ƽ �ν����Ϳ��� ���� �����մϴ�.

using System.Collections.Generic;

[System.Serializable]
public class PlayerChoice
{
    public string choiceText;          // �÷��̾ ������ ����
    public int nextSegmentIndex = -1;  // �� �������� ����� �� �̵��� ���� DialogueSegment�� �ε��� (-1�̸� ��ȭ ����)
    // �ʿ��ϴٸ�, �� �������� ����� �� Ư�� �̺�Ʈ�� �߻��ϵ��� �ݹ� ���� �߰��� ���� �ֽ��ϴ�.
    // public UnityEngine.Events.UnityEvent onChoiceSelectedAction;
}

[System.Serializable]
public class DialogueSegment
{
    public string npcLine;              // NPC�� ���
    public List<PlayerChoice> choices;  // �÷��̾�� �־��� ������ ��� (������� �� ����)
    public bool endsDialogueAfterLine = false; // �� ��� �� ������ ���� ��ȭ�� ����Ǵ��� ���� (�������� ������ ���õ�)
}
