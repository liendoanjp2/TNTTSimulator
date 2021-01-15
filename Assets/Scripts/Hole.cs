using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hole : MonoBehaviour, Interactable
{
    private enum STATE
    {
        EMPTY, HOLE, PLANTED, WATER
    }

    private STATE holeState = STATE.EMPTY; 

    public void Interact(PlayerMovement player)
    {
        GameObject playerGameObject = player.gameObject;
        Animator playerAnimator = playerGameObject.GetComponent<Animator>();

        switch (holeState)
        { 
            case STATE.EMPTY:
                // Set state
                player.setPlayerState(PlayerMovement.STATE.INTERACTING);
                playerAnimator.Play("Player_Shovel");
                holeState = STATE.HOLE;
                break;
            case STATE.HOLE:
                player.setPlayerState(PlayerMovement.STATE.INTERACTING);
                playerAnimator.Play("Player_Doing");
                holeState = STATE.PLANTED;
                break;
            case STATE.PLANTED:
                player.setPlayerState(PlayerMovement.STATE.INTERACTING);
                playerAnimator.Play("Player_Water");
                holeState = STATE.WATER;
                break;
            default:
                // No animation playing so we just call function
                player.onAnimationFinish();
                break;
        }

        Debug.Log("Interacted with hole!");
    }

    public void onFinishInteract()
    {
        Debug.Log("on Finish INteract");
        
    }
}


public interface Interactable
{
    // void betternameforhighlighting();
    void Interact(PlayerMovement player);
    void onFinishInteract();
}