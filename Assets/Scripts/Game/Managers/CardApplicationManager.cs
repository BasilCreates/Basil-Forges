using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CardApplicationManager : MonoBehaviour
{
    [Header("Scene References")]
    public string montageSceneName = "Contestant Stage";

    [Header("Card Data")]
    public Card selectedCardData; // Stored before panel hides

    [Header("Contestant Tags")]
    public List<string> contestantTags = new List<string>
    { "Amara", "Zara", "Priya", "Tariq", "Idris" };

    // Applies modifiers, calculates votes then starts montage, Called when player confirms their card choice
    public void ConfirmCard()
    {
        if (selectedCardData == null)
        {
            Debug.LogWarning("No card selected yet");
            return;
        }

        Card card = selectedCardData;

        // Apply card to correct targets
        ApplyCardToTargets(card);

        // Raise suspicion
        GameManager.Instance.currentSuspicion += card.suspicionAmount;
        Debug.Log("Suspicion is now: " + GameManager.Instance.currentSuspicion);

        // Check if exposed
        if (GameManager.Instance.currentSuspicion >= 150)
        {
            GameManager.Instance.suspicionLoss = true;
            SceneManager.LoadScene("Vote Reveal");
            return;
        }

        // Calculate votes in background
        CalculateAllVotes();

        // Go to correct scene based on card
        switch (GameManager.Instance.selectedCard)
        {
            case "Redirect":
                SceneManager.LoadScene("Redirect");
                break;
            case "Vote Bot":
                SceneManager.LoadScene("Vote Bot Mission");
                break;
            case "Sympathy":
                SceneManager.LoadScene("Sympathy Arc");
                break;
            case "Coaching":
                SceneManager.LoadScene("Coaching");
                break;
            default:
                // Bad Edit and Shadow Cut go straight to montage
                SceneManager.LoadScene(montageSceneName);
                break;
        }
    }

    // Checks if card helps chosen character or hurts targeted character
    void ApplyCardToTargets(Card card)
    {
        if (card.targetsChosenCharacter)
        {
            // Help card — apply to chosen character only
            ApplyModifierToCharacter(GameManager.Instance.chosenCharacter, card);
        }
        else
        {
            // Harm card — apply to targeted character only
            ApplyModifierToCharacter(GameManager.Instance.targetedCharacter, card);
        }
    }

    // Finds the character by tag, adds modifier and applies it
    void ApplyModifierToCharacter(string tag, Card card)
    {
        GameObject character = GameObject.FindWithTag(tag);
        if (character == null) return;

        StatsModifier statsModifier = character.GetComponent<StatsModifier>();
        CurrentStats currentStats = character.GetComponent<CurrentStats>();

        // Add the modifier to their list
        statsModifier.ModifyList.Add(new StatsModify
        {
            statTarget = card.targetStat,
            modifyAmount = card.modifyAmount,
            modifyOperator = card.modifyOperator
        });

        // Apply it directly to their CurrentStats
        statsModifier.ApplyModifiers();
    }

    // Stores results in GameManager for the reveal screen
    void CalculateAllVotes()
    {
        GameManager.Instance.voteResults.Clear();

        foreach (string tag in contestantTags)
        {
            if (GameManager.Instance.IsEliminated(tag)) continue;

            GameObject character = GameObject.FindWithTag(tag);
            if (character == null) continue;

            CurrentStats stats = character.GetComponent<CurrentStats>();

            int audience = 0;
            int talent = 0;
            int toxicity = 0;

            foreach (DefaultStats stat in stats.defaultStats)
            {
                if (stat.statName == "Audience Appeal") audience = stat.statAmount;
                if (stat.statName == "Talent") talent = stat.statAmount;
                if (stat.statName == "Toxicity") toxicity = stat.statAmount;
            }

            int baseVote = Random.Range(70, 95);
            int finalVote = baseVote + audience + talent - toxicity;
            GameManager.Instance.voteResults[tag] = finalVote;

            float payout = GameManager.Instance.CalculatePayout(tag);
            GameManager.Instance.payoutResults[tag] = payout;

            Debug.Log(tag + " final vote: " + finalVote + " | payout: " + payout);
        }

        // Store highest possible payout
        GameManager.Instance.highestPossiblePayout = 0f;
        foreach (KeyValuePair<string, float> payout in GameManager.Instance.payoutResults)
        {
            if (payout.Value > GameManager.Instance.highestPossiblePayout)
                GameManager.Instance.highestPossiblePayout = payout.Value;
        }
    }
}