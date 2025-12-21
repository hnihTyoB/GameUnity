using System.Collections;
using UnityEngine;

public class SceneFadeIn : MonoBehaviour
{
    [SerializeField] private float fadeInDelay = 0.2f;

    private void Start()
    {
        StartCoroutine(FadeInRoutine());
    }

    private IEnumerator FadeInRoutine()
    {
  
        yield return new WaitForSeconds(fadeInDelay);
        
  
        if (UIFade.Instance != null)
        {
            UIFade.Instance.FadeToClear();
        }
    }
}

