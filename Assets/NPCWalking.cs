using UnityEngine;


public class NPCWalking : MonoBehaviour
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
        randomSpot = Random.Range(0, moveSpots.Length);
        gameObject.GetComponent<Animator>().Play("NPC_Girl_NS_Walk");

    }

    // Update is called once per frame
    void Update()
    {

        transform.position = Vector2.MoveTowards(transform.position, moveSpots[randomSpot].position, speed * Time.deltaTime);

        if (Vector2.Distance(transform.position, moveSpots[randomSpot].position) < 0.2f)
        {
            if (waitTime <= 0)
            {
                gameObject.GetComponent<Animator>().Play("NPC_Girl_NS_Walk");
                randomSpot = Random.Range(0, moveSpots.Length);

                waitTime = startWaitTime;
                if (transform.position.x > moveSpots[randomSpot].position.x)
                {
                    if (facingRight == true)
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
                gameObject.GetComponent<Animator>().Play("NPC_Girl_NS_Idle");
            }
        }
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
