using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Getting our song data and determining the song time
[System.Serializable]
public class SongData
{
    public AudioClip clip;

    [Header("Seconds into the song to begin playback.")]
    public float startTime;
}

//creating our list for the songs
[System.Serializable]
public class ContestantPlaylist
{
    public string contestantName;

    public SongData[] songs = new SongData[5];

    [HideInInspector]
    public SongData[] shuffledSongs;
}
public class GameManager : MonoBehaviour
{
    // Single instance that persists through all scenes, it wont be destryoed 
    public static GameManager Instance;

    [Header("Round Info")]
    public int currentRound = 1;
    public int maxRounds = 5;

    [Header("Character Info")]
    public string chosenCharacter = "";
    public List<string> eliminatedCharacters = new List<string>();

    [Header("Card Info")]
    public string selectedCard = "";

    [Header("Payout Info")]
    public float winnerPayout = 0f;
    public float highestPossiblePayout = 0f;

    [Header("Suspicion")]
    public int currentSuspicion = 0;
    public bool suspicionLoss = false;
    public string targetedCharacter = "";

    [Header("Vote Results")]
    public Dictionary<string, int> voteResults = new Dictionary<string, int>();

    [Header("Payout Results")]
    public Dictionary<string, float> payoutResults = new Dictionary<string, float>();

    [Header("Contestant Music")]
    public List<ContestantPlaylist> playlists = new List<ContestantPlaylist>();

    // Makes sure only one GameManager exists and persists between scenes
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            ShuffleAllPlaylists();
        }
        else
        {
            Destroy(gameObject);
        }

        Debug.Log("Playlist Count: " + playlists.Count);

        foreach (ContestantPlaylist playlist in playlists)
        {
            Debug.Log("Playlist Loaded: " + playlist.contestantName);
        }
    }

    // Checks if a character has already been eliminated this game
    public bool IsEliminated(string characterTag)
    {
        return eliminatedCharacters.Contains(characterTag);
    }


    // Adds a character to the eliminated list and checks if its the chosen one
    public void EliminateCharacter(string characterTag)
    {
        eliminatedCharacters.Add(characterTag);
    }


    // Returns true if the player's chosen character got knocked out
    public bool IsChosenCharacterEliminated()
    {
        return eliminatedCharacters.Contains(chosenCharacter);
    }

    // Moves the game to the next round
    public void AdvanceRound()
    {
        currentRound++;
    }

    // Returns true if we have reached the final round
    public bool IsGameOver()
    {
        return currentRound > maxRounds;
    }


    // Resets everything back to default for a fresh run
    public void ResetGame()
    {
        currentRound = 1;
        chosenCharacter = "";
        selectedCard = "";
        eliminatedCharacters.Clear();
        winnerPayout = 0f;
        highestPossiblePayout = 0f;
        currentSuspicion = 0;
        voteResults.Clear();
        payoutResults.Clear();
        suspicionLoss = false;
        targetedCharacter = "";
    }

    // out the estimated prize money for a given character
    // ---------------------------------------------------------------
    public float CalculatePayout(string characterTag)
    {
        GameObject character = GameObject.FindWithTag(characterTag);
        CurrentStats stats = character.GetComponent<CurrentStats>();

        float tvPotential = 0f;
        float toxicity = 0f;

        foreach (DefaultStats stat in stats.defaultStats)
        {
            if (stat.statName == "TV Potential") tvPotential = stat.statAmount;
            if (stat.statName == "Toxicity") toxicity = stat.statAmount;
        }

        return (tvPotential * (1f - (toxicity / 100f))) * 50000f;
    }

    //shuffles the playlists to not hear the same song order
    public void ShuffleAllPlaylists()
    {
        foreach (ContestantPlaylist playlist in playlists)
        {
            playlist.shuffledSongs = (SongData[])playlist.songs.Clone();

            for (int i = playlist.shuffledSongs.Length - 1; i > 0; i--)
            {
                int random = Random.Range(0, i + 1);

                SongData temp = playlist.shuffledSongs[i];
                playlist.shuffledSongs[i] = playlist.shuffledSongs[random];
                playlist.shuffledSongs[random] = temp;
            }
        }
    }

    //links the song to the contestant name
    public SongData GetSong(string contestantName)
    {
        Debug.Log("Looking for playlist: " + contestantName);

        ContestantPlaylist playlist =
            playlists.Find(p => p.contestantName == contestantName);

        if (playlist == null)
        {
            Debug.Log("FAILED: No playlist found for " + contestantName);
            return null;
        }

        Debug.Log("Found playlist: " + playlist.contestantName);

        if (playlist.shuffledSongs == null)
        {
            Debug.Log("FAILED: shuffledSongs is null for " + contestantName);
            return null;
        }

        Debug.Log("Song count: " + playlist.shuffledSongs.Length);

        int roundIndex = Mathf.Clamp(currentRound - 1, 0,
            playlist.shuffledSongs.Length - 1);

        Debug.Log("Returning song index: " + roundIndex);

        return playlist.shuffledSongs[roundIndex];
    }
}