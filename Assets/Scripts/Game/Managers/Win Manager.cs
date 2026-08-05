using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class WinManager : MonoBehaviour
{
    [Header("Spawn & Character")]
    public Transform characterSpawnSpot;
    public GameObject[] characterObjects;

    [Header("UI Text")]
    public Text winningCharacterNameText;

    [Header("Cameras & Timing")]
    public List<Camera> montageCameras;
    public float defaultCameraDuration = 3f;

    [Header("Fade UI")]
    public Image fadePanelImage;
    public float fadeDuration = 1.5f;

    [Header("Canvases & Panels")]
    public GameObject gameUICanvas;
    public AudioSource buttonSound;

    [Header("Win Panel & Payout")]
    public GameObject youWonPanel;
    public Text winnerEarningsText;

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

        if (gameUICanvas != null)
        {
            gameUICanvas.SetActive(true);
        }

        if (youWonPanel != null)
        {
            youWonPanel.SetActive(false);
        }

        SpawnWinningCharacter();
        StartCoroutine(RunWinSequence());
    }

    void SpawnWinningCharacter()
    {
        // Finds the actual chosen winning character from the GameManager
        string winningName = "";

        if (GameManager.Instance != null)
        {
            if (!string.IsNullOrEmpty(GameManager.Instance.chosenCharacter))
            {
                winningName = GameManager.Instance.chosenCharacter.Trim();
            }
            else if (!string.IsNullOrEmpty(GameManager.Instance.targetedCharacter))
            {
                winningName = GameManager.Instance.targetedCharacter.Trim();
            }
        }

        Debug.Log("Looking for winning character tag: '" + winningName + "'");

        if (winningCharacterNameText != null)
        {
            winningCharacterNameText.text = winningName;
        }

        // Handle Payout Logic for the Win Panel
        if (GameManager.Instance != null)
        {
            if (GameManager.Instance.winnerPayout == 0f && GameManager.Instance.payoutResults.ContainsKey(GameManager.Instance.chosenCharacter))
            {
                GameManager.Instance.winnerPayout = GameManager.Instance.payoutResults[GameManager.Instance.chosenCharacter];
            }

            if (winnerEarningsText != null)
            {
                winnerEarningsText.text = "$" + GameManager.Instance.winnerPayout.ToString("N0");
            }
        }

        if (characterSpawnSpot == null || characterObjects.Length == 0) return;

        // Loop through array and activate the character matching the tag or name
        foreach (GameObject obj in characterObjects)
        {
            if (obj != null)
            {
                bool tagMatches = false;
                try
                {
                    tagMatches = obj.CompareTag(winningName);
                }
                catch
                {
                    // Catch if tag isn't registered in tag manager
                }

                bool nameMatches = obj.name.Equals(winningName, System.StringComparison.OrdinalIgnoreCase);

                if (tagMatches || nameMatches)
                {
                    obj.transform.position = characterSpawnSpot.position;
                    obj.transform.rotation = characterSpawnSpot.rotation;
                    obj.SetActive(true);
                    Debug.Log("Activated winning character: " + obj.name);
                    return;
                }
            }
        }

        Debug.LogWarning("Couldn't find an object with tag or name matching '" + winningName + "'!");
    }

    IEnumerator RunWinSequence()
    {
        if (montageCameras == null || montageCameras.Count == 0)
        {
            Debug.LogWarning("Yo, you forgot to assign cameras to the win montage list!");
            yield break;
        }

        // Cycle through the montage cameras
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

        // Fade out to black
        yield return StartCoroutine(FadeToBlack());

        Debug.Log("Win sequence finished! Turning off game UI and showing the You Won panel.");

        // Turn off gameplay canvas
        if (gameUICanvas != null)
        {
            gameUICanvas.SetActive(false);
        }

        // Turn on the You Won panel canvas
        if (youWonPanel != null)
        {
            youWonPanel.SetActive(true);
        }
        else
        {
            SceneManager.LoadScene("Game Menu");
        }
    }

    // Call this method from your Continue button's OnClick event in the Inspector to go straight back to the Game Menu
    public void OnReturnToMenuClicked()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ResetGame();
            if (buttonSound != null) buttonSound.Play();
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