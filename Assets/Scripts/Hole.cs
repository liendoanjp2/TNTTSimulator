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

    public void Interact(PlayerMovement player)
    {

        switch (holeState)
        {
            case STATE.EMPTY:
                // Set state
                player.setPlayerState(PlayerMovement.STATE.INTERACTING);

                GameObject playerGameObject = player.gameObject;
                print("Yo, we hit something");

                Animator playerAnimator = playerGameObject.GetComponent<Animator>();
                playerAnimator.Play("Player_Shovel");
                holeState = STATE.HOLE;
                break;
            default:
                // No animation playing so we just call function
                player.onAnimationFinish();
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
    void Interact(PlayerMovement player);
    void onFinishInteract();
}