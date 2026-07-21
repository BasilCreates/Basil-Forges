using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlinkText : MonoBehaviour
{
    [Header("Blink Settings")]
    public float blinkInterval = 0.5f; // seconds between on/off
    public bool startVisible = true;

    private Text textComponent;

    void Awake()
    {
        textComponent = GetComponent<Text>();
    }

    void OnEnable()
    {
        StartCoroutine(Blink());
    }

    IEnumerator Blink()
    {
        bool visible = startVisible;
        textComponent.enabled = visible;

        while (true)
        {
            yield return new WaitForSeconds(blinkInterval);
            visible = !visible;
            textComponent.enabled = visible;
        }
    }
}
