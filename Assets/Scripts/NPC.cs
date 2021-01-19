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

    private void OnCollisionExit2D(Collision2D collision)
    {
        actualSign.SetActive(false);
        intereacted = false;
    }

    public void Interact(GameObject player)
    {
        if (!intereacted)
        {
            actualSign.SetActive(true);
            intereacted = true;
            StartCoroutine(TypeText(signText));
        }
        else
        {
            actualSign.SetActive(false);
            intereacted = false;
        }
    }

    IEnumerator TypeText(string text)
    {
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