using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [Header("UI Panels")]
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject tutorialPanel;

    [Header("Buttons")]
    [SerializeField] private Button startButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button tutorialButton;
    [SerializeField] private Button exitButton;
    [SerializeField] private Button settingsBackButton;
    [SerializeField] private Button tutorialBackButton;

    [Header("Scene Settings")]
    [SerializeField] private string firstSceneName = "Scene1";

    private void Start()
    {
        // Đặt panel mặc định
        ShowMainMenu();

        // Gán sự kiện cho các nút
        if (startButton != null)
            startButton.onClick.AddListener(OnStartGame);
        
        if (settingsButton != null)
            settingsButton.onClick.AddListener(OnSettings);
        
        if (tutorialButton != null)
            tutorialButton.onClick.AddListener(OnTutorial);
        
        if (exitButton != null)
            exitButton.onClick.AddListener(OnExit);

        if (settingsBackButton != null)
            settingsBackButton.onClick.AddListener(ShowMainMenu);

        if (tutorialBackButton != null)
            tutorialBackButton.onClick.AddListener(ShowMainMenu);
    }

    private void ShowMainMenu()
    {
        if (mainMenuPanel != null)
            mainMenuPanel.SetActive(true);
        
        if (settingsPanel != null)
            settingsPanel.SetActive(false);
        
        if (tutorialPanel != null)
            tutorialPanel.SetActive(false);
    }

    private void OnStartGame()
    {
        Debug.Log("Bắt đầu game...");
        StartCoroutine(LoadGameScene());
    }

    private IEnumerator LoadGameScene()
    {
        // Fade to black trước khi chuyển scene
        if (UIFade.Instance != null)
        {
            UIFade.Instance.FadeToBlack();
        }
        
        // Đợi fade animation hoàn thành
        yield return new WaitForSeconds(1f);
        
        SceneManager.LoadScene(firstSceneName);
    }

    private void OnSettings()
    {
        Debug.Log("Mở Settings...");
        if (mainMenuPanel != null)
            mainMenuPanel.SetActive(false);
        
        if (settingsPanel != null)
            settingsPanel.SetActive(true);
    }

    private void OnTutorial()
    {
        Debug.Log("Mở Tutorial...");
        if (mainMenuPanel != null)
            mainMenuPanel.SetActive(false);
        
        if (tutorialPanel != null)
            tutorialPanel.SetActive(true);
    }

    private void OnExit()
    {
        Debug.Log("Thoát game...");
        
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}

