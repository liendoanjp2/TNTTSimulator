using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;


public class NPCWalking : MonoBehaviour, Interactable
{

    public float speed;
    private float waitTime;
    public float startWaitTime;
    private bool facingRight = true;
    public GameObject textObject;
    public GameObject actualSign;
    public string signText;
    private bool intereacted = false;
    private bool isTyping = false;

    public Transform[] moveSpots;
    private int randomSpot;

    private void OnCollisionExit2D(Collision2D collision)
    {
        actualSign.SetActive(false);
        intereacted = false;
        textObject.GetComponent<TMPro.TextMeshProUGUI>().text = "";
    }

    // Start is called before the first frame update
    void Start()
    {
        randomSpot = Random.Range(0, moveSpots.Length);
        gameObject.GetComponent<Animator>().Play("NPC_Girl_NS_Walk");
        actualSign.SetActive(false);
        intereacted = false;

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

    public void Interact(GameObject player)
    {
        textObject.GetComponent<TMPro.TextMeshProUGUI>().text += "";
        if (!intereacted)
        {
            actualSign.SetActive(true);
            intereacted = true;
            if (!isTyping)
            {
                StartCoroutine(TypeText(signText));
            }
        }
        else
        {
            textObject.GetComponent<TMPro.TextMeshProUGUI>().text = "";
            actualSign.SetActive(false);
            intereacted = false;
        }
    }

    IEnumerator TypeText(string text)
    {
        isTyping = true;
        textObject.GetComponent<TMPro.TextMeshProUGUI>().text = "";
        foreach (char letter in text.ToCharArray())
        {
            if (letter.Equals('$'))
            {
                textObject.GetComponent<TMPro.TextMeshProUGUI>().text += "<br>";
            }
            else
            {
                textObject.GetComponent<TMPro.TextMeshProUGUI>().text += letter;
            }

            Thread.Sleep(20);
            yield return null;
        }
        isTyping = false;
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
