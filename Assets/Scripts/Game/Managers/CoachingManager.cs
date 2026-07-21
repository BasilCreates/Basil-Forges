using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CoachingManager : MonoBehaviour
{
    [Header("Cameras")]
    public Camera camera1;
    public Camera camera2;

    [Header("Timing")]
    public float camera1Duration = 5f;
    public float camera2Duration = 5f;

    [Header("Spawn Points")]
    public Transform spawnPoint;
    public Transform producerSpawnPoint;

    [Header("Fade")]
    public RawImage fadePanel;
    public float fadeDuration = 0.5f;

    [Header("Scene Settings")]
    public string montageScene = "Contestant Stage";

    // Spawns characters and kicks off the sequence
    void Start()
    {
        // Move chosen character to their spawn point
        GameObject chosenContestant = GameObject.FindWithTag(
            GameManager.Instance.chosenCharacter);
        if (chosenContestant != null)
        {
            chosenContestant.transform.position = spawnPoint.position;
            chosenContestant.transform.rotation = spawnPoint.rotation;
        }

        // Move producer to their spawn point
        GameObject producer = GameObject.FindWithTag("Player");
        if (producer != null)
        {
            producer.transform.position = producerSpawnPoint.position;
            producer.transform.rotation = producerSpawnPoint.rotation;
        }

        DisableAllCameras();
        StartCoroutine(RunSequence());
    }

    // Runs cameras in order with adjustable timing
    IEnumerator RunSequence()
    {
        // Fade in at the start
        yield return StartCoroutine(Fade(1f, 0f));

        // Camera 1
        camera1.gameObject.SetActive(true);
        yield return new WaitForSeconds(camera1Duration);
        camera1.gameObject.SetActive(false);

        // Camera 2
        camera2.gameObject.SetActive(true);
        yield return new WaitForSeconds(camera2Duration);
        camera2.gameObject.SetActive(false);

        // Fade out then load montage
        yield return StartCoroutine(Fade(0f, 1f));

        SceneManager.LoadScene(montageScene);
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

    // Disables all cameras before sequence starts
    void DisableAllCameras()
    {
        camera1.gameObject.SetActive(false);
        camera2.gameObject.SetActive(false);
    }
}