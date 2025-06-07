using UnityEngine;
using TMPro;

public class NPCAffinityUI : MonoBehaviour
{
    public TextMeshProUGUI affinityText; // Inspector에서 AffinityText 연결
    public GameObject medal1; // Inspector에서 Medal1 오브젝트 연결
    public GameObject medal2; // Inspector에서 Medal2 오브젝트 연결
    public GameObject medal3; // Inspector에서 Medal3 오브젝트 연결

    void Start()
    {
        if (GameManager.Instance != null && affinityText != null)
        {
            affinityText.text = GameManager.Instance.affinity.ToString();
        }

        // 모든 메달 비활성화
        if (medal1 != null) medal1.SetActive(false);
        if (medal2 != null) medal2.SetActive(false);
        if (medal3 != null) medal3.SetActive(false);

        // 호감도 값에 따라 해당 메달만 활성화
        switch (GameManager.Instance.affinity)
        {
            case 1:
                if (medal1 != null) medal1.SetActive(true);
                break;
            case 2:
                if (medal2 != null) medal2.SetActive(true);
                break;
            case 3:
                if (medal3 != null) medal3.SetActive(true);
                break;
            // 필요하다면 3 이상일 때 medal3 계속 표시
            default:
                if (GameManager.Instance.affinity > 3 && medal3 != null) medal3.SetActive(true);
                break;
        }
    }
}
