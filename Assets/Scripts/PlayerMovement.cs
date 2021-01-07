using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public Rigidbody2D rigidbody;
    public Animator animator;
    public Transform transform;

    private const float leftDirection = -1f;
    private const float rightDirection = 1f;

    private Vector2 movement;
    private float directionFacing = rightDirection;
    private List<string> keyPresses = new List<string>();

    private Vector2 velocity = Vector2.zero;
    private float timeSinceLastRolled = 0;
    private const int rollCooldown = 0; // 3 seconds
    // Update is called once per frame
    void Update()
    {

        ProcessInputs();

        ProcessAnimations();

    }

    void FixedUpdate()
    {
        Move();
    }

    void ProcessInputs()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        movement = new Vector2(moveX, moveY).normalized; // Fix diagonal speed

        if (Input.GetKeyDown("q"))
        {
            // Roll
            print("Q down");
            keyPresses.Add("q");
        }
        else if (Input.GetKeyDown("w"))
        {
            // Jump
            print("W down");
            keyPresses.Add("w");
        }
        else if (Input.GetKeyDown("e"))
        {
            // interact
            print("e down");
        }

    }
  
    void ProcessAnimations()
    {
        animator.SetFloat("Horizontal", movement.x);
        animator.SetFloat("Vertical", movement.y);
        animator.SetFloat("Speed", movement.sqrMagnitude);
        animator.SetFloat("Direction", directionFacing);
    }

    void Move()
    {
        if (directionFacing == rightDirection && movement.x < 0)
        {
            // Turn left
            transform.rotation = transform.rotation * Quaternion.Euler(0, 180f, 0);
            directionFacing = leftDirection;
            print("hello");
        }
        else if(directionFacing == leftDirection && movement.x > 0)
        {
            // Turn right
            transform.rotation = transform.rotation * Quaternion.Euler(0, -180f, 0);
            directionFacing = rightDirection;
            print("hello2");
        }

        // Decrease until zero
        // deaccelerate until its zero
        //print(velocity.x);
        velocity.x /= 2;
        velocity.y /= 2;

        // Movement
        rigidbody.MovePosition((rigidbody.position + movement * moveSpeed * Time.fixedDeltaTime) + velocity);
        
        if (keyPresses.Count > 0)
        {
            string keyPress = keyPresses[0];
            keyPresses.RemoveAt(0);
            if (keyPress.Equals("q") && Time.fixedTime - timeSinceLastRolled > rollCooldown)
            {
                if (movement != Vector2.zero)
                {
                    // Take movement get the vector and add force
                    velocity = new Vector2(movement.x, movement.y).normalized;
                    print("MOVING");
                }
                else
                {
                    // Roll in direction you are facing
                    velocity = transform.right;
                    print("MOVING");
                }
                timeSinceLastRolled = Time.fixedTime;
            }
            else if (Input.GetKeyDown("w"))
            {
                // Jump
                print("W down");
            }
            else if (Input.GetKeyDown("e"))
            {
                // interact
                print("e down");
            }
        }



    }
}
