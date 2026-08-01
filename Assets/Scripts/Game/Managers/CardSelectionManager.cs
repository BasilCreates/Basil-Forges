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

    [Header("Selection Panels")]
    public GameObject targetSelectionPanel;
    public GameObject cardSelectionPanel;

    // We use this to track who we are currently hovering over, so we don't spam the sound every frame
    private string currentHoveredTag = "";

    // Show only the first card on start - UPDATED to hide all cards initially
    void Start()
    {
        HideAllCards();
    }

    // Called by Raycast script when hovered
    public void HoverCard(string cardTag)
    {
        // If we are already hovering over this card, do nothing
        if (currentHoveredTag == cardTag) return;

        currentHoveredTag = cardTag;
        Debug.Log("Hovering card via raycast: " + cardTag);

        buttonSound2.Play();
        HideAllCards(); // Clear all cards first

        // Find and turn on the specific card matching the clicked card's tag
        foreach (GameObject card in corruptionCards)
        {
            if (card.tag == cardTag)
            {
                card.SetActive(true);
                return;
            }
        }

        Debug.LogWarning("No UI card found matching the tag: " + cardTag);
    }

    // Called by Raycast script when we look away
    public void ClearHover()
    {
        // If nothing is open, do nothing
        if (currentHoveredTag == "") return;

        currentHoveredTag = ""; // Reset the tracker
        HideAllCards(); // Hide the card
    }

    // updates the view so you can scroll through all cards - UPDATED to just hide them
    void HideAllCards()
    {
        foreach (GameObject card in corruptionCards)
        {
            card.SetActive(false);
        }
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

        // Otherwise open target selection and explicitly populate the list
        TargetSelectionManager targetManager = FindObjectOfType<TargetSelectionManager>();
        if (targetManager != null)
        {
            targetManager.OpenTargetSelection(); // This populates availableTargets and enables models
        }

        if (cardSelectionPanel != null) cardSelectionPanel.SetActive(false);
        if (targetSelectionPanel != null) targetSelectionPanel.SetActive(true);
    }
}