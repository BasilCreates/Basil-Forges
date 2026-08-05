using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TargetSelectionManager : MonoBehaviour
{
    [Header("3D Character Models")]
    public List<GameObject> characterModels; // Drag your 3D characters in here

    [Header("UI")]
    public Text targetNameText; // Text field that displays "Current Target: [Name]"

    [Header("Panels")]
    public GameObject targetSelectionPanel;

    [Header("Fade UI")]
    public RawImage fadeImage; // NEW: Raw image field for screen fading
    public float fadeDuration = 1.5f; // Duration of the fade effect

    [Header("Sounds UI")]
    public AudioSource buttonSound1;
    public AudioSource buttonSound2;

    public List<string> availableTargets = new List<string>();
    public string currentHoveredTag = "";
    private bool hasSelected = false;

    // Called after card is selected, builds list of valid targets
    public void OpenTargetSelection()
    {
        Debug.Log("Opening target selection!");
        hasSelected = false;
        availableTargets.Clear();

        foreach (GameObject model in characterModels)
        {
            if (model != null)
            {
                string tag = model.tag;

                if (!GameManager.Instance.IsEliminated(tag) &&
                    tag != GameManager.Instance.chosenCharacter)
                {
                    availableTargets.Add(tag);
                }
            }
        }

        currentHoveredTag = "";

        if (targetSelectionPanel != null)
        {
            targetSelectionPanel.SetActive(true);
        }

        // --- UPDATED HERE: Tell LevelManager to update the chosen character text using its existing logic! ---
        LevelManager levelManager = FindObjectOfType<LevelManager>();
        if (levelManager != null)
        {
            levelManager.UpdateChosenCharacterText();
        }

        // Reset the target text display when opening
        if (targetNameText != null)
        {
            targetNameText.text = "Current Target: None";
        }

        // Turn back on valid 3D character models and set only the player's chosen character to inactive
        foreach (GameObject model in characterModels)
        {
            if (model != null)
            {
                string tag = model.tag;

                if (tag == GameManager.Instance.chosenCharacter)
                {
                    // Ensure the player's chosen character stays hidden
                    model.SetActive(false);
                }
                else if (availableTargets.Contains(tag))
                {
                    // Turn on valid target models
                    model.SetActive(true);
                }
                else
                {
                    // Optional: ensure eliminated or invalid ones stay hidden too
                    model.SetActive(false);
                }
            }
        }
    }

    // Called by Raycast script when hovered
    public void HoverTarget(string characterTag)
    {
        if (hasSelected) return;

        if (!availableTargets.Contains(characterTag))
        {
            Debug.LogWarning("Target tag '" + characterTag + "' is NOT in availableTargets list! Check character tags.");
            return;
        }

        if (currentHoveredTag == characterTag) return;

        currentHoveredTag = characterTag;
        Debug.Log("Hovering target via raycast: " + characterTag);

        if (buttonSound2 != null)
        {
            buttonSound2.Play();
        }

        // Update the text field with the current hovered target name
        if (targetNameText != null)
        {
            targetNameText.text = "Current Target: " + characterTag;
        }
    }

    // Called by Raycast script when we look away
    public void ClearHover()
    {
        if (hasSelected) return;
        if (string.IsNullOrEmpty(currentHoveredTag)) return;

        currentHoveredTag = "";

        // Clear the text field when looking away
        if (targetNameText != null)
        {
            targetNameText.text = "Current Target: None";
        }
    }

    // Confirm target and apply card
    public void OnConfirmTarget(string characterTag)
    {
        if (hasSelected) return;

        if (!availableTargets.Contains(characterTag)) return;

        hasSelected = true;

        if (buttonSound1 != null)
        {
            buttonSound1.Play();
        }

        GameManager.Instance.targetedCharacter = characterTag;
        Debug.Log("Choose target: " + characterTag);

        // Update text to show final choice confirmation if desired
        if (targetNameText != null)
        {
            targetNameText.text = "Current Target: " + characterTag + " (Selected)";
        }

        // Uncommented and hooked up with the fade-in routine and card application manager
        StartCoroutine(ConfirmCardRoutine());
    }

    // NEW: Coroutine to handle the fade effect before confirming the card application
    private IEnumerator ConfirmCardRoutine()
    {
        // Fade in from transparent to black if fadeImage is assigned
        if (fadeImage != null)
        {
            fadeImage.gameObject.SetActive(true);
            Color color = fadeImage.color;
            color.a = 0f;
            fadeImage.color = color;

            float elapsedTime = 0f;
            while (elapsedTime < fadeDuration)
            {
                elapsedTime += Time.deltaTime;
                color.a = Mathf.Clamp01(elapsedTime / fadeDuration);
                fadeImage.color = color;
                yield return null;
            }
        }

        // Call the card application manager to proceed
        CardApplicationManager cardManager = FindObjectOfType<CardApplicationManager>();
        if (cardManager != null)
        {
            cardManager.ConfirmCard();
        }
    }
}