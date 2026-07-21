using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TVTrigger : MonoBehaviour
{
    [Header("Scene Settings")]
    public string levelChoosingScene = "All Level Choosing";

    // When the player walks into the TV collider load the level choosing scene
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Character Walked in!");
            SceneManager.LoadScene(levelChoosingScene);
        }
    }
}