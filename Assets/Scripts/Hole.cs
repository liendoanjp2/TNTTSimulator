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

    public void Interact(GameObject player)
    {
        AnimationController playerAnimationController = player.GetComponent<AnimationController>();
        PlayerState playerState = player.GetComponent<PlayerState>();

        switch (holeState)
        {
            case STATE.EMPTY:
                // Set state
                playerState.setInteracting();
                print("Yo, we hit something");

                Animator playerAnimator = player.GetComponent<Animator>();
                playerAnimator.Play("Player_Shovel");
                holeState = STATE.HOLE;
                break;
            default:
                // No animation playing so we just call function
                playerAnimationController.onAnimationFinish();
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

    public void onFinishInteract()
    {
        Debug.Log("on Finish INteract");
        
    }
}


public interface Interactable
{
    // void betternameforhighlighting();
    void Interact(GameObject player);
    void onFinishInteract();
}