using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sign : MonoBehaviour, Interactable
{
    public GameObject signTextObject;
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
        if(!intereacted)
        {
            actualSign.SetActive(true);
            intereacted = true;
            signTextObject.GetComponent<TMPro.TextMeshProUGUI>().text = signText;
        }
        else
        {
            actualSign.SetActive(false);
            intereacted = false;
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

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}