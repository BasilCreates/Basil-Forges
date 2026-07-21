using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectionManager : MonoBehaviour
{
    [Header("Panels")]
    public GameObject characterSelectionPanel;
    public GameObject cardSelectionPanel;

    [Header("Character Cards")]
    public List<GameObject> characterCards; // Drag all 5 cards in here
    public string selectedCharacterTag = "";

    [Header("Sounds UI")]
    public AudioSource buttonSound1;
    public AudioSource buttonSound2;

    private int currentIndex = 0;

    // Show only the first card on start
    void Start()
    {
        UpdateDisplay();
    }

    // Show the previous character card
    public void OnLeftArrow()
    {
        currentIndex--;
        if (currentIndex < 0) currentIndex = characterCards.Count - 1;
        buttonSound2.Play();
        UpdateDisplay();
    }

    // Show the next character card
    public void OnRightArrow()
    {
        currentIndex++;
        if (currentIndex >= characterCards.Count) currentIndex = 0;
        buttonSound2.Play();
        UpdateDisplay();
    }

    // Confirms the clicked character as your choice
    public void OnSelectClicked(string characterTag)
    {
        selectedCharacterTag = characterTag;
        GameManager.Instance.chosenCharacter = characterTag;
        Debug.Log("Chosen character: " + characterTag);

        buttonSound1.Play();

        // Move to card selection panel
        characterSelectionPanel.SetActive(false);
        cardSelectionPanel.SetActive(true);
    }

    // Hide all cards then show only the current one
    void UpdateDisplay()
    {
        foreach (GameObject card in characterCards)
        {
            card.SetActive(false);
        }
        characterCards[currentIndex].SetActive(true);
        UpdateStats(characterCards[currentIndex].tag);
    }

    //takes our values from our stats generated on each character and converts them into readable string for the game
    void UpdateStats(string tag)
    {
        GameObject character = GameObject.FindWithTag(tag);
        if (character == null) return;
        CurrentStats stats = character.GetComponent<CurrentStats>();
        GameObject card = characterCards[currentIndex];
        foreach (DefaultStats stat in stats.defaultStats)
        {
            if (stat.statName == "Audience Appeal")
                card.transform.Find("Stats/Audience Value").GetComponent<Text>().text = stat.statAmount.ToString();
            if (stat.statName == "Talent")
                card.transform.Find("Stats/Talent Value").GetComponent<Text>().text = stat.statAmount.ToString();
            if (stat.statName == "Toxicity")
                card.transform.Find("Stats/Toxicity Value").GetComponent<Text>().text = stat.statAmount.ToString();
            if (stat.statName == "TV Potential")
                card.transform.Find("Stats/TV Potential Value").GetComponent<Text>().text = stat.statAmount.ToString();
        }
    }
}