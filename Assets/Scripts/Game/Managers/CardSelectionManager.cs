using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardSelectionManager : MonoBehaviour
{
    [Header("Corruption Cards")]
    public List<GameObject> corruptionCards; // Drag all 5 cards in here
    public string selectedCardTag = "";

    [Header("Sounds UI")]
    public AudioSource buttonSound1;
    public AudioSource buttonSound2;


    private int currentIndex = 0;

    // Show only the first card on start
    void Start()
    {
        UpdateDisplay();
    }

    // Show the previous corruption card
    public void OnLeftArrow()
    {
        currentIndex--;
        if (currentIndex < 0) currentIndex = corruptionCards.Count - 1;
        buttonSound2.Play();
        UpdateDisplay();
    }

    // Show the next corruption card
    public void OnRightArrow()
    {
        currentIndex++;
        if (currentIndex >= corruptionCards.Count) currentIndex = 0;
        buttonSound2.Play();
        UpdateDisplay();
    }

    //updates the view so you can scroll through all cards
    void UpdateDisplay()
    {
        foreach (GameObject card in corruptionCards)
        {
            card.SetActive(false);
        }
        corruptionCards[currentIndex].SetActive(true);
    }

    // Confirms the clicked card as your choice
    public void OnSelectClicked(string cardTag)
    {
        buttonSound1.Play();
        selectedCardTag = cardTag;
        GameManager.Instance.selectedCard = cardTag;
        Debug.Log("Chosen card: " + cardTag);

        // Store card component before panel hides
        GameObject cardObject = GameObject.FindWithTag(cardTag);
        CardApplicationManager cardManager = FindObjectOfType<CardApplicationManager>();
        cardManager.selectedCardData = cardObject.GetComponent<Card>();

        // If help card skip target selection and confirm immediately
        if (cardManager.selectedCardData.targetsChosenCharacter)
        {
            cardManager.ConfirmCard();
            return;
        }

        // Otherwise open target selection
        TargetSelectionManager targetManager = FindObjectOfType<TargetSelectionManager>();
        targetManager.OpenTargetSelection();
    }
}
