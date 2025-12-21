using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class EndLevelUI : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private GameObject endLevelPanel;
    [SerializeField] private TMP_Text difficultyText;
    [SerializeField] private TMP_Text currentScoreText;
    [SerializeField] private TMP_Text highScoreText;
    [SerializeField] private TMP_Text playTimeText;
    [SerializeField] private TMP_Text victimsRescuedText;
    [SerializeField] private TMP_Text enemiesHitText;
    [SerializeField] private TMP_Text enemiesKilledText;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button menuButton;
    
    [Header("Scene Settings")]
    [SerializeField] private string menuSceneName = "MainMenu";
    [SerializeField] private string firstSceneName = "Scene1";
    
    private void OnEnable()
    {
        SubscribeToEvents();
    }
    
    private void OnDisable()
    {
        UnsubscribeFromEvents();
    }
    
    private void Start()
    {
        if (endLevelPanel != null)
        {
            endLevelPanel.SetActive(false);
        }
        
        if (restartButton != null)
        {
            restartButton.onClick.AddListener(OnRestart);
        }
        else
        {
            Debug.LogError("EndLevelUI: Restart button is null! Please assign it in Inspector!");
        }
        
        if (menuButton != null)
        {
            menuButton.onClick.AddListener(OnMenu);
        }
        else
        {
            Debug.LogError("EndLevelUI: Menu button is null! Please assign it in Inspector!");
        }
        
        SubscribeToEvents();
        
        Debug.Log("EndLevelUI: Started and subscribed to ScoreManager events");
    }
    
    private void SubscribeToEvents()
    {
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.OnLevelEnd += ShowEndLevelPanel;
        }
        else
        {
            Debug.LogWarning("EndLevelUI: ScoreManager.Instance is null! Cannot subscribe to events.");
        }
    }
    
    private void UnsubscribeFromEvents()
    {
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.OnLevelEnd -= ShowEndLevelPanel;
        }
    }
    
    private void ShowEndLevelPanel()
    {
        Debug.Log("EndLevelUI: ShowEndLevelPanel called!");
        
        if (endLevelPanel != null)
        {
            endLevelPanel.SetActive(true);
        }
        else
        {
            Debug.LogError("EndLevelUI: endLevelPanel is null! Please assign it in Inspector!");
        }
        
        if (ScoreManager.Instance != null)
        {
            if (difficultyText != null)
            {
                DifficultyManager.Difficulty difficulty = DifficultyManager.GetCurrentDifficulty();
                difficultyText.text = $"Difficulty: {difficulty}";
            }
            
            if (currentScoreText != null)
            {
                currentScoreText.text = $"Score: {ScoreManager.Instance.GetCurrentScore()}";
            }
            
            if (highScoreText != null)
            {
                int highScore = ScoreManager.Instance.GetHighScore();
                highScoreText.text = $"High Score: {highScore}";
            }
            else
            {
                Debug.LogError("EndLevelUI: highScoreText is NULL!");
            }
            
            if (playTimeText != null)
            {
                string playTime = ScoreManager.Instance.GetPlayTimeFormatted();
                playTimeText.text = $"Time: {playTime}";
            }
            
            if (victimsRescuedText != null)
            {
                victimsRescuedText.text = $"Victims Rescued: {ScoreManager.Instance.GetVictimsInSafeZone()}";
            }
            
            if (enemiesHitText != null)
            {
                int hitPenalty = ScoreManager.Instance.GetTotalHitPenalty();
                enemiesHitText.text = $"Enemies Hit: -{hitPenalty}";
            }
            
            if (enemiesKilledText != null)
            {
                int killPenalty = ScoreManager.Instance.GetTotalKillPenalty();
                enemiesKilledText.text = $"Enemies Killed: -{killPenalty}";
            }
        }
        
        Time.timeScale = 0f;
        
        if (PlayerController.Instance != null)
        {
            PlayerController.Instance.DisableInput();
        }
        
        if (ActiveWeapon.Instance != null)
        {
            ActiveWeapon.Instance.DisableInput();
        }
        
        if (ActiveInventory.Instance != null)
        {
            ActiveInventory.Instance.DisableInput();
        }
        
        UnityEngine.EventSystems.EventSystem eventSystem = UnityEngine.EventSystems.EventSystem.current;
        if (eventSystem == null)
        {
            eventSystem = FindObjectOfType<UnityEngine.EventSystems.EventSystem>();
        }
        
        if (eventSystem == null)
        {
            GameObject eventSystemObj = new GameObject("EventSystem");
            eventSystem = eventSystemObj.AddComponent<UnityEngine.EventSystems.EventSystem>();
            eventSystemObj.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
        }
        
        if (eventSystem != null)
        {
            eventSystem.enabled = true;
        }
        else
        {
            Debug.LogError("EndLevelUI: Failed to create EventSystem! UI buttons will not work.");
        }
        
        Canvas canvas = endLevelPanel.GetComponentInParent<Canvas>();
        if (canvas == null)
        {
            canvas = FindObjectOfType<Canvas>();
        }
        
        if (canvas != null)
        {
            UnityEngine.UI.GraphicRaycaster raycaster = canvas.GetComponent<UnityEngine.UI.GraphicRaycaster>();
            if (raycaster == null)
            {
                canvas.gameObject.AddComponent<UnityEngine.UI.GraphicRaycaster>();
            }
            
            if (canvas.renderMode == RenderMode.WorldSpace)
            {
                Debug.LogWarning("EndLevelUI: Canvas is in World Space mode. UI buttons may not work properly. Consider using Screen Space - Overlay.");
            }
        }
        else
        {
            Debug.LogError("EndLevelUI: Canvas not found! UI buttons will not work.");
        }
        
        if (restartButton == null)
        {
            Debug.LogError("EndLevelUI: Restart button is NULL! Please assign it in Inspector!");
        }
        else
        {
            restartButton.interactable = true;
            Debug.Log("EndLevelUI: Restart button is interactable");
        }
        
        if (menuButton == null)
        {
            Debug.LogError("EndLevelUI: Menu button is NULL! Please assign it in Inspector!");
        }
        else
        {
            menuButton.interactable = true;
            Debug.Log("EndLevelUI: Menu button is interactable");
        }
        if (BackgroundMusicManager.Instance != null)
        {
            BackgroundMusicManager.Instance.PlayMenuMusic();
        }
        else
        {
            Debug.LogWarning("EndLevelUI: BackgroundMusicManager.Instance is null! Cannot switch music.");
        }
        
        Debug.Log("EndLevelUI: Game paused, input disabled");
    }
    
    private void OnRestart()
    {
        Debug.Log("EndLevelUI: Restart button clicked!");
        StartCoroutine(FadeAndRestart());
    }
    private IEnumerator FadeAndRestart()
    {
        Time.timeScale = 1f;
        if (UIFade.Instance != null)
        {
            UIFade.Instance.FadeToBlack();
            Debug.Log("EndLevelUI: Fading to black...");
            yield return new WaitForSeconds(1f);
        }
        if (endLevelPanel != null)
        {
            endLevelPanel.SetActive(false);
        }
        if (PlayerController.Instance != null)
        {
            PlayerController.Instance.EnableInput();
        }
        
        if (ActiveWeapon.Instance != null)
        {
            ActiveWeapon.Instance.EnableInput();
        }
        
        if (ActiveInventory.Instance != null)
        {
            ActiveInventory.Instance.EnableInput();
        }
        
        DestroyPersistentObjects();
        SceneManager.LoadScene(firstSceneName);
    }
    private void DestroyPersistentObjects()
    {
        GameObject[] rootObjects = gameObject.scene.GetRootGameObjects();
        GameObject temp = new GameObject("Temp");
        DontDestroyOnLoad(temp);
        UnityEngine.SceneManagement.Scene dontDestroyScene = temp.scene;
        Destroy(temp);
        
        GameObject[] dontDestroyObjects = dontDestroyScene.GetRootGameObjects();
        
        Debug.Log($"EndLevelUI: Found {dontDestroyObjects.Length} DontDestroyOnLoad objects to destroy");
        
        foreach (GameObject obj in dontDestroyObjects)
        {
            Debug.Log($"EndLevelUI: Destroying {obj.name}");
            Destroy(obj);
        }
    }
    private void OnMenu()
    {
        Debug.Log("EndLevelUI: Menu button clicked!");
        StartCoroutine(FadeAndLoadMenu());
    }
    private IEnumerator FadeAndLoadMenu()
    {
        
        Time.timeScale = 1f;
        if (UIFade.Instance != null)
        {
            UIFade.Instance.FadeToBlack();
            Debug.Log("EndLevelUI: Fading to black...");
            yield return new WaitForSeconds(1f);
        }
        if (endLevelPanel != null)
        {
            endLevelPanel.SetActive(false);
        }
        if (PlayerController.Instance != null)
        {
            PlayerController.Instance.EnableInput();
        }
        
        if (ActiveWeapon.Instance != null)
        {
            ActiveWeapon.Instance.EnableInput();
        }
        
        if (ActiveInventory.Instance != null)
        {
            ActiveInventory.Instance.EnableInput();
        }
        DestroyPersistentObjects();
        SceneManager.LoadScene(menuSceneName);
    }
}

