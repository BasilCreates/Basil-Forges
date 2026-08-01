using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TVInteraction : MonoBehaviour
{
    [Header("Screen Parameters")]
    public GameObject offScreen; //our black screen
    public GameObject onScreen; //the on screen with our static mp4

    [Header("Fade Panel Parameters")]
    public Image fadePanel;
    public float fadeSpeed = 5f;
    public float waitToFade = 5f;

    [Header("Scene Parameters")]
    public string sceneName;

    [Header("Screen Bool Parameters")]
    public bool tvOn = false;

    public void Start()
    {
        tvOn = false; //make sure you cannot press enter   
        offScreen.SetActive(true); //turns on our black screen
        onScreen.SetActive(false); //turns off our screen with the static mp4
    }

    public void Update()
    {
        LevelStart();
    }
    public void TurnOnTV()
    {
        Debug.Log("Hit the TV!");
        offScreen.SetActive(false); //turns off our black screen
        onScreen.SetActive(true); //turns on our screen with the static mp4
    }

    public void LevelStart()
    {
        

        if(Input.GetKeyDown(KeyCode.Return) && !tvOn) //When Enter is clicked
        {
            Debug.Log("Hit the Button");
            tvOn = true;
            StartCoroutine(FadeIn()); //start the fade transition

        }
    }

    IEnumerator FadeIn()
    {
        yield return new WaitForSeconds(waitToFade); //waits before fading
        float alpha = 0f;

        while (alpha < 1f)
        {
            alpha += Time.deltaTime * fadeSpeed;

            Color color = fadePanel.color;
            color.a = alpha;
            fadePanel.color = color;

            yield return null;
        }
        SceneManager.LoadScene(sceneName); //loads our scene
    }
}
