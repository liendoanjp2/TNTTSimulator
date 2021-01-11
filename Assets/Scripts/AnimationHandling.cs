using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationHandling : MonoBehaviour
{
    public void onShovelAnimationUp()
    {
        // Animation frame where the shovel is up in the air so we can create a hole
        GameObject playerInteractableGameObject = gameObject.GetComponent<PlayerMovement>().getPlayerInteractableGameObject();
        SpriteRenderer holeRenderer = playerInteractableGameObject.GetComponent<SpriteRenderer>();
        // Make it invis
        holeRenderer.color = new Color(255, 255, 255, 255);
    }
}
