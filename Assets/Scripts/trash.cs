using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class trash : MonoBehaviour, Interactable
{

    public GameObject trashInstance;
    public GameObject cleanUpMiniGameUI;
    private bool pickedUp = false;

    public void Interact(GameObject player)
    {
        Animator playerAnimator = player.GetComponent<Animator>();
        PlayerState playerState = player.GetComponent<PlayerState>();
        cleanUpMiniGameUI.SetActive(true);
        if (cleanUpMiniGameUI.GetComponent<CleanUpUI>().pickUpTrash() && !pickedUp)
        {
            //StartCoroutine(FadeOut());
            playerState.setInteracting();
            playerAnimator.Play("Player_Doing");
            trashInstance.SetActive(false);
            pickedUp = true;
        }
    }

    IEnumerator FadeOut()
    {

        SpriteRenderer spriteRenderer = trashInstance.GetComponent<SpriteRenderer>();

        for (float f = 1f; f >= -0.05f; f -= 0.05f)
        {
            Color color = spriteRenderer.material.color;
            color.a = f;
            spriteRenderer.material.color = color;

            yield return new WaitForSeconds(0.05f);

        }
        trashInstance.SetActive(false);
    }

    public void onAnimationEnd()
    {
        
    }

    public void onAnimationEvent(GameObject player, InteractableAction action)
    {
        
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
