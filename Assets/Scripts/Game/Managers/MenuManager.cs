using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [Header("UI References")]
    public Image fadePanel;        // Black panel that covers the screen
    public float fadeDuration = 1f; // How long the fade takes

    public GameObject menuPanel;        // Your main menu panel
    public GameObject settingPanel;     //my settings duh
    public GameObject splashPanel;      // Your splash screen panel
    public GameObject tutorialPanel;    // Tutorial panel with page 1 and page 2
    public float splashDuration = 3f;   // How long splash screen shows before tutorial

    [Header("Sounds UI")]
    public AudioSource buttonSound;

    void Start()
    {
        SetFadeAlpha(0f);
        tutorialPanel.SetActive(false);
    }

//self explanatory
    public void OnPlayClicked()
    {
        StartCoroutine(FadeAndLoad());
        buttonSound.Play();
    }
    //the settings once I click it
    public void OnSettingsClicked()
    {
        menuPanel.SetActive(false);
        settingPanel.SetActive(true);
        buttonSound.Play();

    }
    //the return button function once it is clicked
    public void onReturnClicked()
    {
        settingPanel.SetActive(false);
        menuPanel.SetActive(true);
        buttonSound.Play();
    }
    //The Exit button when it is clicked
    public void OnExitClicked()
    {
        Application.Quit();
        buttonSound.Play();
    }


    // Fades screen to black then loads the splash screen image
    IEnumerator FadeAndLoad()
    {
        buttonSound.Play();
        // Fade to black
        yield return StartCoroutine(Fade(0f, 1f));
        menuPanel.SetActive(false);
        splashPanel.SetActive(true);

        // Fade back in
        yield return StartCoroutine(Fade(1f, 0f));

        // Wait on splash screen
        yield return new WaitForSeconds(splashDuration);

        // Fade to black again
        yield return StartCoroutine(Fade(0f, 1f));

        // Switch to tutorial
        splashPanel.SetActive(false);
        tutorialPanel.SetActive(true);

        // Fade back in
        yield return StartCoroutine(Fade(1f, 0f));
    }

    //starting the fade to my splash screen
    IEnumerator Fade(float startAlpha, float endAlpha)
    {
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, endAlpha, elapsed / fadeDuration);
            SetFadeAlpha(alpha);
            yield return null;
        }

        SetFadeAlpha(endAlpha);
    }

    // SET FADE ALPHA
    // Helper to set the fade panel transparency
    void SetFadeAlpha(float alpha)
    {
        Color color = fadePanel.color;
        color.a = alpha;
        fadePanel.color = color;
    }
}