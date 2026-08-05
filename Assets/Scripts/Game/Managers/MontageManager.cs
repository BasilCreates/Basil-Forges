using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MontageManager : MonoBehaviour
{
    [Header("Cameras")]
    public List<Camera> cameras;

    [Header("Bad Edit Camera")]
    public Camera badEditCamera;

    [Header("Stage")]
    public Transform spawnPoint;

    [Header("Scene Settings")]
    public string voteRevealScene = "VoteReveal";
    public float timePerContestant = 10f;
    public float cameraSwitchInterval = 3f;

    [Header("Contestants")]
    public List<string> contestantTags = new List<string>
    { "Amara", "Zarah", "Priya", "Tariq", "Idris" };
    private bool montageRunning = false;

    [Header("Fade")]
    public Image fadePanel;
    public float fadeDuration = 0.5f;

    [Header("Bools")]
    private bool badEditActive = false;

    [Header("Sounds")]
    public AudioSource badEditSound;
    public AudioSource shadowCutSound;
    public AudioSource performanceMusic;

    // Tracks the previously used camera index so it never repeats consecutively
    private int lastCameraIndex = -1;

    // Turns off all cameras and kicks off the montage
    void Start()
    {
        GameManager.Instance.UpdateChosenCharacterUI();

        // Tick suspicion down at start of each montage
        SuspicionManager suspicionManager = FindObjectOfType<SuspicionManager>();
        if (suspicionManager != null)
        {
            suspicionManager.TickDown();
        }
        DisableAllCameras();
        StartCoroutine(RunMontage());
    }

    // Brings each contestant to the spawn point one at a time
    IEnumerator RunMontage()
    {
        montageRunning = true;
        StartCoroutine(SwitchCameras());

        foreach (string tag in contestantTags)
        {
            if (GameManager.Instance.IsEliminated(tag)) continue;

            GameObject contestant = GameObject.FindWithTag(tag);
            if (contestant == null) continue;

            // Fade to black before swap
            yield return StartCoroutine(Fade(0f, 1f));

            // Stop previous performance music
            StartCoroutine(FadeMusicOut());

            // Move everyone off screen
            foreach (string otherTag in contestantTags)
            {
                GameObject other = GameObject.FindWithTag(otherTag);
                if (other != null) other.transform.position = new Vector3(0f, -100f, 0f);
            }

            // Move current contestant to spawn point
            contestant.transform.position = spawnPoint.position;
            contestant.transform.rotation = spawnPoint.rotation;

            // Fade back in
            yield return StartCoroutine(Fade(1f, 0f));

            Debug.Log("Now showing: " + tag);
            SongData song = GameManager.Instance.GetSong(tag);

            if (song != null && song.clip != null)
            {
                performanceMusic.Stop();

                Debug.Log("Assigning song: " + song.clip.name);
                Debug.Log("Audio Source: " + performanceMusic);

                performanceMusic.clip = song.clip;

                performanceMusic.time = Mathf.Clamp(song.startTime, 0f, song.clip.length - 0.1f);

                performanceMusic.Play();
            }

            if (tag == GameManager.Instance.targetedCharacter &&
                GameManager.Instance.selectedCard == "Bad Edit")
            {
                badEditActive = true;
                DisableAllCameras();
                badEditCamera.gameObject.SetActive(true);
                Debug.Log("Bad Edit in Effect!");
                badEditSound.Play();
            }
            else
            {
                badEditSound.Stop();
            }

            // Check if this contestant is shadow cut target
            float performanceTime = timePerContestant;
            if (tag == GameManager.Instance.targetedCharacter &&
                GameManager.Instance.selectedCard == "Shadow Cut")
            {
                performanceTime = timePerContestant / 2f;
                shadowCutSound.Play();
                Debug.Log("Shadow Cut in Effect!");
            }
            else
            {
                shadowCutSound.Stop();
            }

            // Wait for their performance (or skip early with P)
            yield return StartCoroutine(WaitForPerformanceOrSkip(performanceTime));
            badEditActive = false; // Reset for next contestant
        }

        // Final fade to black before vote reveal
        yield return StartCoroutine(Fade(0f, 1f));

        montageRunning = false;
        performanceMusic.Stop();
        SceneManager.LoadScene(voteRevealScene);
    }

    // Fades the panel from one alpha to another
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

    // Picks a random camera every 3 seconds while montage is running (guarantees no back-to-back repeats)
    IEnumerator SwitchCameras()
    {
        while (montageRunning)
        {
            // Don't switch cameras if bad edit is active
            if (!badEditActive && cameras.Count > 0)
            {
                int randomIndex = lastCameraIndex;

                if (cameras.Count > 1)
                {
                    while (randomIndex == lastCameraIndex)
                    {
                        randomIndex = Random.Range(0, cameras.Count);
                    }
                }
                else
                {
                    randomIndex = 0;
                }

                lastCameraIndex = randomIndex;

                DisableAllCameras();
                cameras[randomIndex].gameObject.SetActive(true);
            }

            yield return new WaitForSeconds(cameraSwitchInterval);
        }
    }

    // Turns off every camera before enabling the chosen one
    void DisableAllCameras()
    {
        foreach (Camera cam in cameras)
        {
            cam.gameObject.SetActive(false);
        }
        // Also disable bad edit camera
        if (badEditCamera != null)
            badEditCamera.gameObject.SetActive(false);
    }

    IEnumerator FadeMusicOut()
    {
        float startVolume = performanceMusic.volume;

        while (performanceMusic.volume > 0)
        {
            performanceMusic.volume -= startVolume * Time.deltaTime / fadeDuration;
            yield return null;
        }

        performanceMusic.Stop();
        performanceMusic.volume = startVolume;
    }

    // Waits out the performance time, but breaks early if P is pressed
    IEnumerator WaitForPerformanceOrSkip(float duration)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            if (Input.GetKeyDown(KeyCode.P))
            {
                Debug.Log("Performance skipped by player");
                yield break; // exits immediately, skips straight to the fade/next contestant
            }

            elapsed += Time.deltaTime;
            yield return null;
        }
    }
}