using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneTester : MonoBehaviour
{
    void Awake()
    {
        // If GameManager doesn't exist yet (because you loaded this scene directly), create a dummy one for testing
        if (GameManager.Instance == null)
        {
            GameObject gm = new GameObject("GameManager_Dummy");
            GameManager gameManagerComponent = gm.AddComponent<GameManager>();

            // Populate dummy values so WinManager doesn't null-reference or show $0
            gameManagerComponent.chosenCharacter = "Amara"; // Change to one of your character names/tags
            gameManagerComponent.winnerPayout = 50000f;   // Test payout amount
        }
    }
}