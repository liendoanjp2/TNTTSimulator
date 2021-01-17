using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hole : MonoBehaviour, Interactable
{
    public GameObject plant;
    public GameObject sacraficeExp;

    private enum STATE
    {
        EMPTY, HOLE, PLANTED, WATERED
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
                playerState.setInteracting();
                playerAnimator.Play("Player_Shovel");
                break;
            case STATE.HOLE:
                playerState.setInteracting();
                playerAnimator.Play("Player_Doing");
                holeState = STATE.PLANTED;
                break;
            case STATE.PLANTED:
                playerState.setInteracting();
                playerAnimator.Play("Player_Water");
                holeState = STATE.WATERED;
                break;
            default:
                // No animation playing so we just call function
                playerAnimationController.onAnimationEnd();
                break;
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
            case InteractableAction.PLANT:
                //Don't do anything right now. We have no UI for planted
                holeState = STATE.PLANTED;
                break;
            case InteractableAction.WATER:
                SpriteRenderer plantRenderer = plant.GetComponent<SpriteRenderer>();
                plantRenderer.color = new Color(255, 255, 255, 255);
                gameObject.GetComponent<BoxCollider2D>().enabled = false;
                holeState = STATE.WATERED;

                int numberOfMiniGameHoles = 5;
                float amountOfExpPerHole = sacraficeExp.GetComponent<ExpBar>().getMaxExp() / numberOfMiniGameHoles;
                sacraficeExp.GetComponent<ExpBar>().addExp(amountOfExpPerHole);
                break;
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
    NONE, DIG, PLANT, WATER
};

public interface Interactable
{ 

    // void betternameforhighlighting();
    void Interact(GameObject player);
    void onAnimationEvent(GameObject player, InteractableAction action); // Will be called at some part of animation we want to update the game
    void onAnimationEnd(); // Will be called at end of animation
}

