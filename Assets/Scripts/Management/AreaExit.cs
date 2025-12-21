using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AreaExit : MonoBehaviour
{
    [SerializeField] private string sceneToLoad;
    [SerializeField] private string sceneTransitionName;
    [SerializeField] private string endExitTag = "AreaEnd";
    [SerializeField] private BoxCollider2D blockingCollider;

    private float waitToLoadTime = 1f;
    private bool hasTriggered = false; 

    private void Start()
    {
   
        if (blockingCollider != null)
        {
            blockingCollider.enabled = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasTriggered) return;
        if (other.gameObject.GetComponent<PlayerController>() == null) return;

      
        if (SceneManager.GetActiveScene().name == "Scene2" && sceneToLoad == "Scene1")
        {
        
            if (blockingCollider != null)
            {
                blockingCollider.enabled = true;
            }
            Debug.Log("AreaExit: Đã chặn người chơi quay lại scene1 từ scene2.");
            return;
        }

        hasTriggered = true;

    
        if (gameObject.CompareTag(endExitTag))
        {
            if (ScoreManager.Instance != null)
            {
                ScoreManager.Instance.OnLevelComplete();
               
            }
            else
            {
                Debug.LogError("AreaExit: ScoreManager.Instance is null!");
            }
            return;
        }

      
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
