using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighlightController : MonoBehaviour
{
    public GameObject highlighter;
    GameObject currentTarget;
    Rigidbody2D playerRigidbody;
    Transform playerTransform;

    private void Start()
    {
        playerRigidbody = gameObject.GetComponent<Rigidbody2D>();
        playerTransform = gameObject.GetComponent<Transform>();
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
    }

    private void ProcessPointer()
    {
        Collider2D farthestCollider = getFarthestCollider();
        if (farthestCollider)
        {
            Interactable hit = farthestCollider.GetComponent<Interactable>();
            if (hit != null)
            {
                Highlight(farthestCollider.gameObject);
                return;
            }
        }
        Hide();
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

}
