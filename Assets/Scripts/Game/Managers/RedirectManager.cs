using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class RedirectManager : MonoBehaviour
{
    [Header("Newspapers")]
    public List<GameObject> newspapers;

    [Header("Contestant Faces")]
    public List<GameObject> contestantFaces; // Named exactly after character tags

    [Header("Scene Settings")]
    public string montageScene = "Contestant Stage";

    [Header("Sounds UI")]
    public AudioSource buttonSound;


    public void Awake()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
    // Picks a random newspaper, updates name and shows correct face
    void Start()
    {
        // Hide all newspapers first
        foreach (GameObject newspaper in newspapers)
        {
            newspaper.SetActive(false);
        }

        // Hide all faces first
        foreach (GameObject face in contestantFaces)
        {
            face.SetActive(false);
        }

        // Pick a random newspaper
        int randomIndex = Random.Range(0, newspapers.Count);
        newspapers[randomIndex].SetActive(true);

        // Update the name text inside the active newspaper
        Text nameText = newspapers[randomIndex].GetComponentInChildren<Text>();
        if (nameText != null)
        {
            nameText.text = GameManager.Instance.targetedCharacter;
            Debug.Log("Name Exposed: " + GameManager.Instance.targetedCharacter);
        }

        // Activate the correct contestant face
        foreach (GameObject face in contestantFaces)
        {
            if (face.tag == GameManager.Instance.targetedCharacter)
            {
                face.SetActive(true);
                break;
            }
        }
    }

    // Continue button loads the montage
    public void OnContinueClicked()
    {
        buttonSound.Play();
        SceneManager.LoadScene(montageScene);
    }
}