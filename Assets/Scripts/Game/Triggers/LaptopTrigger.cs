using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LaptopTrigger : MonoBehaviour
{
    public AudioSource buttonsound1;
    // When player reaches the laptop call VoteBotManager
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            buttonsound1.Play();
            VoteBotManager voteBotManager = FindObjectOfType<VoteBotManager>();
            voteBotManager.OnLaptopReached();
        }
    }
}