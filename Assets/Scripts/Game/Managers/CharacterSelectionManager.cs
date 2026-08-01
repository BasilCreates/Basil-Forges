using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectionManager : MonoBehaviour
{
    [Header("UI Canvases")]
    public GameObject characterSelectionCanvas; // NEW: The base UI for character selection (comes on first)
    public GameObject cardSelectionCanvas;      // NEW: The base UI for card selection (comes on next)
    public GameObject charactersEnlargedCanvas; // RENAMED: The canvas with the character hover cards
    public GameObject cardsEnlargedCanvas;      // RENAMED: The canvas with the corruption hover cards

    [Header("3D Character Models")]
    public List<GameObject> characterModels; // Drag all your 3D characters from the scene in here

    [Header("Character Cards")]
    public List<GameObject> characterCards; // Drag all 5 cards in here
    public string selectedCharacterTag = "";

    [Header("Sounds UI")]
    public AudioSource buttonSound1;
    public AudioSource buttonSound2;

    // We use this to track who we are currently hovering over, so we don't spam the sound every frame
    private string currentHoveredTag = "";

    // A safety lock so we don't accidentally hover or click multiple characters during transition
    private bool hasSelected = false;

    // Show all cards on start so the screen is clear to view the 3D models
    void Start()
    {
        HideAllCards();

        // Ensure the correct starting state for the base canvases
        if (characterSelectionCanvas != null) characterSelectionCanvas.SetActive(true);
        if (cardSelectionCanvas != null) cardSelectionCanvas.SetActive(false);
    }

    // Called by Raycast script when hovered
    public void HoverCharacter(string characterTag)
    {
        // If we already picked a character, ignore any further hovers
        if (hasSelected) return;

        // If we are already hovering over this character, do nothing so we don't spam the UI and Sounds
        if (currentHoveredTag == characterTag) return;

        currentHoveredTag = characterTag;
        Debug.Log("Hovering character via raycast: " + characterTag);

        buttonSound2.Play();
        HideAllCards(); // Clear all cards first

        // Find and turn on the specific card matching the clicked character's tag
        foreach (GameObject card in characterCards)
        {
            if (card.tag == characterTag)
            {
                card.SetActive(true);
                UpdateStats(characterTag, card);
                return;
            }
        }

        Debug.LogWarning("No character card found matching the tag: " + characterTag);
    }

    // Called by Raycast script when we look away
    public void ClearHover()
    {
        if (hasSelected) return; // Do nothing if we already made a choice

        // If nothing is open, do nothing
        if (currentHoveredTag == "") return;

        currentHoveredTag = ""; // Reset the tracker
        HideAllCards(); // Hide the card
    }

    // Hide all cards
    void HideAllCards()
    {
        foreach (GameObject card in characterCards)
        {
            card.SetActive(false);
        }
    }

    // Confirms the clicked character as your choice
    public void OnSelectClicked(string characterTag)
    {
        if (hasSelected) return; // Prevent clicking twice
        hasSelected = true;      // Lock in the choice

        selectedCharacterTag = characterTag;
        GameManager.Instance.chosenCharacter = characterTag;
        Debug.Log("Chosen character: " + characterTag);

        buttonSound1.Play();

        // Turn off all 3D character models in the scene
        foreach (GameObject model in characterModels)
        {
            if (model != null)
            {
                model.SetActive(false);
            }
        }

        // --- NEW CANVAS SWITCHING LOGIC ---

        // Turn OFF Character Canvases
        if (characterSelectionCanvas != null) characterSelectionCanvas.SetActive(false);
        if (charactersEnlargedCanvas != null) charactersEnlargedCanvas.SetActive(false);

        // Turn ON Card Canvases
        if (cardSelectionCanvas != null) cardSelectionCanvas.SetActive(true);
        if (cardsEnlargedCanvas != null) cardsEnlargedCanvas.SetActive(true);
    }

    // Takes our values from our stats generated on each character and converts them into readable string for the game
    void UpdateStats(string tag, GameObject card)
    {
        GameObject character = GameObject.FindWithTag(tag);
        if (character == null) return;

        CurrentStats stats = character.GetComponent<CurrentStats>();

        // We wrap each stat check in { } so we can update both the Text and the Slider at the same time
        foreach (DefaultStats stat in stats.defaultStats)
        {
            if (stat.statName == "Audience Appeal")
            {
                card.transform.Find("Stats/Audience Value").GetComponent<Text>().text = stat.statAmount.ToString();
                card.transform.Find("Stats/Audience Bar").GetComponent<Slider>().value = stat.statAmount;
            }
            if (stat.statName == "Talent")
            {
                card.transform.Find("Stats/Talent Value").GetComponent<Text>().text = stat.statAmount.ToString();
                card.transform.Find("Stats/Talent Bar").GetComponent<Slider>().value = stat.statAmount;
            }
            if (stat.statName == "Toxicity")
            {
                card.transform.Find("Stats/Toxicity Value").GetComponent<Text>().text = stat.statAmount.ToString();
                card.transform.Find("Stats/Toxicity Bar").GetComponent<Slider>().value = stat.statAmount;
            }
            if (stat.statName == "TV Potential")
            {
                card.transform.Find("Stats/TV Potential Value").GetComponent<Text>().text = stat.statAmount.ToString();
                card.transform.Find("Stats/TV Potential Bar").GetComponent<Slider>().value = stat.statAmount;
            }
        }
    }
}