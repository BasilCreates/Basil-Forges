using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TargetSelectionManager : MonoBehaviour
{
    [Header("Target Cards")]
    public List<GameObject> targetCards; // Drag character cards in here
    public Text targetNameText; // Shows current target name

    [Header("Panels")]
    public GameObject cardSelectionPanel;
    public GameObject targetSelectionPanel;

    [Header("Sounds UI")]
    public AudioSource buttonSound1;
    public AudioSource buttonSound2;

    private int currentIndex = 0;
    private List<string> availableTargets = new List<string>();



    // Called after card is selected, builds list of valid targets
    public void OpenTargetSelection()
    {
        // Build list of valid targets - not eliminated, not chosen character
        availableTargets.Clear();

        foreach (GameObject card in targetCards)
        {
            string tag = card.tag;

            if (!GameManager.Instance.IsEliminated(tag) &&
                tag != GameManager.Instance.chosenCharacter)
            {
                availableTargets.Add(tag);
            }
        }

        currentIndex = 0;
        cardSelectionPanel.SetActive(false);
        targetSelectionPanel.SetActive(true);
        UpdateDisplay();
    }

    // Show previous target
    public void OnLeftArrow()
    {
        currentIndex--;
        if (currentIndex < 0) currentIndex = availableTargets.Count - 1;
        buttonSound2.Play();
        UpdateDisplay();
    }

    // Show next target
    public void OnRightArrow()
    {
        currentIndex++;
        if (currentIndex >= availableTargets.Count) currentIndex = 0;
        buttonSound2.Play();
        UpdateDisplay();
    }

    // Confirm target and apply card
    public void OnConfirmTarget(string characterTag)
    {
        buttonSound1.Play();
        GameManager.Instance.targetedCharacter = characterTag;
        Debug.Log("Targeting: " + characterTag);

        CardApplicationManager cardManager = FindObjectOfType<CardApplicationManager>();
        cardManager.ConfirmCard();
    }

    // Updates display to show current target
    void UpdateDisplay()
    {
        foreach (GameObject card in targetCards)
        {
            card.SetActive(false);
        }

        // Find and show the card matching current target
        foreach (GameObject card in targetCards)
        {
            if (card.tag == availableTargets[currentIndex])
            {
                card.SetActive(true);
                if (targetNameText != null)
                    targetNameText.text = availableTargets[currentIndex];
                break;
            }
        }
    }
}