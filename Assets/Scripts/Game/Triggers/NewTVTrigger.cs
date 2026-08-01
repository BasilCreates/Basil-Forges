using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class NewTVTrigger : MonoBehaviour
{
    [Header("Panel Parameters")]
    public GameObject gameExplanationText; //Game Explanation text to show
    public GameObject missionText; //The mission Text to turn off;

    [Header("Card Glow Components")]
    public Outline[] cardOutline; //our card outline shaders to make it easier for players to see

    [Header("Notification Sound")]
    public AudioSource notificationSound;

    [Header("Trigger Parameter")]
    public GameObject newTVTrigger;

    [Header("Arrow Parameters")]
    public GameObject[] arrowIndicator;


    public void Start()
    {
        foreach (Outline outline in cardOutline)
        {
            outline.enabled = false; //run a check to make sure that each outline is off.
        }
        foreach (GameObject arrows in arrowIndicator)
        {
           arrows.SetActive(false); //run a check to turn the arrows off in the beginning
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        gameExplanationText.SetActive(true); //turns on the explanation text
        missionText.SetActive(false); //turns off the mission text
        notificationSound.Play(); //plays the paper shifting sound

        foreach (Outline outline in cardOutline)
        {
            outline.enabled = true; //Turns on our card outline shader.
        }

        foreach (GameObject arrows in arrowIndicator)
        {
            arrows.SetActive(true); //run a check to turn the arrows on 
        }

        newTVTrigger.SetActive(false); //turns off the trigger to prevent repeated use. 
        
    }
}
