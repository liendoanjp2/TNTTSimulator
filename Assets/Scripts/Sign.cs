using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sign : MonoBehaviour, Interactable
{
    public GameObject signTextObject;
    public GameObject actualSign;
    public string signText;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Swag");
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        Debug.Log("Yolo");
        actualSign.SetActive(false);
    }

    public void Interact(GameObject player)
    {
        actualSign.SetActive(true);
        signTextObject.GetComponent<TMPro.TextMeshProUGUI>().text = signText;
        throw new System.NotImplementedException();
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