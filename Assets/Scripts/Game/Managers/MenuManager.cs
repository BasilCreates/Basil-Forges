using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject mainCanvas;

    public Image fadePanel;        // For the splash screen fade
    public Image endFadePanel;     // For the fade at the end of the cutscene
    public float fadeDuration = 1f;

    public GameObject menuPanel;
    public GameObject settingPanel;
    public GameObject splashPanel;
    public GameObject tutorialPanel;
    public float splashDuration = 3f;

    [Header("Cutscene Parameters")]
    public GameObject defaultCamera;
    public List<GameObject> cutsceneCameras;
    public List<float> cameraDurations;

    [Header("Animation")]
    public Animator cutsceneAnimator;

    [Header("Dialogue Parameters")]
    public GameObject dialogueCanvas;         // The new canvas specifically for dialogue
    public Text dialogueText;                 // The text component that displays the words
    public List<string> dialogueLines;        // What the characters say
    public List<float> dialogueDurations;     // How long each line stays on screen
    public List<Color> dialogueColors;        // The color for each line of dialogue

    [Header("Sounds UI")]
    public AudioSource buttonSound;

    void Start()
    {
        SetFadeAlpha(fadePanel, 0f);
        if (endFadePanel != null) SetFadeAlpha(endFadePanel, 0f);

        tutorialPanel.SetActive(false);

        if (dialogueCanvas != null) dialogueCanvas.SetActive(false); // Make sure it starts off

        if (cutsceneAnimator != null)
        {
            cutsceneAnimator.enabled = false;
        }
    }

    public void OnPlayClicked()
    {
        StartCoroutine(FadeAndLoad());
        buttonSound.Play();
    }

    public void OnSettingsClicked()
    {
        menuPanel.SetActive(false);
        settingPanel.SetActive(true);
        buttonSound.Play();
    }

    public void onReturnClicked()
    {
        settingPanel.SetActive(false);
        menuPanel.SetActive(true);
        buttonSound.Play();
    }

    public void OnExitClicked()
    {
        Application.Quit();
        buttonSound.Play();
    }

    IEnumerator FadeAndLoad()
    {
        buttonSound.Play();

        // 1. Fade to black and show splash
        yield return StartCoroutine(Fade(fadePanel, 0f, 1f));
        menuPanel.SetActive(false);
        splashPanel.SetActive(true);

        // 2. Fade back in to Splash Screen
        yield return StartCoroutine(Fade(fadePanel, 1f, 0f));

        // 3. Wait on splash screen
        yield return new WaitForSeconds(splashDuration);

        // --- CUTSCENE TESTING PHASE ---

        // Turn off the ENTIRE main canvas and default camera instantly
        if (mainCanvas != null) mainCanvas.SetActive(false);
        if (defaultCamera != null) defaultCamera.SetActive(false);

        // Turn ON the dialogue canvas and start the text sequence in parallel!
        if (dialogueCanvas != null)
        {
            dialogueCanvas.SetActive(true);
            StartCoroutine(PlayDialogueSequence());
        }

        // START THE ANIMATION!
        if (cutsceneAnimator != null)
        {
            cutsceneAnimator.enabled = true;
        }

        // Ensure all cutscene cameras are off initially
        foreach (GameObject cam in cutsceneCameras)
        {
            if (cam != null) cam.SetActive(false);
        }

        // Loop through Cutscene Cameras
        for (int i = 0; i < cutsceneCameras.Count; i++)
        {
            if (cutsceneCameras[i] != null)
            {
                // Turn on the current camera in the sequence
                cutsceneCameras[i].SetActive(true);

                float waitTime = (i < cameraDurations.Count) ? cameraDurations[i] : 2f;

                // Check if this is the VERY LAST camera in the list
                bool isLastCamera = (i == cutsceneCameras.Count - 1);

                if (isLastCamera && endFadePanel != null)
                {
                    Debug.Log("[Cutscene] Reached last camera. Preparing end fade...");

                    // If it's the last camera, we want to fade out *while* this camera is playing.
                    float fadeStartTime = Mathf.Max(0f, waitTime - fadeDuration);

                    yield return new WaitForSeconds(fadeStartTime);

                    Debug.Log("[Cutscene] Starting end fade OUT to black...");
                    // Fade out to black *during* the last camera's remaining time
                    yield return StartCoroutine(Fade(endFadePanel, 0f, 1f));
                    Debug.Log("[Cutscene] End fade OUT complete.");

                    // Any leftover time for the last camera if fade was faster than waitTime
                    float remainingTime = waitTime - fadeStartTime - fadeDuration;
                    if (remainingTime > 0f)
                    {
                        yield return new WaitForSeconds(remainingTime);
                    }
                }
                else
                {
                    // Normal camera behavior for earlier cameras
                    yield return new WaitForSeconds(waitTime);
                }

                // Turn it off so the next one takes over (or it ends)
                cutsceneCameras[i].SetActive(false);
            }
        }

        // --- END CUTSCENE ---

        // Turn off the dialogue canvas when the cutscene ends
        if (dialogueCanvas != null) dialogueCanvas.SetActive(false);

        // Bring back the default camera and the entire Main Canvas (hidden under the black endFadePanel)
        if (defaultCamera != null) defaultCamera.SetActive(true);
        if (mainCanvas != null) mainCanvas.SetActive(true);

        // Ensure the splash panel is gone and tutorial panel is ready
        splashPanel.SetActive(false);
        tutorialPanel.SetActive(true);

        // FADE BACK IN USING THE NEW END FADE PANEL TO REVEAL TUTORIAL
        if (endFadePanel != null)
        {
            Debug.Log("[Cutscene] Starting end fade IN to reveal tutorial...");
            yield return StartCoroutine(Fade(endFadePanel, 1f, 0f));
            Debug.Log("[Cutscene] End fade IN complete.");
        }
        else
        {
            Debug.LogWarning("[Cutscene] Warning: endFadePanel is null, skipping fade-in!");
        }
    }

    // PLAYS DIALOGUE AND FORCES FULL ALPHA SO IT'S VISIBLE
    IEnumerator PlayDialogueSequence()
    {
        if (dialogueText == null) yield break; // Safety check

        for (int i = 0; i < dialogueLines.Count; i++)
        {
            // Set the current line of text
            dialogueText.text = dialogueLines[i];

            // SET THE COLOR & FORCE FULL OPACITY (ALPHA = 1f)
            if (i < dialogueColors.Count)
            {
                Color chosenColor = dialogueColors[i];
                chosenColor.a = 1f;
                dialogueText.color = chosenColor;
            }
            else
            {
                dialogueText.color = Color.white;
            }

            // Wait for this specific line's duration (with a safe 2-second fallback)
            float waitTime = (i < dialogueDurations.Count) ? dialogueDurations[i] : 2f;
            yield return new WaitForSeconds(waitTime);
        }

        // Clear the text box after all lines are done (in case the cameras are still rolling)
        dialogueText.text = "";
    }

    IEnumerator Fade(Image panel, float startAlpha, float endAlpha)
    {
        if (panel == null) yield break;

        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, endAlpha, elapsed / fadeDuration);
            SetFadeAlpha(panel, alpha);
            yield return null;
        }

        SetFadeAlpha(panel, endAlpha);
    }

    void SetFadeAlpha(Image panel, float alpha)
    {
        if (panel != null)
        {
            Color color = panel.color;
            color.a = alpha;
            panel.color = color;
        }
    }
}