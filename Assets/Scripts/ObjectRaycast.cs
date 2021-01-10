using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectRaycast : MonoBehaviour
{
    // Start is called before the first frame update
    public void OnTriggerEnter2D(Collider2D collider)
    {
        print("Collider: " + collider.gameObject.name);
        //Popup Menu
    }


    public void doShovelingAction(GameObject playerGameObject)
    {
        

        Transform playerTransform = playerGameObject.transform;
        Vector3 playerPosition = playerTransform.position;
        float tileDistance = 2.0f;

        //Raycast up
        //var raycastUp = Physics2D.Raycast(playerPosition, new Vector2(0,-1));
        //Raycast down
        //var raycastDown = Physics2D.Raycast(playerPosition, new Vector2(0, 1));

        //Raycast right
        RaycastHit2D[] raycastAllRight = Physics2D.RaycastAll(playerPosition, playerTransform.right, tileDistance);
        Debug.DrawRay(playerPosition, new Vector2(1, 0), Color.red);

        RaycastHit2D rightHit = getFarthestRaycastObject(raycastAllRight);

        if (rightHit != new RaycastHit2D())
        {
            print("Yo, we hit something" + rightHit);
            // Do Animation
            // After animation show holeee
            Animator playerAnimator = playerGameObject.GetComponent<Animator>();
            playerAnimator.Play("Player_Shovel");
            rightHit.transform.gameObject.GetComponent<SpriteRenderer>().color = new Color(255, 255, 255, 255);
        }

        //Raycast left
        RaycastHit2D[] raycastAllLeft = Physics2D.RaycastAll(playerPosition, new Vector2(-1, 0), tileDistance);
        Debug.DrawRay(playerPosition, new Vector2(-1, 0), Color.red);

    }

    public RaycastHit2D getFarthestRaycastObject(RaycastHit2D[] arrayOfRaycastHit2D)
    {

        RaycastHit2D thetile = new RaycastHit2D();
        float currentMaxDistance = 0.0f;
        //Find what we hit
        foreach (RaycastHit2D hitbox in arrayOfRaycastHit2D)
        {
            if(currentMaxDistance < hitbox.distance)
            {
                currentMaxDistance = hitbox.distance;
                thetile = hitbox;
            }
        }

        return thetile;
    }

}
