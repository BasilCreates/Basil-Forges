using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class EliminationMontageManager : MonoBehaviour
{
    [Header("Spawn & Character")]
    public Transform characterSpawnSpot;
    public GameObject[] characterObjects;

    [Header("UI Text")]
    public Text eliminatedCharacterNameText;

    [Header("Cameras & Timing")]
    public List<Camera> montageCameras;
    public float defaultCameraDuration = 3f;
    public AudioSource buttonSound;

    [Header("Fade UI")]
    public Image fadePanelImage;
    public float fadeDuration = 1.5f;

    [Header("Loss Indicator")]
    public Image redLossIndicatorImage; // Assign your opaque red image here

    [Header("Canvases & Panels")]
    public GameObject gameUICanvas; // Assign your first canvas here
    public GameObject youLostPanel;

    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (fadePanelImage != null)
        {
            Color c = fadePanelImage.color;
            c.a = 0f;
            fadePanelImage.color = c;
            fadePanelImage.gameObject.SetActive(true);
        }

        if (redLossIndicatorImage != null)
        {
            redLossIndicatorImage.gameObject.SetActive(false);
        }

        if (gameUICanvas != null)
        {
            gameUICanvas.SetActive(true);
        }

        if (youLostPanel != null)
        {
            youLostPanel.SetActive(false);
        }

        SpawnEliminatedCharacter();
        StartCoroutine(RunEliminationSequence());
    }

    void SpawnEliminatedCharacter()
    {
        // Grab the character name directly from targetedCharacter, falling back to the list if needed
        string eliminatedName = "";

        if (GameManager.Instance != null)
        {
            if (!string.IsNullOrEmpty(GameManager.Instance.targetedCharacter))
            {
                eliminatedName = GameManager.Instance.targetedCharacter.Trim();
            }
            else if (GameManager.Instance.eliminatedCharacters.Count > 0)
            {
                eliminatedName = GameManager.Instance.eliminatedCharacters[GameManager.Instance.eliminatedCharacters.Count - 1].Trim();
            }
        }

        Debug.Log("Looking for eliminated character tag: '" + eliminatedName + "'");

        if (eliminatedCharacterNameText != null)
        {
            eliminatedCharacterNameText.text = eliminatedName;
        }

        if (characterSpawnSpot == null || characterObjects.Length == 0) return;

        // Loop through the array and activate the character matching the tag or name
        foreach (GameObject obj in characterObjects)
        {
            if (obj != null)
            {
                bool tagMatches = false;
                try
                {
                    tagMatches = obj.CompareTag(eliminatedName);
                }
                catch
                {
                    // Catch if tag isn't registered in tag manager
                }

                bool nameMatches = obj.name.Equals(eliminatedName, System.StringComparison.OrdinalIgnoreCase);

                if (tagMatches || nameMatches)
                {
                    obj.transform.position = characterSpawnSpot.position;
                    obj.transform.rotation = characterSpawnSpot.rotation;
                    obj.SetActive(true);
                    Debug.Log("Activated eliminated character: " + obj.name);
                    return;
                }
            }
        }

        Debug.LogWarning("Couldn't find an object with tag or name matching '" + eliminatedName + "'!");
    }

    IEnumerator RunEliminationSequence()
    {
        if (montageCameras == null || montageCameras.Count == 0)
        {
            Debug.LogWarning("Yo, you forgot to assign cameras to the montage list!");
            yield break;
        }

        for (int i = 0; i < montageCameras.Count; i++)
        {
            foreach (Camera cam in montageCameras)
            {
                if (cam != null) cam.gameObject.SetActive(false);
            }

            if (montageCameras[i] != null)
            {
                montageCameras[i].gameObject.SetActive(true);
            }

            yield return new WaitForSeconds(defaultCameraDuration);
        }

        // Check if the player lost and show the red flash/indicator *before* the fade starts
        if (GameManager.Instance != null && GameManager.Instance.IsChosenCharacterEliminated())
        {
            if (redLossIndicatorImage != null)
            {
                redLossIndicatorImage.gameObject.SetActive(true);
                yield return new WaitForSeconds(0.75f); // Brief pause so the player registers the red alert
            }
        }

        // Fade out to black
        yield return StartCoroutine(FadeToBlack());

        // Check if the player's chosen character just got booted
        if (GameManager.Instance != null && GameManager.Instance.IsChosenCharacterEliminated())
        {
            Debug.Log("Your chosen character got eliminated! Turning off game UI and showing the You Lost panel.");

            // Turn off the first canvas (UI + fade text)
            if (gameUICanvas != null)
            {
                gameUICanvas.SetActive(false);
            }

            // Turn on the You Lost panel canvas
            if (youLostPanel != null)
            {
                youLostPanel.SetActive(true);
            }
            else
            {
                SceneManager.LoadScene("Game Menu");
            }
        }
        else
        {
            SceneManager.LoadScene("Producer Office");
        }
    }

    // Call this method from your You Lost panel button's OnClick event in the Inspector
    public void OnReturnToMenuClicked()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ResetGame();
            buttonSound.Play();
        }
        SceneManager.LoadScene("Game Menu");
    }

    IEnumerator FadeToBlack()
    {
        if (fadePanelImage == null) yield break;

        float timer = 0f;
        Color startColor = fadePanelImage.color;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float alpha = Mathf.Clamp01(timer / fadeDuration);

            Color newColor = startColor;
            newColor.a = alpha;
            fadePanelImage.color = newColor;

            yield return null;
        }

        Color finalColor = startColor;
        finalColor.a = 1f;
        fadePanelImage.color = finalColor;
    }
}