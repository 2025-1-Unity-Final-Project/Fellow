using UnityEngine;
using TMPro;

public class NPCAffinityUI : MonoBehaviour
{
    public TextMeshProUGUI affinityText; // Inspector���� AffinityText ����
    public GameObject medal1; // Inspector���� Medal1 ������Ʈ ����
    public GameObject medal2; // Inspector���� Medal2 ������Ʈ ����
    public GameObject medal3; // Inspector���� Medal3 ������Ʈ ����

    void Start()
    {
        if (GameManager.Instance != null && affinityText != null)
        {
            affinityText.text = GameManager.Instance.affinity.ToString();
        }

        // ��� �޴� ��Ȱ��ȭ
        if (medal1 != null) medal1.SetActive(false);
        if (medal2 != null) medal2.SetActive(false);
        if (medal3 != null) medal3.SetActive(false);

        // ȣ���� ���� ���� �ش� �޴޸� Ȱ��ȭ
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
            // �ʿ��ϴٸ� 3 �̻��� �� medal3 ��� ǥ��
            default:
                if (GameManager.Instance.affinity > 3 && medal3 != null) medal3.SetActive(true);
                break;
        }
    }
}
