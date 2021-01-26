using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChurchMinigame : MonoBehaviour
{
    Button answer1;
    Button answer2; 
    Button answer3;
    Button answer4;
    GameObject minigamePanel;

    GameObject stairs;
    List<string> kinh = new List<string>();
    List<string> completedKinh = new List<string>();

    Vector3 bottomStepLocation = new Vector3(-13.5f, 6.5f, -18.75f);

    // Start is called before the first frame update
    void Start()
    {
        // Save minigamepanel
        minigamePanel = gameObject.transform.Find("MinigamePanel").gameObject;
        // Move the minigamepanel down
        float rectHeight = minigamePanel.GetComponent<RectTransform>().rect.height;
        minigamePanel.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -rectHeight);
        Debug.Log(minigamePanel.GetComponent<RectTransform>().anchoredPosition);

        // main panel contains title, question, answer
        GameObject mainPanel = minigamePanel.transform.Find("MainPanel").gameObject;

        // answer panel contains answers
        GameObject answerPanel = mainPanel.transform.Find("AnswerPanel").gameObject;
        GameObject leftAnswerPanel = answerPanel.transform.Find("InnerTextPanel").Find("LeftPanel").gameObject;
        GameObject rightAnswerPanel = answerPanel.transform.Find("InnerTextPanel").Find("RightPanel").gameObject;

        // Get references to the buttons
        answer1 = leftAnswerPanel.transform.Find("Answer1").GetComponent<Button>();
        answer2 = rightAnswerPanel.transform.Find("Answer2").GetComponent<Button>();
        answer3 = leftAnswerPanel.transform.Find("Answer3").GetComponent<Button>();
        answer4 = rightAnswerPanel.transform.Find("Answer4").GetComponent<Button>();


        // Get stairs in the scene
        stairs = GameObject.Find("Stairs");

        // Level 1 --> phrases delimit by /,*.*/
        // Level 2 --> delimit by / *.*,*/ to get words. Do every couple words as the level increase
        // Level 4 -- > delimit by / *.*,*/ to get words

        kinh.Add("Lạy Cha chúng con ở trên trời,");
        kinh.Add("chúng con nguyện danh Cha cả sáng,");
        kinh.Add("nước Cha trị đến, ý Cha thể hiện dưới đất cũng như trên trời.");
        kinh.Add("Xin Cha cho chúng con hôm nay lương thực hằng ngày,");
        kinh.Add("và tha nợ chúng con như chúng con cũng tha kẻ có nợ chúng con.");
        kinh.Add("Xin chớ để chúng con sa chước cám dỗ,");
        kinh.Add("nhưng cứu chúng con cho khỏi mọi sự dữ.Amen.");
    }

    public void UpdatePanelPosition(Vector2 newPosition)
    {
        /*        RectTransform rectTransform = panelToMove.GetComponent<RectTransform>();
                rectTransform.anchoredPosition = newPosition;*/
        Debug.Log("im moving" + newPosition);
        //panel.GetComponent<RectTransform>().anchoredPosition = position;
    }


    private void Awake()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Player is ready, lets run
    public void run()
    {
        StartCoroutine(runSetupCoroutine());

        // Tell script to run?
        // Select random kinh...

        // Assign text to buttons from kinh.
        // onClickHandler --> Check if kinh is correct, if not then we mark it red
        // If it is good, we then 1.) move the player, 2.) add it to the completedkinh, 3.) assign text to buttons with new line
 

        // Then loop thru the prayers
    }

    private IEnumerator runSetupCoroutine()
    {
        
        GameObject player = GameObject.Find("Player");
        Transform character = player.transform.Find("Character");
        Camera playerCam = player.transform.Find("Camera").GetComponent<Camera>();
        CameraMovement cameraMovement = playerCam.GetComponent<CameraMovement>();
        PlayerMovement playerMovement = character.GetComponent<PlayerMovement>();
        GameObject playerUI = player.transform.Find("PlayerUI").gameObject;
        PlayerUIAnimator playerUIAnimator = playerUI.GetComponent<PlayerUIAnimator>();

        playerCam.GetComponent<CameraMovement>().MoveTo(bottomStepLocation, 2f);
        playerMovement.movement = new Vector2(1, 0);

        // Character walk in to bottom step and zoom out
        Transform baseStairsTransform = stairs.transform.Find("Bottom");
        Debug.Log("Player: " + gameObject.transform.position + "| " + baseStairsTransform.position);
        playerMovement.MoveTo(baseStairsTransform.position, 2f);

        while (playerMovement.isMoveToOn() || cameraMovement.isMoveToOn())
        {
            yield return null;
        }

        playerUIAnimator.showPanelSlideUp(minigamePanel);

        while (playerUIAnimator.isPlaying())
        {
            yield return null;
        }

        // Fade in start menu

        answer1.transform.Find("Text").GetComponent<Text>().text = kinh[0];
        answer2.transform.Find("Text").GetComponent<Text>().text = "2";
        answer3.transform.Find("Text").GetComponent<Text>().text = "3";


    }
}

