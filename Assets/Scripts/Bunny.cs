using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bunny : MonoBehaviour, Interactable
{

    public float speed;
    private float waitTime;
    public float startWaitTime;
    private bool facingRight = true;

    public Transform[] moveSpots;
    private int randomSpot;

    // Start is called before the first frame update
    void Start()
    {
        randomSpot = Random.Range(0, moveSpots.Length -1);

    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector2.MoveTowards(transform.position, moveSpots[randomSpot].position, speed * Time.deltaTime);

        if (Vector2.Distance(transform.position,  moveSpots[randomSpot].position) < 0.2f)
        {
            if(waitTime <= 0)
            {
                gameObject.GetComponent<Animator>().Play("BunnyAnimation");
                randomSpot = Random.Range(0, moveSpots.Length - 1);

                waitTime = startWaitTime;
                if (transform.position.x > moveSpots[randomSpot].position.x)
                {
                    if(facingRight == true)
                    {
                        facingRight = false;
                        transform.rotation = transform.rotation * Quaternion.Euler(0, -180f, 0);
                    }

                }
                else
                {
                    if (facingRight == false)
                    {
                        facingRight = true;
                        transform.rotation = transform.rotation * Quaternion.Euler(0, 180f, 0);
                    }
                }

            }
            else
            {
                waitTime -= Time.deltaTime;
                gameObject.GetComponent<Animator>().Play("BunnyIdleAnimation");
            }
        }
    }

    public void Interact(GameObject player)
    {
        throw new System.NotImplementedException();
    }

    public void onAnimationEvent(GameObject player, InteractableAction action)
    {
        throw new System.NotImplementedException();
    }

    public void onAnimationEnd()
    {
        throw new System.NotImplementedException();
    }
}
