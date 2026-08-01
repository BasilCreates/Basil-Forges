using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardRayCastClick : MonoBehaviour
{
    [Header("Managers")]
    public CharacterSelectionManager characterManager;
    public CardSelectionManager cardManager;
    public TargetSelectionManager targetManager;

    [Header("Camera Raycast Parameters")]
    public float lookDistance = 5f;
    public Camera playerCamera;

    public void Awake()
    {
        characterManager = FindObjectOfType<CharacterSelectionManager>();
        cardManager = FindObjectOfType<CardSelectionManager>();
        targetManager = FindObjectOfType<TargetSelectionManager>();
    }

    public void Update()
    {
        RaycastInteract();
    }

    public void RaycastInteract()
    {
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        Debug.DrawRay(ray.origin, ray.direction * lookDistance, Color.red);

        bool isLookingAtCharacter = false;
        bool isLookingAtCard = false;
        bool isLookingAtTarget = false;

        if (Physics.Raycast(ray, out RaycastHit hit, lookDistance))
        {
            //Check if we hit a Card
            if (hit.collider.GetComponent<Card>() != null)
            {
                isLookingAtCard = true;

                if (cardManager != null)
                {
                    cardManager.HoverCard(hit.collider.tag);

                    if (Input.GetMouseButtonDown(0))
                    {
                        cardManager.OnSelectClicked(hit.collider.tag);
                    }
                }
            }
            // Check if we hit a CardCloseUp
            else if (hit.collider.GetComponent<CardCloseUp>() != null)
            {
                isLookingAtCard = true;

                if (Input.GetMouseButtonDown(0))
                {
                    hit.collider.GetComponent<CardCloseUp>().ClickCard();
                }
            }
            // Check if we hit a 3D Character
            else if (hit.collider.GetComponent<CurrentStats>() != null)
            {
                bool isTargetSelectionActive = targetManager != null && targetManager.targetSelectionPanel != null && targetManager.targetSelectionPanel.activeSelf;

                if (isTargetSelectionActive)
                {
                    isLookingAtTarget = true;
                    targetManager.HoverTarget(hit.collider.tag);

                    if (Input.GetMouseButtonDown(0))
                    {
                        targetManager.OnConfirmTarget(hit.collider.tag);
                    }
                }
                else if (characterManager != null && characterManager.enabled)
                {
                    isLookingAtCharacter = true;
                    characterManager.HoverCharacter(hit.collider.tag);

                    if (Input.GetMouseButtonDown(0))
                    {
                        characterManager.OnSelectClicked(hit.collider.tag);
                    }
                }
            }
            // Check if we hit the TV to turn it on
            else if (hit.collider.CompareTag("TV"))
            {
                if (Input.GetMouseButtonDown(0))
                {
                    hit.collider.GetComponent<TVInteraction>().TurnOnTV();
                }
            }
            // Check for the Start Button tag and trigger the fade
            else if (hit.collider.CompareTag("Start Button"))
            {
                if (Input.GetMouseButtonDown(0))
                {
                    Debug.Log("Hit the Start Button via simple raycast!");
                    TVInteraction tv = FindObjectOfType<TVInteraction>();
                    if (tv != null)
                    {
                        tv.tvOn = true; // Lock it
                        tv.StartCoroutine("FadeIn"); // Start the level load directly
                    }
                }
            }
        }

        // Clear Hovers
        if (!isLookingAtCharacter && characterManager != null) characterManager.ClearHover();
        if (!isLookingAtCard && cardManager != null) cardManager.ClearHover();
        if (!isLookingAtTarget && targetManager != null) targetManager.ClearHover();
    }
}