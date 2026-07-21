using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SuspicionManager : MonoBehaviour
{
    [Header("UI")]
    public Slider suspicionSlider;

    [Header("Settings")]
    public int tickDownAmount = 10;

    // Updates the slider to current suspicion on scene load
    void Start()
    {
        suspicionSlider.maxValue = 100;
        UpdateSlider();
    }

    // Ticks suspicion down by 10 each round
    public void TickDown()
    {
        GameManager.Instance.currentSuspicion -= tickDownAmount;

        // Clamp so it never goes below 0
        if (GameManager.Instance.currentSuspicion < 0)
            GameManager.Instance.currentSuspicion = 0;

        UpdateSlider();
    }

    // Checks if suspicion has hit 100
    public bool IsExposed()
    {
        return GameManager.Instance.currentSuspicion >= 150;
    }

    // Updates the slider visual to match current suspicion
    void UpdateSlider()
    {
        suspicionSlider.value = GameManager.Instance.currentSuspicion;
    }
}