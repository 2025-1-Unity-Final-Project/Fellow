// GameManager.cs
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("�� �̸�")]
    public string npcSceneName = "NPCScene";
    public string gamePlaySceneName = "GamePlayScene"; // ���� GamePlayScene �̸� ���� ����

    [Header("UI")]
    // ���� Inspector���� ������ ������, ���� ����� OnSceneLoaded���� ���Ҵ�� �� ���� ����
    public GameObject successPanel;

    [Header("ȣ����(ģ�е�)")]
    public int affinity = 0;

    private int enemyCount = 0;
    private bool levelComplete = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded; // �� �ε� �̺�Ʈ ����
        }
        else
        {
            Destroy(gameObject); // �ߺ� �ν��Ͻ� �ı�
        }
    }

    // ���� ���� �ε�� ������ ȣ��Ǵ� �Լ� ����
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("OnSceneLoaded ȣ���. �ε�� ��: " + scene.name);

        // ���� ���� GamePlayScene���� Ȯ��
        if (scene.name == gamePlaySceneName)
        {
            Debug.Log(gamePlaySceneName + " �ε��. SuccessPanel ã�� �õ�...");
     
            GameObject panelInScene = GameObject.Find("SuccessPanel");

            if (panelInScene != null)
            {
                successPanel = panelInScene; // ã�� ������Ʈ�� successPanel ���� ������Ʈ
                successPanel.SetActive(false); // ã�Ҵٸ� �⺻������ ��Ȱ��ȭ
                Debug.Log("'SuccessPanel' ã�� �� ���� �Ҵ� �Ϸ�. �⺻ ��Ȱ��ȭ��.");
            }
            else
            {
                Debug.LogError(gamePlaySceneName + "���� 'SuccessPanel'�̶�� �̸��� Ȱ��ȭ�� ������Ʈ�� ã�� �� �����ϴ�! Hierarchy â�� �̸��� Ȱ��ȭ ���¸� Ȯ���ϼ���.");
                successPanel = null; // ��������� null ó���Ͽ� ���� �ڵ忡�� NullReferenceException ����
            }

            // GamePlayScene ���� �ʱ�ȭ
            InitializeLevel();
        }
        else
        {
            
            if (successPanel != null && gameObject.scene.name != gamePlaySceneName)
            {
            
            }
        }
    }

    // ����(��) ���� �� �ʿ��� �ʱ�ȭ ����
    void InitializeLevel()
    {
        enemyCount = GameObject.FindGameObjectsWithTag("Enemy").Length;
        levelComplete = false;

        if (successPanel != null)
        {
            successPanel.SetActive(false);

            // SuccessPanel ������ �ִ� Button ������Ʈ ã��
            Button nextButton = successPanel.GetComponentInChildren<Button>();
            if (nextButton != null)
            {
                nextButton.onClick.RemoveAllListeners(); // �ߺ� ����
                nextButton.onClick.AddListener(LoadNPCScene); // �̺�Ʈ ����
            }
        }
    }


    void Start()
    {
       
        if (SceneManager.GetActiveScene().name == gamePlaySceneName)
        {
         
        }
    }

    public void EnemyDestroyed()
    {
        enemyCount--;
        Debug.Log("���� �� ��: " + enemyCount);

        if (enemyCount <= 0 && !levelComplete)
        {
            levelComplete = true;
            StartCoroutine(ShowSuccessPanelAndLoadNPCScene());
        }
    }

    IEnumerator ShowSuccessPanelAndLoadNPCScene()
    {
        Debug.Log("��� �� óġ! ���� Ŭ����!");
        if (successPanel != null)
        {
            successPanel.SetActive(true);
        }
        else
        {
            Debug.LogError("ShowSuccessPanelAndLoadNPCScene: SuccessPanel ������ null�Դϴ�! OnSceneLoaded���� ã�� ���߰ų�, �ٸ� �����Դϴ�.");
        }
        yield return new WaitForSeconds(2f);
        // LoadNPCScene(); // ��ư Ŭ������ �̵��ϵ��� �ּ� ó��
    }

    public void LoadNPCScene()
    {
        SceneManager.LoadScene(npcSceneName);
        Time.timeScale = 1f;
        Debug.Log("NPC ������ �̵�: " + npcSceneName);
    }

    public void RetryGame()
    {
        if (successPanel != null)
        {
            successPanel.SetActive(false);
        }
        levelComplete = false;
        Time.timeScale = 1f;
        // ���� ���� �ٽ� �ε��մϴ�. OnSceneLoaded�� �ٽ� ȣ��Ǿ� �ʿ��� �ʱ�ȭ�� �����մϴ�.
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Debug.Log("Retry ��ư Ŭ����: " + SceneManager.GetActiveScene().name + " �� �����");
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded; // �̺�Ʈ ���� ����
    }

    public void GoToMainMenu() { /* ... */ }
    public void OnNPCButtonClick() { /* ... */ }
}
