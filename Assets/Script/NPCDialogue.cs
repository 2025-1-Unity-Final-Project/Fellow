// NPCDialogue.cs
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

namespace ClearSky
{
    public class NPCDialogue : MonoBehaviour
    {
        [Header("Dialogue Data")]
        public string npcName = "NPC";
        public List<DialogueSegment> dialogueSegments;

        [Header("UI References")]
        public GameObject dialoguePanel;
        public TextMeshProUGUI nameText;
        public TextMeshProUGUI dialogueText;
        public Button continueButton;
        public List<Button> choiceButtons;

        [Header("External Panels")] // ���ο� ���� �߰�
        public GameObject affinityPanel; // ���⿡ AffinityPanel ������Ʈ�� �����մϴ�.

        private int currentSegmentIndex = 0;
        private SimplePlayerController playerController; // ������ ����ϴ� ����, �ʿ� ���ٸ� ���� ����

        // �� ������ �߰��Ͽ�, � ���׸�Ʈ�� ȣ���� �г��� ����� �ϴ��� �����մϴ�.
        // ����: Element 3 (�ε��� 3)�� ȣ���� �г��� ���� ���׸�Ʈ��� �����մϴ�.
        // �ν����Ϳ��� �� ���� �����ϰų�, �� ������ �������� ������ ���� �ֽ��ϴ�.
        // ������ �����ϰ�, "���� ȣ������ �����ٰ�" ���׸�Ʈ�� �ε����� 3�̶�� �����ϰ� �ڵ忡 �ݿ��մϴ�.
        private const int AFFINITY_TRIGGER_SEGMENT_INDEX = 3;

        void Start()
        {
            if (dialoguePanel != null) dialoguePanel.SetActive(false);
            if (affinityPanel != null) affinityPanel.SetActive(false); // AffinityPanel�� ���� �� ��Ȱ��ȭ

            if (continueButton != null) continueButton.onClick.RemoveAllListeners();

            foreach (Button choiceBtn in choiceButtons)
            {
                if (choiceBtn != null)
                {
                    choiceBtn.gameObject.SetActive(false);
                    choiceBtn.onClick.RemoveAllListeners();
                }
            }
            playerController = FindObjectOfType<SimplePlayerController>();
        }

        public void StartDialogue()
        {
            // (���� StartDialogue �Լ� ����� ���� ����, affinityPanel ���� ���� �߰� ����)
            if (dialogueSegments == null || dialogueSegments.Count == 0)
            {
                Debug.LogError(gameObject.name + ": ��ȭ ���׸�Ʈ�� �������� �ʾҽ��ϴ�.");
                return;
            }
            if (dialoguePanel == null || dialogueText == null)
            {
                Debug.LogError("��ȭ UI ��Ұ� NPCDialogue ��ũ��Ʈ�� ������� �ʾҽ��ϴ�!");
                return;
            }

            // Ȥ�� AffinityPanel�� �����ִٸ� �ݾ��ݴϴ� (������)
            if (affinityPanel != null && affinityPanel.activeSelf)
            {
                affinityPanel.SetActive(false);
            }

            dialoguePanel.SetActive(true);
            currentSegmentIndex = 0;
            DisplaySegment(dialogueSegments[currentSegmentIndex]);
        }

        void DisplaySegment(DialogueSegment segment)
        {
            // (���� DisplaySegment �Լ� ����� ����)
            if (nameText != null) nameText.text = npcName;
            dialogueText.text = segment.npcLine;

            foreach (Button choiceBtn in choiceButtons)
            {
                if (choiceBtn != null)
                {
                    choiceBtn.gameObject.SetActive(false);
                    choiceBtn.onClick.RemoveAllListeners();
                }
            }

            if (continueButton != null)
            {
                continueButton.gameObject.SetActive(false);
                continueButton.onClick.RemoveAllListeners();
            }

            if (segment.choices != null && segment.choices.Count > 0)
            {
                for (int i = 0; i < segment.choices.Count; i++)
                {
                    if (i < choiceButtons.Count && choiceButtons[i] != null)
                    {
                        choiceButtons[i].gameObject.SetActive(true);
                        choiceButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = segment.choices[i].choiceText;

                        PlayerChoice currentChoice = segment.choices[i];
                        choiceButtons[i].onClick.AddListener(() => OnChoiceSelected(currentChoice));
                    }
                }
            }
            else
            {
                if (segment.endsDialogueAfterLine)
                {
                    if (continueButton != null)
                    {
                        continueButton.gameObject.SetActive(true);
                        continueButton.GetComponentInChildren<TextMeshProUGUI>().text = "�ݱ�";
                        continueButton.onClick.AddListener(EndDialogue); // <--- EndDialogue ȣ��
                    }
                }
                else
                {
                    if (continueButton != null)
                    {
                        continueButton.gameObject.SetActive(true);
                        continueButton.GetComponentInChildren<TextMeshProUGUI>().text = "���";
                        continueButton.onClick.AddListener(ShowNextImplicitSegment);
                    }
                }
            }
        }

        void OnChoiceSelected(PlayerChoice choice)
        {
            // (���� OnChoiceSelected �Լ� ����� ����)
            if (choice.nextSegmentIndex < 0 || choice.nextSegmentIndex >= dialogueSegments.Count)
            {
                EndDialogue(); // <--- EndDialogue ȣ�� (�̶��� currentSegmentIndex�� Ȯ���ؾ� �� �� ����)
            }
            else
            {
                currentSegmentIndex = choice.nextSegmentIndex;
                DisplaySegment(dialogueSegments[currentSegmentIndex]);
            }
        }

        void ShowNextImplicitSegment()
        {
            // (���� ShowNextImplicitSegment �Լ� ����� ����)
            currentSegmentIndex++;
            if (currentSegmentIndex < dialogueSegments.Count)
            {
                DisplaySegment(dialogueSegments[currentSegmentIndex]);
            }
            else
            {
                EndDialogue(); // <--- EndDialogue ȣ��
            }
        }

        void EndDialogue()
        {
            // **�� �κ��� �߿��ϰ� ����˴ϴ�**
            int endedSegmentIndex = currentSegmentIndex; // ��ȭ�� ����� ���� ���׸�Ʈ �ε��� ����

            if (dialoguePanel != null)
            {
                dialoguePanel.SetActive(false);
            }
            currentSegmentIndex = 0; // ���� ��ȭ�� ���� �ʱ�ȭ

            // Ư�� ���׸�Ʈ���� ��ȭ�� ����Ǿ��� �� AffinityPanel�� ���ϴ�.
            // AFFINITY_TRIGGER_SEGMENT_INDEX�� '���� ȣ������ �����ٰ�' ���׸�Ʈ�� �ε������� �մϴ�.
            // ����ڴ��� ���������� Element 3�̹Ƿ�, �ε����� 3�Դϴ�.
            if (endedSegmentIndex == AFFINITY_TRIGGER_SEGMENT_INDEX && affinityPanel != null)
            {
                affinityPanel.SetActive(true);
                // ���⿡ AffinityPanel�� ���� ȣ���� �����͸� �����ϰ� ������Ʈ�ϴ� ������ �߰��� �� �ֽ��ϴ�.
                // ��: affinityPanel.GetComponent<AffinityDisplayScript>().ShowAffinity(someAffinityValue);
            }
        }
    }
}
