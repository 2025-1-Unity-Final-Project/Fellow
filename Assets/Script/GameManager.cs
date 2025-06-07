// GameManager.cs
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("씬 이름")]
    public string npcSceneName = "NPCScene";
    public string gamePlaySceneName = "GamePlayScene"; // ▼▼▼ GamePlayScene 이름 설정 ▼▼▼

    [Header("UI")]
    // ▼▼▼ Inspector에서 연결은 하지만, 실제 사용은 OnSceneLoaded에서 재할당될 수 있음 ▼▼▼
    public GameObject successPanel;

    [Header("호감도(친밀도)")]
    public int affinity = 0;

    private int enemyCount = 0;
    private bool levelComplete = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded; // 씬 로드 이벤트 구독
        }
        else
        {
            Destroy(gameObject); // 중복 인스턴스 파괴
        }
    }

    // ▼▼▼ 씬이 로드될 때마다 호출되는 함수 ▼▼▼
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("OnSceneLoaded 호출됨. 로드된 씬: " + scene.name);

        // 현재 씬이 GamePlayScene인지 확인
        if (scene.name == gamePlaySceneName)
        {
            Debug.Log(gamePlaySceneName + " 로드됨. SuccessPanel 찾기 시도...");
     
            GameObject panelInScene = GameObject.Find("SuccessPanel");

            if (panelInScene != null)
            {
                successPanel = panelInScene; // 찾은 오브젝트로 successPanel 참조 업데이트
                successPanel.SetActive(false); // 찾았다면 기본적으로 비활성화
                Debug.Log("'SuccessPanel' 찾음 및 참조 할당 완료. 기본 비활성화됨.");
            }
            else
            {
                Debug.LogError(gamePlaySceneName + "에서 'SuccessPanel'이라는 이름의 활성화된 오브젝트를 찾을 수 없습니다! Hierarchy 창의 이름과 활성화 상태를 확인하세요.");
                successPanel = null; // 명시적으로 null 처리하여 이후 코드에서 NullReferenceException 방지
            }

            // GamePlayScene 관련 초기화
            InitializeLevel();
        }
        else
        {
            
            if (successPanel != null && gameObject.scene.name != gamePlaySceneName)
            {
            
            }
        }
    }

    // 레벨(씬) 시작 시 필요한 초기화 로직
    void InitializeLevel()
    {
        enemyCount = GameObject.FindGameObjectsWithTag("Enemy").Length;
        levelComplete = false;

        if (successPanel != null)
        {
            successPanel.SetActive(false);

            // SuccessPanel 하위에 있는 Button 컴포넌트 찾기
            Button nextButton = successPanel.GetComponentInChildren<Button>();
            if (nextButton != null)
            {
                nextButton.onClick.RemoveAllListeners(); // 중복 방지
                nextButton.onClick.AddListener(LoadNPCScene); // 이벤트 연결
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
        Debug.Log("남은 적 수: " + enemyCount);

        if (enemyCount <= 0 && !levelComplete)
        {
            levelComplete = true;
            StartCoroutine(ShowSuccessPanelAndLoadNPCScene());
        }
    }

    IEnumerator ShowSuccessPanelAndLoadNPCScene()
    {
        Debug.Log("모든 적 처치! 게임 클리어!");
        if (successPanel != null)
        {
            successPanel.SetActive(true);
        }
        else
        {
            Debug.LogError("ShowSuccessPanelAndLoadNPCScene: SuccessPanel 참조가 null입니다! OnSceneLoaded에서 찾지 못했거나, 다른 문제입니다.");
        }
        yield return new WaitForSeconds(2f);
        // LoadNPCScene(); // 버튼 클릭으로 이동하도록 주석 처리
    }

    public void LoadNPCScene()
    {
        SceneManager.LoadScene(npcSceneName);
        Time.timeScale = 1f;
        Debug.Log("NPC 씬으로 이동: " + npcSceneName);
    }

    public void RetryGame()
    {
        if (successPanel != null)
        {
            successPanel.SetActive(false);
        }
        levelComplete = false;
        Time.timeScale = 1f;
        // 현재 씬을 다시 로드합니다. OnSceneLoaded가 다시 호출되어 필요한 초기화를 수행합니다.
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Debug.Log("Retry 버튼 클릭됨: " + SceneManager.GetActiveScene().name + " 씬 재시작");
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded; // 이벤트 구독 해제
    }

    public void GoToMainMenu() { /* ... */ }
    public void OnNPCButtonClick() { /* ... */ }
}
