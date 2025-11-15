using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// Manages game pause functionality
/// Handles pause panel display and input
/// </summary>
public class PauseManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private Button pauseButton;
    [SerializeField] private Button backButton; // Resume button (renamed to Back)
    [SerializeField] private Button restartButton;
    [SerializeField] private Button menuButton;
    
    [Header("Scene Settings")]
    [SerializeField] private string firstSceneName = "Scene1";
    [SerializeField] private string menuSceneName = "MainMenu";
    
    private bool isPaused = false;
    
    private void Start()
    {
        // Hide pause panel initially
        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
        }
        
        // Ensure EventSystem exists
        EnsureEventSystem();
        
        // Setup button listeners
        if (pauseButton != null)
        {
            pauseButton.onClick.AddListener(PauseGame);
            Debug.Log($"PauseManager: Pause button setup - interactable={pauseButton.interactable}");
        }
        else
        {
            Debug.LogError("PauseManager: Pause button is NULL!");
        }
        
        if (backButton != null)
        {
            backButton.onClick.AddListener(ResumeGame);
        }
        
        if (restartButton != null)
        {
            restartButton.onClick.AddListener(OnRestart);
        }
        
        if (menuButton != null)
        {
            menuButton.onClick.AddListener(OnMenu);
        }
    }
    
    /// <summary>
    /// Ensure EventSystem exists for UI interaction
    /// </summary>
    private void EnsureEventSystem()
    {
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
            Debug.Log("PauseManager: Created EventSystem");
        }
        else
        {
            Debug.Log("PauseManager: EventSystem already exists");
        }
        
        // Ensure Canvas has GraphicRaycaster
        Canvas canvas = GetComponentInParent<Canvas>();
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
                Debug.Log("PauseManager: Added GraphicRaycaster to Canvas");
            }
            else
            {
                Debug.Log("PauseManager: GraphicRaycaster already exists on Canvas");
            }
        }
        else
        {
            Debug.LogError("PauseManager: Canvas not found!");
        }
    }
    
    /// <summary>
    /// Restart game from beginning
    /// </summary>
    private void OnRestart()
    {
        Debug.Log("PauseManager: Restart button clicked!");
        StartCoroutine(FadeAndRestart());
    }
    
    /// <summary>
    /// Fade to black then restart game
    /// </summary>
    private IEnumerator FadeAndRestart()
    {
        // Keep input disabled during fade
        Time.timeScale = 1f;
        
        // Fade to black
        if (UIFade.Instance != null)
        {
            UIFade.Instance.FadeToBlack();
            Debug.Log("PauseManager: Fading to black...");
            yield return new WaitForSeconds(1f);
        }
        
        // Hide pause panel
        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
        }
        
        isPaused = false;
        
        // Re-enable input
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
    /// Go to main menu
    /// </summary>
    private void OnMenu()
    {
        Debug.Log("PauseManager: Menu button clicked!");
        StartCoroutine(FadeAndLoadMenu());
    }
    
    /// <summary>
    /// Fade to black then load main menu
    /// </summary>
    private IEnumerator FadeAndLoadMenu()
    {
        // Keep input disabled during fade
        Time.timeScale = 1f;
        
        // Fade to black
        if (UIFade.Instance != null)
        {
            UIFade.Instance.FadeToBlack();
            Debug.Log("PauseManager: Fading to black...");
            yield return new WaitForSeconds(1f);
        }
        
        // Hide pause panel
        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
        }
        
        isPaused = false;
        
        // Re-enable input
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
    
    /// <summary>
    /// Destroy all persistent (DontDestroyOnLoad) objects for clean restart
    /// </summary>
    private void DestroyPersistentObjects()
    {
        // Find all root GameObjects in DontDestroyOnLoad scene
        GameObject temp = new GameObject("Temp");
        DontDestroyOnLoad(temp);
        Scene dontDestroyScene = temp.scene;
        Destroy(temp);
        
        GameObject[] dontDestroyObjects = dontDestroyScene.GetRootGameObjects();
        
        Debug.Log($"PauseManager: Found {dontDestroyObjects.Length} DontDestroyOnLoad objects to destroy");
        
        foreach (GameObject obj in dontDestroyObjects)
        {
            Debug.Log($"PauseManager: Destroying {obj.name}");
            Destroy(obj);
        }
    }
    
    private void Update()
    {
        // Check for ESC key
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }
    
    /// <summary>
    /// Pause the game
    /// </summary>
    public void PauseGame()
    {
        Debug.Log("PauseManager: PauseGame() called!");
        
        if (isPaused) return;
        
        isPaused = true;
        
        // Show pause panel
        if (pausePanel != null)
        {
            pausePanel.SetActive(true);
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
        
        // Switch to menu music (wait.mp3) when paused
        if (BackgroundMusicManager.Instance != null)
        {
            BackgroundMusicManager.Instance.PlayMenuMusic();
            Debug.Log("PauseManager: Switched to menu music (wait.mp3)");
        }
        
        Debug.Log("PauseManager: Game paused");
    }
    
    /// <summary>
    /// Resume the game
    /// </summary>
    public void ResumeGame()
    {
        if (!isPaused) return;
        
        isPaused = false;
        
        // Hide pause panel
        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
        }
        
        // Resume game
        Time.timeScale = 1f;
        
        // Enable player input
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
        
        // Switch back to game music when resumed
        if (BackgroundMusicManager.Instance != null)
        {
            BackgroundMusicManager.Instance.PlayGameMusic();
            Debug.Log("PauseManager: Switched back to game music (music.mp3)");
        }
        
        Debug.Log("PauseManager: Game resumed");
    }
    
    /// <summary>
    /// Check if game is currently paused
    /// </summary>
    public bool IsPaused()
    {
        return isPaused;
    }
}
