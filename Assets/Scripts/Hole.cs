using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hole : MonoBehaviour, Interactable
{
    private enum STATE
    {
        EMPTY, HOLE, PLANTED, HARVEST
    }

    private STATE holeState = STATE.EMPTY;

    /* Player interacts with object. Function in charge of playing the animation */
    public void Interact(GameObject player)
    {
        AnimationController playerAnimationController = player.GetComponent<AnimationController>();
        PlayerState playerState = player.GetComponent<PlayerState>();
        HighlightController highlightController = gameObject.GetComponent<HighlightController>();
        Animator playerAnimator = player.GetComponent<Animator>();

        switch (holeState)
        {
            case STATE.EMPTY:
                // Set state
                playerState.setInteracting();
                print("Yo, we hit something");

                playerAnimator.Play("Player_Shovel");
                break;
            default:
                // No animation playing so we just call function
                playerAnimationController.onAnimationEnd();
                break;
/*            case STATE.HOLE:
                // Plant here
                break;
            case STATE.PLANTED:
                // Water here
                break;
            case STATE.HARVEST:
                // Harvest here
                break;*/
        }

        Debug.Log("Interacted with hole!");
    }

    /* Will be called at some part of animation we want to update the game 
     * Handle any hole related changes in this function!
     */
    public void onAnimationEvent(GameObject player, InteractableAction action)
    {
        AnimationController playerAnimationController = player.GetComponent<AnimationController>();
        PlayerState playerState = player.GetComponent<PlayerState>();
        HighlightController highlightController = gameObject.GetComponent<HighlightController>();

        switch (action)
        {
            case InteractableAction.DIG:
                SpriteRenderer holeRenderer = gameObject.GetComponent<SpriteRenderer>();
                holeRenderer.color = new Color(255, 255, 255, 255);
                holeState = STATE.HOLE;
                break;
            default:
                // No animation playing so we just call function
                playerAnimationController.onAnimationEnd();
                break;
                /* 
                case STATE.HOLE:
                    // Plant here
                    break;
                case STATE.PLANTED:
                    // Water here
                    break;
                case STATE.HARVEST:
                    // Harvest here
                    break;*/
        }
        Debug.Log("on Finish Interact");
    }

    /* Will be called at end of animation */
    public void onAnimationEnd()
    {
        Debug.Log("onAnimationEnd() for Hole class");
    }
}

public enum InteractableAction
{
    NONE, DIG
};

public interface Interactable
{ 

    // void betternameforhighlighting();
    void Interact(GameObject player);
    void onAnimationEvent(GameObject player, InteractableAction action); // Will be called at some part of animation we want to update the game
    void onAnimationEnd(); // Will be called at end of animation
}

