using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighlightController : MonoBehaviour
{
    public GameObject highlighter;
    public GameObject reminderInteract;
    GameObject currentTarget;
    Rigidbody2D playerRigidbody;
    private GameObject GameObjectInteractingWith; // Gameobj Player is currently interacting with
    private PlayerState playerState;

    private void Start()
    {
        playerRigidbody = gameObject.GetComponent<Rigidbody2D>();
        playerState = gameObject.GetComponent<PlayerState>();
        if (playerState == null)
            throw new System.Exception("playerState null");
    }

    private void Update()
    {
        ProcessPointer();
    }

    public void Highlight(GameObject target)
    {
        if (currentTarget != target)
        {
            currentTarget = target;
            Vector3 position = target.transform.position;
            Highlight(position);
        }
    }

    public void Highlight(Vector3 position)
    {
        highlighter.SetActive(true);
        highlighter.transform.position = position;
    }

    public void Hide()
    {
        currentTarget = null;
        highlighter.SetActive(false);
        reminderInteract.SetActive(false);
    }

    private void ProcessPointer()
    {

        Collider2D farthestCollider = getFarthestCollider();
        if (farthestCollider)
        {
            if (farthestCollider.name.Contains("Hole"))
            {
                ToggleReminder(true);
            }

            if (farthestCollider.name.Contains("Sign"))
            {
                ToggleReminder(true);
            }

            if (farthestCollider.name.Contains("NPC"))
            {
                ToggleReminder(true);
            }

            Interactable hit = farthestCollider.GetComponent<Interactable>();
            if (hit != null)
            {
                // hit.HighlightSomethingUIIDK_Philips_come_up_with_a_beter_name(ref to UI)
                Highlight(farthestCollider.gameObject);
                return;
            }
        }
        Hide();
    }

    public void Interact()
    {
        Collider2D farthestCollider = getFarthestCollider();
        if (farthestCollider)
        {
            Interactable hit = farthestCollider.GetComponent<Interactable>();
            if (hit != null)
            {
                Debug.Log("hit " + farthestCollider.gameObject.name);
                GameObjectInteractingWith = farthestCollider.gameObject;
                hit.Interact(gameObject);
                return;
            }
        }
    }

    public void onInteractEnd(InteractableAction interactableAction)
    {
        Interactable interactable = GameObjectInteractingWith.GetComponent<Interactable>();
        interactable.onAnimationEvent(gameObject, interactableAction);
    }

    public void onAnimationComplete()
    {
        Interactable interactable = GameObjectInteractingWith.GetComponent<Interactable>();
        interactable.onAnimationEnd();

        GameObjectInteractingWith = null;
        playerState.setNormal();
    }

    private void ToggleReminder(bool toggleValue)
    {
        reminderInteract.SetActive(toggleValue);
    }

    public GameObject getGameObjectInteractingWith()
    {
        return GameObjectInteractingWith;
    }

    private Collider2D[] getColliders()
    {
        return Physics2D.OverlapPointAll(playerRigidbody.position + transform.right * new Vector2(0.5f, 0f));
    }

    public Collider2D getFarthestCollider()
    {
        Collider2D[] colliders = getColliders();
        Collider2D furthestCollider = null;

        if (colliders.Length >= 1)
        {
            // Now grab the furthest collider
            furthestCollider = getFarthestCollider(colliders);
        }
        return furthestCollider;
    }

    // Determines farthest collider from the player
    private Collider2D getFarthestCollider(Collider2D[] colliders)
    {
        Collider2D playerCollider = gameObject.GetComponent<Collider2D>();

        // We can assume that it has length >= 1
        Collider2D theCollider = colliders[0];
        float maxDistance = playerCollider.Distance(theCollider).distance;
        for (int nonPlayerIndex = 1; nonPlayerIndex < colliders.Length; nonPlayerIndex++)
        {
            float currDistance = playerCollider.Distance(colliders[nonPlayerIndex]).distance;
            if (currDistance > maxDistance)
            {
                maxDistance = currDistance;
                theCollider = colliders[nonPlayerIndex];
            }
        }
        return theCollider;
    }

    public void onShovelAHole()
    {
        SpriteRenderer holeRenderer = GameObjectInteractingWith.GetComponent<SpriteRenderer>();
        holeRenderer.color = new Color(255, 255, 255, 255);
    }

}
