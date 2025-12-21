using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private Button pauseButton;
    [SerializeField] private Button backButton; 
    [SerializeField] private Button restartButton;
    [SerializeField] private Button menuButton;
    
    [Header("Scene Settings")]
    [SerializeField] private string firstSceneName = "Scene1";
    [SerializeField] private string menuSceneName = "MainMenu";
    
    private bool isPaused = false;
    
    private void Start()
    {
        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
        }
        
        EnsureEventSystem();
    
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
    
    private void EnsureEventSystem()
    {
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
            Debug.Log("PauseManager: Created EventSystem");
        }
        else
        {
            Debug.Log("PauseManager: EventSystem already exists");
        }
        
      
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
    
   
    private void OnRestart()
    {
        Debug.Log("PauseManager: Restart button clicked!");
        StartCoroutine(FadeAndRestart());
    }
    
 
    private IEnumerator FadeAndRestart()
    {
       
        Time.timeScale = 1f;
        
       
        if (UIFade.Instance != null)
        {
            UIFade.Instance.FadeToBlack();
            Debug.Log("PauseManager: Fading to black...");
            yield return new WaitForSeconds(1f);
        }
        
       
        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
        }
        
        isPaused = false;
        
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
    
  
    private void OnMenu()
    {
        Debug.Log("PauseManager: Menu button clicked!");
        StartCoroutine(FadeAndLoadMenu());
    }
    
      private IEnumerator FadeAndLoadMenu()
    {
       
        Time.timeScale = 1f;
        
        
        if (UIFade.Instance != null)
        {
            UIFade.Instance.FadeToBlack();
            Debug.Log("PauseManager: Fading to black...");
            yield return new WaitForSeconds(1f);
        }
        
     
        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
        }
        
        isPaused = false;
        
       
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
    
  
    private void DestroyPersistentObjects()
    {
       
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
    
    
    public void PauseGame()
    {
        Debug.Log("PauseManager: PauseGame() called!");
        
        if (isPaused) return;
        
        isPaused = true;
        
   
        if (pausePanel != null)
        {
            pausePanel.SetActive(true);
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
        
        if (BackgroundMusicManager.Instance != null)
        {
            BackgroundMusicManager.Instance.PlayMenuMusic();
            Debug.Log("PauseManager: Switched to menu music (wait.mp3)");
        }
        
        Debug.Log("PauseManager: Game paused");
    }
    
    public void ResumeGame()
    {
        if (!isPaused) return;
        
        isPaused = false;
  
        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
        }

        Time.timeScale = 1f;
        
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
        
  
        if (BackgroundMusicManager.Instance != null)
        {
            BackgroundMusicManager.Instance.PlayGameMusic();
            Debug.Log("PauseManager: Switched back to game music (music.mp3)");
        }
        
        Debug.Log("PauseManager: Game resumed");
    }
    
    public bool IsPaused()
    {
        return isPaused;
    }
}
