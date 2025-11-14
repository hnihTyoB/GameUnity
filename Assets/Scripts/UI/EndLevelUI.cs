using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

/// <summary>
/// UI panel displayed at end of level
/// Shows score, statistics, and high score
/// </summary>
public class EndLevelUI : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private GameObject endLevelPanel;
    [SerializeField] private TMP_Text difficultyText;
    [SerializeField] private TMP_Text currentScoreText;
    [SerializeField] private TMP_Text highScoreText;
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
        // Subscribe to score manager events
        // Note: Subscribe in OnEnable AND Start to ensure we catch the event
        SubscribeToEvents();
    }
    
    private void OnDisable()
    {
        // Unsubscribe
        UnsubscribeFromEvents();
    }
    
    private void Start()
    {
        // Hide panel initially
        if (endLevelPanel != null)
        {
            endLevelPanel.SetActive(false);
        }
        
        // Setup buttons
        if (restartButton != null)
        {
            restartButton.onClick.AddListener(OnRestart);
            Debug.Log("EndLevelUI: Restart button setup");
        }
        else
        {
            Debug.LogError("EndLevelUI: Restart button is null! Please assign it in Inspector!");
        }
        
        if (menuButton != null)
        {
            menuButton.onClick.AddListener(OnMenu);
            Debug.Log("EndLevelUI: Menu button setup");
        }
        else
        {
            Debug.LogError("EndLevelUI: Menu button is null! Please assign it in Inspector!");
        }
        
        // Subscribe again in Start to ensure we're subscribed (in case ScoreManager wasn't ready in OnEnable)
        SubscribeToEvents();
        
        Debug.Log("EndLevelUI: Started and subscribed to ScoreManager events");
    }
    
    private void SubscribeToEvents()
    {
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.OnLevelEnd += ShowEndLevelPanel;
            Debug.Log("EndLevelUI: Subscribed to OnLevelEnd event");
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
    
    /// <summary>
    /// Show end level panel with score and statistics
    /// </summary>
    private void ShowEndLevelPanel()
    {
        Debug.Log("EndLevelUI: ShowEndLevelPanel called!");
        
        if (endLevelPanel != null)
        {
            endLevelPanel.SetActive(true);
            Debug.Log("EndLevelUI: Panel activated!");
        }
        else
        {
            Debug.LogError("EndLevelUI: endLevelPanel is null! Please assign it in Inspector!");
        }
        
        // Update UI with score and statistics
        if (ScoreManager.Instance != null)
        {
            // Difficulty
            if (difficultyText != null)
            {
                DifficultyManager.Difficulty difficulty = DifficultyManager.GetCurrentDifficulty();
                difficultyText.text = $"Difficulty: {difficulty}";
            }
            
            // Current score
            if (currentScoreText != null)
            {
                currentScoreText.text = $"Score: {ScoreManager.Instance.GetCurrentScore()}";
            }
            
            // High score
            if (highScoreText != null)
            {
                int highScore = ScoreManager.Instance.GetHighScore();
                highScoreText.text = $"High Score: {highScore}";
            }
            
            // Statistics
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
        
        // Pause game
        Time.timeScale = 0f;
        
        // Disable player input
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
        
        // Ensure UI can still receive input even when timeScale = 0
        // Unity UI should work with timeScale = 0, but we'll ensure EventSystem is active
        UnityEngine.EventSystems.EventSystem eventSystem = UnityEngine.EventSystems.EventSystem.current;
        if (eventSystem == null)
        {
            // Try to find EventSystem in scene
            eventSystem = FindObjectOfType<UnityEngine.EventSystems.EventSystem>();
        }
        
        if (eventSystem == null)
        {
            // Create EventSystem if it doesn't exist
            GameObject eventSystemObj = new GameObject("EventSystem");
            eventSystem = eventSystemObj.AddComponent<UnityEngine.EventSystems.EventSystem>();
            eventSystemObj.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
            Debug.Log("EndLevelUI: Created EventSystem");
        }
        
        if (eventSystem != null)
        {
            eventSystem.enabled = true;
            Debug.Log("EndLevelUI: EventSystem is active");
        }
        else
        {
            Debug.LogError("EndLevelUI: Failed to create EventSystem! UI buttons will not work.");
        }
        
        // Ensure Canvas has GraphicRaycaster
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
                Debug.Log("EndLevelUI: Added GraphicRaycaster to Canvas");
            }
            
            // Ensure Canvas is set to Screen Space - Overlay or Screen Space - Camera
            if (canvas.renderMode == RenderMode.WorldSpace)
            {
                Debug.LogWarning("EndLevelUI: Canvas is in World Space mode. UI buttons may not work properly. Consider using Screen Space - Overlay.");
            }
        }
        else
        {
            Debug.LogError("EndLevelUI: Canvas not found! UI buttons will not work.");
        }
        
        // Double-check buttons are assigned and setup
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
        
        // Switch to menu music (wait.mp3) when End Level Panel shows
        if (BackgroundMusicManager.Instance != null)
        {
            BackgroundMusicManager.Instance.PlayMenuMusic();
            Debug.Log("EndLevelUI: Switched to menu music (wait.mp3)");
        }
        else
        {
            Debug.LogWarning("EndLevelUI: BackgroundMusicManager.Instance is null! Cannot switch music.");
        }
        
        Debug.Log("EndLevelUI: Game paused, input disabled");
    }
    
    /// <summary>
    /// Restart game from the beginning (Scene1)
    /// </summary>
    private void OnRestart()
    {
        Debug.Log("EndLevelUI: Restart button clicked!");
        StartCoroutine(FadeAndRestart());
    }
    
    /// <summary>
    /// Fade to black then restart game
    /// </summary>
    private IEnumerator FadeAndRestart()
    {
        // Keep input disabled during fade
        // Don't re-enable input yet!
        
        Time.timeScale = 1f;
        
        // Fade to black
        if (UIFade.Instance != null)
        {
            UIFade.Instance.FadeToBlack();
            Debug.Log("EndLevelUI: Fading to black...");
            yield return new WaitForSeconds(1f);
        }
        
        // Hide panel after fade
        if (endLevelPanel != null)
        {
            endLevelPanel.SetActive(false);
        }
        
        // Re-enable input just before loading (scene will handle it)
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
        
        // Destroy all DontDestroyOnLoad objects
        DestroyPersistentObjects();
        
        // Load first scene
        SceneManager.LoadScene(firstSceneName);
    }
    
    /// <summary>
    /// Destroy all persistent (DontDestroyOnLoad) objects for clean restart
    /// </summary>
    private void DestroyPersistentObjects()
    {
        // Find all root GameObjects in DontDestroyOnLoad scene
        GameObject[] rootObjects = gameObject.scene.GetRootGameObjects();
        
        // Also get objects from DontDestroyOnLoad
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
    
    /// <summary>
    /// Return to main menu
    /// </summary>
    private void OnMenu()
    {
        Debug.Log("EndLevelUI: Menu button clicked!");
        StartCoroutine(FadeAndLoadMenu());
    }
    
    /// <summary>
    /// Fade to black then load main menu
    /// </summary>
    private IEnumerator FadeAndLoadMenu()
    {
        // Keep input disabled during fade
        // Don't re-enable input yet!
        
        Time.timeScale = 1f;
        
        // Fade to black
        if (UIFade.Instance != null)
        {
            UIFade.Instance.FadeToBlack();
            Debug.Log("EndLevelUI: Fading to black...");
            yield return new WaitForSeconds(1f);
        }
        
        // Hide panel after fade
        if (endLevelPanel != null)
        {
            endLevelPanel.SetActive(false);
        }
        
        // Re-enable input just before loading (scene will handle it)
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
        
        // Destroy all DontDestroyOnLoad objects
        DestroyPersistentObjects();
        
        // Load main menu
        SceneManager.LoadScene(menuSceneName);
    }
}

