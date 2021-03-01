using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using UnityEngine;

public class PlayerUI : MonoBehaviour
{
    List<GameObject> requiredUIGO = new List<GameObject>();

    // Names of required objects that are childs of PlayerUI
    const string expPanelName = "Exp";
    const string sceneTransitionPanelName = "SceneTransition";
    const string signPopup = "SignPopup";
    private GameObject textObject;
    private GameObject actualSign;
    private Coroutine typingCoroutine;
    public int convoId;

    // Start is called before the first frame update
    void Start()
    {
        // Add requiredUI gameobjects here by name
        foreach (Transform child in transform)
        {
            switch (child.name)
            {
                case expPanelName:
                case sceneTransitionPanelName:
                    requiredUIGO.Add(child.gameObject);
                    break;
                case signPopup:
                    actualSign = child.gameObject;
                    textObject = child.Find("SignText").gameObject;
                    break;
            }

        }

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void talkToPlayer()
    {
        textObject.GetComponent<TMPro.TextMeshProUGUI>().text += "";

        string rawJSON = File.ReadAllText(Application.dataPath + @"\Characters\NPC\Dialogue\Dialogue.json");
        ConvoListClass convoList = JsonUtility.FromJson<ConvoListClass>(rawJSON);
        string convoText = convoList.ConvoList[convoId - 1].ConvoText;
        if (!actualSign.activeSelf)
        {
            actualSign.SetActive(true);
            typingCoroutine = StartCoroutine(TypeText(convoText));
        }
        else
        {
            if (textObject.GetComponent<TMPro.TextMeshProUGUI>().text == convoText)
            {
                closeText();
                return;
            }

            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
            textObject.GetComponent<TMPro.TextMeshProUGUI>().text = convoText;
        }
    }

    public void closeText()
    {
        textObject.GetComponent<TMPro.TextMeshProUGUI>().text = "";
        actualSign.SetActive(false);
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }
    }

    IEnumerator TypeText(string text, int typeSpeed = 20)
    {
        textObject.GetComponent<TMPro.TextMeshProUGUI>().text = "";
        foreach (char letter in text.ToCharArray())
        {
            textObject.GetComponent<TMPro.TextMeshProUGUI>().text += letter;

            Thread.Sleep(typeSpeed);
            yield return null;
        }
    }

    public void hideAllPlayerUI()
    {
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }
    }

    public void hideAllExceptSceneTransition()
    {
        foreach (Transform child in transform)
        {
            if(!child.name.Equals(sceneTransitionPanelName))
                child.gameObject.SetActive(false);
        }
    }

    public void showAllRequiredPlayerUI()
    {
        foreach(GameObject go in requiredUIGO)
        {
            go.SetActive(true);
        }
    }

    public void showAllPlayerUI()
    {
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(true);
        }
    }
}
