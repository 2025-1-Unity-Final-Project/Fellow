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

        [Header("External Panels")] // 새로운 섹션 추가
        public GameObject affinityPanel; // 여기에 AffinityPanel 오브젝트를 연결합니다.

        private int currentSegmentIndex = 0;
        private SimplePlayerController playerController; 

       
        private const int AFFINITY_TRIGGER_SEGMENT_INDEX = 3;

        void Start()
        {
            if (dialoguePanel != null) dialoguePanel.SetActive(false);
            if (affinityPanel != null) affinityPanel.SetActive(false); // AffinityPanel도 시작 시 비활성화

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
            // (이전 StartDialogue 함수 내용과 거의 동일, affinityPanel 관련 로직 추가 없음)
            if (dialogueSegments == null || dialogueSegments.Count == 0)
            {
                Debug.LogError(gameObject.name + ": 대화 세그먼트가 설정되지 않았습니다.");
                return;
            }
            if (dialoguePanel == null || dialogueText == null)
            {
                Debug.LogError("대화 UI 요소가 NPCDialogue 스크립트에 연결되지 않았습니다!");
                return;
            }

            // 혹시 AffinityPanel이 열려있다면 닫아줍니다 (선택적)
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
            // (이전 DisplaySegment 함수 내용과 동일)
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
                        continueButton.GetComponentInChildren<TextMeshProUGUI>().text = "닫기";
                        continueButton.onClick.AddListener(EndDialogue); // <--- EndDialogue 호출
                    }
                }
                else
                {
                    if (continueButton != null)
                    {
                        continueButton.gameObject.SetActive(true);
                        continueButton.GetComponentInChildren<TextMeshProUGUI>().text = "계속";
                        continueButton.onClick.AddListener(ShowNextImplicitSegment);
                    }
                }
            }
        }

        void OnChoiceSelected(PlayerChoice choice)
        {
            // (이전 OnChoiceSelected 함수 내용과 동일)
            if (choice.nextSegmentIndex < 0 || choice.nextSegmentIndex >= dialogueSegments.Count)
            {
                EndDialogue(); // <--- EndDialogue 호출 (이때도 currentSegmentIndex를 확인해야 할 수 있음)
            }
            else
            {
                currentSegmentIndex = choice.nextSegmentIndex;
                DisplaySegment(dialogueSegments[currentSegmentIndex]);
            }
        }

        void ShowNextImplicitSegment()
        {
            // (이전 ShowNextImplicitSegment 함수 내용과 동일)
            currentSegmentIndex++;
            if (currentSegmentIndex < dialogueSegments.Count)
            {
                DisplaySegment(dialogueSegments[currentSegmentIndex]);
            }
            else
            {
                EndDialogue(); // <--- EndDialogue 호출
            }
        }

        void EndDialogue()
        {
            // **이 부분이 중요하게 변경됩니다**
            int endedSegmentIndex = currentSegmentIndex; // 대화가 종료될 때의 세그먼트 인덱스 저장

            if (dialoguePanel != null)
            {
                dialoguePanel.SetActive(false);
            }
            currentSegmentIndex = 0; // 다음 대화를 위해 초기화

         
            if (endedSegmentIndex == AFFINITY_TRIGGER_SEGMENT_INDEX && affinityPanel != null)
            {
                affinityPanel.SetActive(true);
           
            }
        }
    }
}
