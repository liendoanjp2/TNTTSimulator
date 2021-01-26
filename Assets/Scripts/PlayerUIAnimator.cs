using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUIAnimator : MonoBehaviour
{
    GameObject panelToMove;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void showPanelSlideUp(GameObject panel)
    {
        // Do not do anything if our animator is playing something...
        // Perhaps we could queue it up?
        if (isPlaying())
            return;

        panelToMove = panel;
        Vector2 rectPosition = panelToMove.GetComponent<RectTransform>().anchoredPosition;
        Debug.Log(rectPosition);
        iTween.ValueTo(panelToMove, iTween.Hash("from", rectPosition, "to", Vector2.zero, "time", 1f, "onupdatetarget", gameObject, "onupdate", "UpdatePanelPosition", "easeType", "easeInCubic"));
    }

    private void UpdatePanelPosition(Vector2 newPosition)
    {
        RectTransform rectTransform = panelToMove.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = newPosition;
        Debug.Log("im moving" + newPosition);
        //panel.GetComponent<RectTransform>().anchoredPosition = position;
    }

    public bool isPlaying()
    {
        return panelToMove != null ? true : false;
    }

}