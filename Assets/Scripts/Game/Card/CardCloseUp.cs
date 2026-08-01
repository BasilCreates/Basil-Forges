using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UI;

public class CardCloseUp : MonoBehaviour
{
    [Header("Card Parameters")]
    public GameObject cardIndex; //cards on the table
    public GameObject cardCloseIndex; //cards close up when you click them

    [Header("Game Text Parameters")]
    public GameObject explanationText; //to turn off the explanation text to display our other text
    public GameObject offText;


    public void Start()
    {

      cardCloseIndex.SetActive(false); //turn off the close card index to avoid blocking player view
    }

    public void Update()
    {
        CloseCard(); //runs our closecard method
    }

    public void ClickCard()
    {
        cardCloseIndex.SetActive(true); //turns on the corresponding card close up
        explanationText.SetActive(false);
        offText.SetActive(true);
    }

    public void CloseCard()
    {
        if(Input.GetKeyDown(KeyCode.L))
        {
            cardCloseIndex.SetActive(false);  //turns off the card close up
            explanationText.SetActive(true);
            offText.SetActive(false);
        }
    }
}
