using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationSwitch : MonoBehaviour
{
    public Animator animator;

    public string animation1 = "Amara Singing";
    public string animation2 = "Amara Crying";

    private bool nextAmimation = false;

    // Update is called once per frame
    void Update()
    {
        // Detect when the "P" key is pressed down
        if (Input.GetKeyDown(KeyCode.P))
        {
            ToggleAnimation();
        }
    }

    public void ToggleAnimation()
    {
        // Safety check to make sure you assigned the animator in the inspector
        if (animator == null)
        {
            Debug.LogWarning("Please assign the Animator component in the Inspector!");
            return;
        }

        if (nextAmimation)
        {
            // Play the first animation and blend smoothly over 0.25 seconds
            animator.CrossFadeInFixedTime(animation1, 0.25f);
        }
        else
        {
            // Play the second animation
            animator.CrossFadeInFixedTime(animation2, 0.25f);
        }

        // Flip the boolean switch so it toggles back next time you press P
        nextAmimation = !nextAmimation;
    }
}