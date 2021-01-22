using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

public class NPC : MonoBehaviour, Interactable
{
    public GameObject textObject;
    public GameObject actualSign;
    public string signText;
    private bool intereacted = false;
    private bool isTyping = false;

    private void OnCollisionExit2D(Collision2D collision)
    {
        actualSign.SetActive(false);
        intereacted = false;
        textObject.GetComponent<TMPro.TextMeshProUGUI>().text = "";
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
        foreach(char letter in text.ToCharArray())
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

    public void onAnimationEnd()
    {
        throw new System.NotImplementedException();
    }

    public void onAnimationEvent(GameObject player, InteractableAction action)
    {
        throw new System.NotImplementedException();
    }
}