using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUIAnimator : MonoBehaviour
{
    GameObject panelToMove;
    GameObject panelDisableClick;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void showPanelSlideBottomToTop(GameObject panel, GameObject disableClickPanel)
    {
        // Do not do anything if our animator is playing something...
        // Perhaps we could queue it up?
        if (isPlaying())
            return;

        // Set the panel top to the bottom so it is off screen at the bottom
        float rectHeight = panel.GetComponent<RectTransform>().rect.height;
        panel.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -rectHeight);

        // Move panel
        movePanelTo(panel, Vector2.zero, 1f, disableClickPanel);
    }

    public void hidePanelSlideUp(GameObject panel, GameObject disableClickPanel)
    {
        // Do not do anything if our animator is playing something...
        // Perhaps we could queue it up?
        if (isPlaying())
            return;

        float rectHeight = panel.GetComponent<RectTransform>().rect.height;
        movePanelTo(panel, new Vector2(0, rectHeight), 1f, disableClickPanel);        
    }

    private void movePanelTo(GameObject panel, Vector2 to, float time, GameObject disableClickPanel)
    {
        // set globals
        panelToMove = panel;
        panelDisableClick = disableClickPanel;

        // Disable clicking
        panelDisableClick.SetActive(true);

        Vector2 rectPosition = panelToMove.GetComponent<RectTransform>().anchoredPosition;
        iTween.ValueTo(panelToMove, iTween.Hash("from", rectPosition, "to", to, "time", time, "onupdatetarget", gameObject, "onupdate", "UpdatePanelPosition", "oncompletetarget", gameObject, "oncomplete", "onMovePanelComplete", "easeType", "easeInCubic"));
    }

    private void UpdatePanelPosition(Vector2 newPosition)
    {
        RectTransform rectTransform = panelToMove.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = newPosition;
        Debug.Log("im moving" + newPosition);
        //panel.GetComponent<RectTransform>().anchoredPosition = position;
    }

    private void onMovePanelComplete()
    {
        // enable clicking
        panelDisableClick.SetActive(false);

        panelToMove = null;
        panelDisableClick = null;
        Debug.Log("FInished moving panel!");
    }


    public bool isPlaying()
    {
        return panelToMove != null ? true : false;
    }

}