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
        // Đợi một chút trước khi fade in
        yield return new WaitForSeconds(fadeInDelay);
        
        // Fade to clear khi scene load xong
        if (UIFade.Instance != null)
        {
            UIFade.Instance.FadeToClear();
        }
    }
}

