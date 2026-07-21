using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialManager : MonoBehaviour
{
    [Header("Page Attributes")]
    public GameObject page1;
    public GameObject page2;

    [Header("Sounds UI")]
    public AudioSource buttonSound;


    //making sure the second page is off
    public void Start()
    {
        page2.SetActive(false);
    }

    //loads the next panel in the tutorial
    public void OnNextClicked()
    {
        page1.SetActive(false);
        page2.SetActive(true);
        buttonSound.Play();
    }

    //loads up the producer scene
    public void onPlayClicked()
    {
        SceneManager.LoadScene("Producer Office");
        buttonSound.Play();
    }
}
