using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class VoteBotManager : MonoBehaviour
{
    [Header("Fade")]
    public RawImage fadePanel;
    public float fadeDuration = 0.5f;

    [Header("Scene Settings")]
    public string montageScene = "Contestant Stage";

    // Fade in at the start
    void Start()
    {
        StartCoroutine(FadeIn());
    }

    // Fades in when scene loads
    IEnumerator FadeIn()
    {
        yield return StartCoroutine(Fade(1f, 0f));
    }

    // Called when player reaches the laptop trigger
    public void OnLaptopReached()
    {
        StartCoroutine(FadeAndLoadMontage());
    }

    // Fades out then loads montage
    IEnumerator FadeAndLoadMontage()
    {
        yield return StartCoroutine(Fade(0f, 1f));
        SceneManager.LoadScene(montageScene);
    }

    // Fades panel from one alpha to another
    IEnumerator Fade(float startAlpha, float endAlpha)
    {
        float elapsed = 0f;
        Color color = fadePanel.color;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            color.a = Mathf.Lerp(startAlpha, endAlpha, elapsed / fadeDuration);
            fadePanel.color = color;
            yield return null;
        }

        color.a = endAlpha;
        fadePanel.color = color;
    }
}