using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    [Header("UI Canvases")]
    public GameObject characterPickCanvas;
    public GameObject cardSelectionCanvas;
    public GameObject targetSelectionCanvas;

    [Header("UI Text (Inspector Assign)")]
    public Text weekTitleText;
    public Text chosenCharacterText; // Inspector assign your target UI text here

    void Start()
    {
        // Check if it's the first round/week
        if (GameManager.Instance.currentRound == 1)
        {
            // First week: Show character selection, hide card and target selection
            if (characterPickCanvas != null) characterPickCanvas.SetActive(true);
            if (cardSelectionCanvas != null) cardSelectionCanvas.SetActive(false);
            if (targetSelectionCanvas != null) targetSelectionCanvas.SetActive(false);

            Debug.Log("Loaded Week 1: Character Pick");
        }
        else
        {
            // Future rounds: Hide character pick, start directly from card selection
            if (characterPickCanvas != null) characterPickCanvas.SetActive(false);
            if (cardSelectionCanvas != null) cardSelectionCanvas.SetActive(true);
            if (targetSelectionCanvas != null) targetSelectionCanvas.SetActive(false);

            Debug.Log("Loaded Week " + GameManager.Instance.currentRound + ": Skipping Character Pick");
        }

        RefreshUI();
    }

    void OnEnable()
    {
        RefreshUI();
    }

    // Call this method whenever you transition to a new screen or open the target UI!
    public void RefreshUI()
    {
        UpdateWeekText();
        UpdateChosenCharacterText();
    }

    // Updates all week text fields
    public void UpdateWeekText()
    {
        if (GameManager.Instance == null) return;

        int currentWeek = Mathf.Clamp(GameManager.Instance.currentRound, 1, 5);
        string weekString = "Week " + currentWeek;

        if (weekTitleText != null)
        {
            weekTitleText.text = weekString;
        }

        GameObject[] weekTextObjects = GameObject.FindGameObjectsWithTag("Current Week");
        foreach (GameObject obj in weekTextObjects)
        {
            Text uiText = obj.GetComponent<Text>();
            if (uiText != null)
            {
                uiText.text = weekString;
            }
        }
    }

    // Updates the inspector-assigned field and any active objects tagged with "Character Picked"
    public void UpdateChosenCharacterText()
    {
        if (GameManager.Instance == null) return;

        string characterName = GameManager.Instance.chosenCharacter;
        Debug.Log("LevelManager updating character text to: " + characterName);

        // Update the inspector reference directly (works even if it starts inactive)
        if (chosenCharacterText != null)
        {
            chosenCharacterText.text = characterName;
        }

        GameObject[] characterTextObjects = GameObject.FindGameObjectsWithTag("Character Picked");
        foreach (GameObject obj in characterTextObjects)
        {
            Text uiText = obj.GetComponent<Text>();
            if (uiText != null)
            {
                uiText.text = "Contestant Picked: " + characterName;
            }
        }
    }
}