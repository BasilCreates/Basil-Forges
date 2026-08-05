using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CaughtManager : MonoBehaviour
{
    [Header("Player Parameters")]
    public GameObject player;
    public Rigidbody rb;

    [Header("Bool Parameters")]
    public bool inSight = false;

    [Header("Animator Parameters")]
    public Animator playerAnimator;

    [Header("Camera Transform")]
    public Transform cameraTransform;

    [Header("Smooth Turn Parameters")]
    public float rotationSpeed = 10f;

    [Header("Breaking News Panel")]
    public GameObject breakingNewsPanel;
    public float secondsToAppear = 1f;

    [Header("Panel Slide Parameters")]
    public float slideSpeed = 5f;
    public Vector3 startOffset = new Vector3(-500f, 0f, 0f);

    [Header("Scene Fade Parameters")]
    public RawImage fadeRawImage;
    public float fadeInDuration = 1f;
    public float fadeOutDuration = 1f;

    [Header("Lose Panel")]
    public GameObject losePanel; // Keep this hidden until player completely loses

    private Quaternion targetRotation;
    private bool shouldRotate = false;
    private RectTransform panelRectTransform;
    private Vector3 centerPosition;

    [Header("Sounds")]
    public AudioSource footsteps;
    public AudioSource chains;
    public AudioSource breakingNews;
    public AudioSource buttonSound;

    public void Start()
    {

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true; 
        // Start ambient background sounds right away
        footsteps.Play();
        chains.Play();

        inSight = false;
        rb = GetComponent<Rigidbody>();

        if (breakingNewsPanel != null)
        {
            breakingNewsPanel.SetActive(false);
            panelRectTransform = breakingNewsPanel.GetComponent<RectTransform>();
            if (panelRectTransform != null)
            {
                centerPosition = panelRectTransform.anchoredPosition;
            }
        }

        // Hide lose panel at start so it doesn't spoil the jumpscare
        if (losePanel != null)
        {
            losePanel.SetActive(false);
        }

        // Fade in from black when the scene boots up
        if (fadeRawImage != null)
        {
            Color c = fadeRawImage.color;
            c.a = 1f;
            fadeRawImage.color = c;
            StartCoroutine(SceneFadeInRoutine());
        }
    }

    public void Update()
    {
        // Smoothly makes player's rotation toward the camera when spotted
        if (shouldRotate && player != null)
        {
            player.transform.rotation = Quaternion.Slerp(player.transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (!inSight && gameObject.CompareTag("Player"))
        {
            TriggerCaughtSequence();
        }
        else if (!inSight && playerAnimator != null)
        {
            TriggerCaughtSequence();
        }
    }

    void TriggerCaughtSequence()
    {
        inSight = true;

        if (playerAnimator != null)
        {
            playerAnimator.SetBool("InSight", true);

            // Stop movement noises and drop the breaking news audio cue
            chains.Stop();
            footsteps.Stop();
            breakingNews.Play();
        }

        // Lock onto the camera position for dramatic effect
        if (player != null && cameraTransform != null)
        {
            targetRotation = Quaternion.LookRotation(new Vector3(cameraTransform.position.x - player.transform.position.x, 0f, cameraTransform.position.z - player.transform.position.z)) * Quaternion.Euler(0f, 0f, 0f);
            shouldRotate = true;
        }

        StartCoroutine(BreakingNewsRoutine());

        // Fade to black as the sequence finishes up
        if (fadeRawImage != null)
        {
            StartCoroutine(SceneFadeOutRoutine());
        }
    }

    IEnumerator BreakingNewsRoutine()
    {
        // Wait a bit before popping up the breaking news graphic
        yield return new WaitForSeconds(secondsToAppear);

        if (breakingNewsPanel != null)
        {
            breakingNewsPanel.SetActive(true);

            if (panelRectTransform != null)
            {
                // Slide it in from the offset position smoothly
                panelRectTransform.anchoredPosition = centerPosition + startOffset;

                float elapsedTime = 0f;
                Vector3 initialPos = panelRectTransform.anchoredPosition;

                while (elapsedTime < 1f)
                {
                    elapsedTime += Time.deltaTime * slideSpeed;
                    panelRectTransform.anchoredPosition = Vector3.Lerp(initialPos, centerPosition, elapsedTime);
                    yield return null;
                }

                panelRectTransform.anchoredPosition = centerPosition;
            }
        }
    }

    IEnumerator SceneFadeInRoutine()
    {
        float elapsedTime = 0f;
        Color startColor = fadeRawImage.color;

        while (elapsedTime < fadeInDuration)
        {
            elapsedTime += Time.deltaTime;
            float newAlpha = Mathf.Clamp01(1f - (elapsedTime / fadeInDuration));
            fadeRawImage.color = new Color(startColor.r, startColor.g, startColor.b, newAlpha);
            yield return null;
        }

        fadeRawImage.color = new Color(startColor.r, startColor.g, startColor.b, 0f);
    }

    IEnumerator SceneFadeOutRoutine()
    {
        float elapsedTime = 0f;
        Color startColor = fadeRawImage.color;

        while (elapsedTime < fadeOutDuration)
        {
            elapsedTime += Time.deltaTime;
            float newAlpha = Mathf.Clamp01(elapsedTime / fadeOutDuration);
            fadeRawImage.color = new Color(startColor.r, startColor.g, startColor.b, newAlpha);
            yield return null;
        }

        fadeRawImage.color = new Color(startColor.r, startColor.g, startColor.b, 1f);

        // Fade is done, safe to show the game over panel now
        ActivateLosePanel();
    }

    void ActivateLosePanel()
    {
        if (losePanel != null)
        {
            losePanel.SetActive(true);
        }
    }

    // Hook this up to the UI button OnClick event to head back to the main menu
    public void OnContinueClicked()
    {
        buttonSound.Play();
        SceneManager.LoadScene("Game Menu");
    }
}