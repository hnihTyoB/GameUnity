using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AreaExit : MonoBehaviour
{
    [SerializeField] private string sceneToLoad;
    [SerializeField] private string sceneTransitionName;
    [SerializeField] private string endExitTag = "AreaEnd";
    [SerializeField] private BoxCollider2D blockingCollider; // Tham chiếu đến BoxCollider2D để chặn người chơi

    private float waitToLoadTime = 1f;
    private bool hasTriggered = false; // Prevent multiple triggers

    private void Start()
    {
        // Đảm bảo blockingCollider bị tắt khi bắt đầu
        if (blockingCollider != null)
        {
            blockingCollider.enabled = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasTriggered) return;
        if (other.gameObject.GetComponent<PlayerController>() == null) return;

        // Ngăn người chơi quay lại scene1 từ scene2
        if (SceneManager.GetActiveScene().name == "Scene2" && sceneToLoad == "Scene1")
        {
            // Kích hoạt collider để chặn người chơi và không chuyển cảnh
            if (blockingCollider != null)
            {
                blockingCollider.enabled = true;
            }
            Debug.Log("AreaExit: Đã chặn người chơi quay lại scene1 từ scene2.");
            return; // Dừng thực thi để không chuyển cảnh
        }

        hasTriggered = true;

        // If this door is tagged as the end-game exit, trigger level complete
        if (gameObject.CompareTag(endExitTag))
        {
            if (ScoreManager.Instance != null)
            {
                ScoreManager.Instance.OnLevelComplete();
                Debug.Log("AreaExit: End-game door triggered - OnLevelComplete called");
            }
            else
            {
                Debug.LogError("AreaExit: ScoreManager.Instance is null!");
            }
            return;
        }

        // Normal scene transition (non end-game door)
        if (SceneManagement.Instance != null)
        {
            SceneManagement.Instance.SetTransitionName(sceneTransitionName);
        }
        else
        {
            Debug.LogWarning("AreaExit: SceneManagement.Instance is null.");
        }

        if (UIFade.Instance != null)
        {
            UIFade.Instance.FadeToBlack();
        }
        else
        {
            Debug.LogWarning("AreaExit: UIFade.Instance is null.");
        }

        StartCoroutine(LoadSceneRoutine());
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // Tắt blockingCollider khi người chơi rời khỏi khu vực trigger
        // để họ có thể đi vào lại nếu cần (ví dụ: nếu đó không phải là cửa bị chặn)
        if (blockingCollider != null && blockingCollider.enabled)
        {
            blockingCollider.enabled = false;
        }
    }
    
    private IEnumerator LoadSceneRoutine()
    {
        while (waitToLoadTime >= 0) 
        {
            waitToLoadTime -= Time.deltaTime;
            yield return null;
        }

        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneToLoad);
    }
    
}
