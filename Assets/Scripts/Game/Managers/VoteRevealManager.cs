using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class VoteRevealManager : MonoBehaviour
{
    [Header("Contestant Sliders")]
    public Slider amaraSlider;
    public Slider zarahSlider;
    public Slider priyaSlider;
    public Slider tariqSlider;
    public Slider idrisSlider;

    [Header("Contestant Vote Text")]
    public Text amaraText;
    public Text zarahText;
    public Text priyaText;
    public Text tariqText;
    public Text idrisText;

    [Header("Contestant Panels")]
    public GameObject amaraPanel;
    public GameObject zarahPanel;
    public GameObject priyaPanel;
    public GameObject tariqPanel;
    public GameObject idrisPanel;

    [Header("Result Panels")]
    public GameObject youPassPanel;
    //public GameObject youLostPanel;
    public GameObject youWonPanel;
    public GameObject youAlmostWonPanel;

    [Header("Win Text")]
    public Text winnerEarningsText;

    [Header("Almost Won Text")]
    public Text almostWonEarningsText;
    public Text bestContestantNameText;
    public Text bestContestantEarningsText;

    [Header("Scene Settings")]
    public string nextScene = "ProducerOffice";
    public float barFillSpeed = 2f;

    [Header("Continue Button")]
    public Button continueButton;

    [Header("Sounds UI")]
    public AudioSource voteBarSound;
    public AudioSource buttonSound1;

    private string eliminatedTag = "";

    // Hides continue button and starts the vote reveal sequence
    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (GameManager.Instance.IsEliminated("Amara"))
        {
            amaraPanel.SetActive(false);
        }
        if (GameManager.Instance.IsEliminated("Zarah"))
        {
            zarahPanel.SetActive(false);
        }
        if (GameManager.Instance.IsEliminated("Priya"))
        {
            priyaPanel.SetActive(false);
        }

        if (GameManager.Instance.IsEliminated("Tariq"))
        {
            tariqPanel.SetActive(false);
        }
        if (GameManager.Instance.IsEliminated("Idris"))
        {
            idrisPanel.SetActive(false);
        }

        if (GameManager.Instance.suspicionLoss)
        {
            GameManager.Instance.AdvanceRound();

            if (GameManager.Instance.IsGameOver())
            {
                // ... (Keep your win/almost win logic here) ...
                return;
            }

            // Always load the montage scene for round eliminations!
            SceneManager.LoadScene(nextScene);
            buttonSound1.Play();
        }

        continueButton.gameObject.SetActive(false);
        StartCoroutine(RevealVotes());
    }

    // Fills each contestant's bar one by one then highlights loser
    IEnumerator RevealVotes()
    {
        // Find the lowest scorer first
        int lowestVote = int.MaxValue;
        foreach (KeyValuePair<string, int> result in GameManager.Instance.voteResults)
        {
            if (result.Value < lowestVote)
            {
                lowestVote = result.Value;
                eliminatedTag = result.Key;
            }
        }

        // Fill each bar one by one
        foreach (KeyValuePair<string, int> result in GameManager.Instance.voteResults)
        {
            Slider slider = GetSlider(result.Key);
            Text voteText = GetText(result.Key);

            if (slider == null || voteText == null) continue;

            Image fill = slider.fillRect.GetComponent<Image>();

            // Animate the bar filling up
            float elapsed = 0f;
            float targetValue = result.Value;
            slider.maxValue = 300;

            while (elapsed < barFillSpeed)
            {
                elapsed += Time.deltaTime;
                slider.value = Mathf.Lerp(0, targetValue, elapsed / barFillSpeed);
                voteBarSound.Play();
                voteText.text = Mathf.RoundToInt(slider.value).ToString();
                yield return null;
            }

            slider.value = targetValue;
            voteText.text = targetValue.ToString();

            // Highlight loser red
            if (result.Key == eliminatedTag)
            {
                fill.color = Color.red;
                voteText.color = Color.red;
            }

            // Small pause between each contestant
            yield return new WaitForSeconds(0.5f);
        }

        // Show continue button after all bars filled
        continueButton.gameObject.SetActive(true);
    }

    // Eliminates the loser and checks if its the chosen character
    // Eliminates the loser and checks if its the chosen character
    public void OnContinueClicked()
    {
        buttonSound1.Play();
        GameManager.Instance.EliminateCharacter(eliminatedTag);

        // Save the targeted character name so the Elimination Montage script knows who to spawn!
        GameManager.Instance.targetedCharacter = eliminatedTag;

        GameManager.Instance.AdvanceRound();

        // Game is over - check payout for win type
        if (GameManager.Instance.IsGameOver())
        {
            GameManager.Instance.winnerPayout =
                GameManager.Instance.payoutResults[GameManager.Instance.chosenCharacter];

            if (GameManager.Instance.winnerPayout >= GameManager.Instance.highestPossiblePayout)
            {
                // True win
                SceneManager.LoadScene("Win Montage");
            }
            else
            {
                // Almost won — show chosen character earnings and best contestant
                almostWonEarningsText.text = "$" + GameManager.Instance.winnerPayout.ToString("N0");

                // Find who had the highest payout
                string bestContestant = "";
                float highest = 0f;
                foreach (KeyValuePair<string, float> payout in GameManager.Instance.payoutResults)
                {
                    if (payout.Value > highest)
                    {
                        highest = payout.Value;
                        bestContestant = payout.Key;
                    }
                }

                bestContestantNameText.text = bestContestant;
                bestContestantEarningsText.text = "$" + highest.ToString("N0");
                youAlmostWonPanel.SetActive(true);
            }
            return;
        }

        // Round continues - load the Elimination Montage scene
        SceneManager.LoadScene(nextScene);
    }

    // Go to producer office for next round
    public void OnPassContinueClicked()
    {
        buttonSound1.Play();
        SceneManager.LoadScene(nextScene);
    }

    // Go to main menu after losing or winning
    public void OnEndContinueClicked()
    {
        buttonSound1.Play();
        GameManager.Instance.ResetGame();
        SceneManager.LoadScene("Game Menu");
    }

    // Returns the correct slider for each character tag
    Slider GetSlider(string tag)
    {
        switch (tag)
        {
            case "Amara": return amaraSlider;
            case "Zarah": return zarahSlider;
            case "Priya": return priyaSlider;
            case "Tariq": return tariqSlider;
            case "Idris": return idrisSlider;
            default: return null;
        }
    }

    // Returns the correct text for each character tag
    Text GetText(string tag)
    {
        switch (tag)
        {
            case "Amara": return amaraText;
            case "Zarah": return zarahText;
            case "Priya": return priyaText;
            case "Tariq": return tariqText;
            case "Idris": return idrisText;
            default: return null;
        }
    }
}