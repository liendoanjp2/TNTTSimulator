﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : MonoBehaviour
{
    private PlayerMovement playerMovement;
    private Animator playerAnimator;
    private HighlightController highlightController;

    void Start()
    {
        playerMovement = gameObject.GetComponent<PlayerMovement>();
        if (playerMovement == null)
            throw new System.Exception("playerMovement null");
        playerAnimator = gameObject.GetComponent<Animator>();
        if (playerAnimator == null)
            throw new System.Exception("playerAnimator null");
        highlightController = gameObject.GetComponent<HighlightController>();
        if (highlightController == null)
            throw new System.Exception("highlightController null");
    }

    // Update is called once per frame
    void Update()
    {
        ProcessAnimations();
    }

    void ProcessAnimations()
    {
        playerAnimator.SetFloat("Horizontal", playerMovement.movement.x);
        playerAnimator.SetFloat("Vertical", playerMovement.movement.y);
        playerAnimator.SetFloat("Speed", playerMovement.movement.sqrMagnitude);
        playerAnimator.SetFloat("Direction", playerMovement.directionFacing);
        playerAnimator.SetBool("isRunning", playerMovement.currMoveSpeed > playerMovement.runningMoveSpeed);
    }

    public void playRoll()
    {
        playerAnimator.Play("Player_Roll");
    }

    public void onAnimationEnd()
    {
        highlightController.onAnimationComplete();
        Debug.Log("Done");
    }

    public void onAnimationEvent(AnimationEvent myEvent)
    {
        string interactableActionString = myEvent.stringParameter;
        InteractableAction interactableAction = InteractableAction.NONE;

        foreach (InteractableAction action in Enum.GetValues(typeof(InteractableAction)))
        {
            if (action.ToString().Equals(interactableActionString))
            {
                interactableAction = action;
                break;
            }
        }

        highlightController.onInteractEnd(interactableAction);
    }
}
