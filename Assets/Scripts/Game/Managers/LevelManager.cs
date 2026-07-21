using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    public GameObject week1Panel;
    public GameObject week2to5Panel;
    public Text weekTitleText;
    public List<string> roundCaptions;
    public Text captionText;
    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (GameManager.Instance.currentRound == 1)
        {
            week1Panel.SetActive(true);
            week2to5Panel.SetActive(false);
            Debug.Log("Loaded week 1");
        }
        else
        {
            week1Panel.SetActive(false);
            week2to5Panel.SetActive(true);
            Debug.Log("Loaded the next week!");
        }

        if (weekTitleText != null)
        {
            weekTitleText.text = "Week " + GameManager.Instance.currentRound;
        }
        TextUpdate();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TextUpdate()
    {
        if (captionText != null && GameManager.Instance.currentRound <= roundCaptions.Count)
        {
            captionText.text = roundCaptions[GameManager.Instance.currentRound - 1];
        }
    }
}
