using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public class ChurchMinigame : MonoBehaviour
{
    Button answer1;
    Button answer2; 
    Button answer3;
    Button answer4;
    GameObject minigamePanel;
    GameObject minigameDisableClickPanel;
    GameObject startMenuPanel;
    GameObject startMenuDisableClickPanel;

    Text kinhTitle;
    Text kinhText;

    Button correctAnswerButton;
    List<Button> wrongAnswerButtonList = new List<Button>();
    Kinh correctKinh;
    List<Kinh> wrongKinhList = new List<Kinh>();

    string disableClickPanelName = "DisableClickPanel";
    string fillInTheBlankString = "________";

    GameObject stairs;
    List<string> completedKinhContent = new List<string>();
    Dictionary<int, List<Kinh>> kinhs = new Dictionary<int, List<Kinh>>();
    List<string> allKinhNames;
    int numKinhLevels = 4;


    Vector3 bottomStepLocation = new Vector3(-13.5f, 6.5f, -18.75f);

    GameObject player;
    Transform character;
    Camera playerCam;
    CameraMovement cameraMovement;
    PlayerMovement playerMovement;
    GameObject playerUI;
    PlayerUIAnimator playerUIAnimator;

    // Start is called before the first frame update
    void Start()
    {
        // Save minigamepanel
        minigamePanel = gameObject.transform.Find("MinigamePanel").gameObject;
        // Move the minigamepanel down
        float rectHeight = minigamePanel.GetComponent<RectTransform>().rect.height;
        minigamePanel.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -rectHeight);

        // save startmenupanel
        startMenuPanel = gameObject.transform.Find("StartMenuPanel").gameObject;
        // Move the startmenupanel down
        rectHeight = startMenuPanel.GetComponent<RectTransform>().rect.height;
        startMenuPanel.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -rectHeight);

        // Get disable click panels
        minigameDisableClickPanel = minigamePanel.transform.Find(disableClickPanelName).gameObject;
        startMenuDisableClickPanel = startMenuPanel.transform.Find(disableClickPanelName).gameObject;


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

        // Get references to title + text for minigame
        GameObject titlePanel = mainPanel.transform.Find("TitlePanel").gameObject;
        GameObject kinhPanel = mainPanel.transform.Find("KinhPanel").gameObject;
        kinhTitle = titlePanel.transform.Find("InnerTextPanel").Find("TitleText").GetComponent<Text>();
        kinhText = kinhPanel.transform.Find("InnerTextPanel").Find("KinhText").GetComponent<Text>();

        // Get stairs in the scene
        stairs = GameObject.Find("Stairs");


        // Populate kinh from file
        hardcodedKinhs();

    }

    private void Awake()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    private void initializePlayerRefs()
    {
        player = GameObject.Find("Player");
        character = player.transform.Find("Character");
        playerCam = player.transform.Find("Camera").GetComponent<Camera>();
        cameraMovement = playerCam.GetComponent<CameraMovement>();
        playerMovement = character.GetComponent<PlayerMovement>();
        playerUI = player.transform.Find("PlayerUI").gameObject;
        playerUIAnimator = playerUI.GetComponent<PlayerUIAnimator>();
    }

    // Player is ready, lets run
    public void run()
    {
        StartCoroutine(runSetupCoroutine());

        // Tell script to run?
        // Select random kinh...

        // Assign text to buttons from kinh.
        // onClickHandler --> Check if kinh is correct, if not then we mark it red
        // If it is good, we then 1.) move the player, 2.) add it to the completedKinhContent, 3.) assign text to buttons with new line


        // Then loop thru the prayers
    }

    private IEnumerator runSetupCoroutine()
    {
        // Initialize references to player components/gameobjects
        initializePlayerRefs();

        playerCam.GetComponent<CameraMovement>().MoveTo(bottomStepLocation, 2f);

        // Character walk in to bottom step and zoom out
        Transform baseStairsTransform = stairs.transform.Find("Bottom");
        Debug.Log("Player: " + gameObject.transform.position + "| " + baseStairsTransform.position);
        playerMovement.MoveTo(baseStairsTransform.position, 2f);

        while (playerMovement.isMoveToOn() || cameraMovement.isMoveToOn())
        {
            yield return null;
        }

        // Fade in start menu
        playerUIAnimator.showPanelSlideBottomToTop(startMenuPanel, startMenuDisableClickPanel);

        while (playerUIAnimator.isPlaying())
        {
            yield return null;
        }

    }

    public void onStartMinigameButtonClick()
    {
        StartCoroutine(startMinigame());
    }

    private IEnumerator startMinigame()
    {
        // Fade out start menu
        playerUIAnimator.hidePanelSlideUp(startMenuPanel, startMenuDisableClickPanel);

        while (playerUIAnimator.isPlaying())
        {
            yield return null;
        }

        // Generate random kinh at difficulty 0 and set global
        chooseRandomKinh(0);

        // From here on loop this...

        // Do minigame iteration
        minigameIteration();

        // Slide minigamepanel up
        playerUIAnimator.showPanelSlideBottomToTop(minigamePanel, minigameDisableClickPanel);

        while (playerUIAnimator.isPlaying())
        {
            yield return null;
        }
    }

    // TO REMOVE!!!
    private void hardcodedKinhs()
    {
        string kinhName = "Kinh Lạy Cha";
        string kinhContentString = "Lạy Cha chúng con ở trên trời,chúng con nguyện danh Cha cả sáng, " +
                                    "nước Cha trị đến,ý Cha thể hiện dưới đất cũng như trên trời. " +
                                    "Xin Cha cho chúng con hôm nay lương thực hằng ngày, " +
                                    "và tha nợ chúng con như chúng con cũng tha kẻ có nợ chúng con. " +
                                    "Xin chớ để chúng con sa chước cám dỗ, " +
                                    "nhưng cứu chúng con cho khỏi sự dữ. Amen.";

        populateKinh(kinhName, kinhContentString, 0);

        kinhName = "Kinh Kính Mừng";
        kinhContentString = "Kính mừng Maria đầy ơn phúc Đức Chúa Trời ở cùng Bà, Bà có phúc lạ hơn mọi người nữ, " +
                            "và Giêsu con lòng Bà gồm phúc lạ. Thánh Maria Đức Mẹ Chúa Trời cầu cho chúng con là " +
                            "kẻ có tội khi này và trong giờ lâm tử, Amen.";

        populateKinh(kinhName, kinhContentString, 1);

        kinhName = "Kinh Sáng Danh";
        kinhContentString = "Sáng danh Đức Chúa Cha và Đức Chúa Con, và Đức Chúa Thánh Thần. Như đã có trước vô " +
                            "cùng và bây giờ và hằng có và đời đời chẳng cùng. Amen.";

        populateKinh(kinhName, kinhContentString, 2);
        kinhName = "Kinh Rước Lễ Thiêng Liêng";
        kinhContentString = "Lạy Chúa Giêsu Thánh Thể. Con yêu mến Chúa. Xin Chúa ngự vào tâm hồn con, " +
                            "và ở lại với con luôn mãi. Amen.";

        populateKinh(kinhName, kinhContentString, 3);
    }

    // populate all the kinhs in this function
    private void populateKinh(string kinhName, string kinhContentString, int kinhLevel)
    {
        // parse the kinh by sentences/phrases/clauses
        char[] delimiterChars = { ',', '.'};
        string[] kinhContent = kinhContentString.Split(delimiterChars, System.StringSplitOptions.RemoveEmptyEntries);

        Kinh kinh = new Kinh(kinhName, kinhContent);

        if (!kinhs.ContainsKey(kinhLevel)) {
            // Create list and initialize with our newly created kinh
            List<Kinh> kinhList = new List<Kinh>() { kinh };
            kinhs.Add(kinhLevel, kinhList);
        }
        else
        {
            // Access the existing list and append
            kinhs[kinhLevel].Add(kinh);
        }

    }

    private void minigameIteration()
    {
        // Get the next line of the kinh
        string nextKinhLine = correctKinh.getNextContent();

        // Generate the kinh text for completedKinhContent list
        string kinhTextString = "";
        foreach (string kinhLine in completedKinhContent)
        {
            kinhTextString += kinhLine;
        }

        // Initialize the start of the game with the title + kinh text
        kinhTitle.text = correctKinh.Name;
        // Set the text of the kinh on the UI
        kinhText.text = kinhTextString;

        if (nextKinhLine == null)
        {
            // Game has ended!
            gameHasEnded();
            return;
        }

        // Game has not ended so append to the text _____
        kinhText.text = kinhText.text + fillInTheBlankString;

        // Choose random correct answer button + initialize wrong answer button list and set global
        chooseRandomCorrectAnswerButton();

        // Then set the correct kinh answer to it;
        correctAnswerButton.transform.Find("Text").GetComponent<Text>().text = nextKinhLine;

        // Random wrong answers
        assignRandomWrongAnswers();
    }

    /* Chooses a random kinh and assigns it globally to correctKinh */
    private void chooseRandomKinh(int difficulty)
    {
        // Get kinhlist
        List<Kinh> kinhList = kinhs[difficulty];

        // Gets random kinh
        int randomIndex = Random.Range(0, kinhList.Count);

        correctKinh = kinhList[randomIndex];
    }

    /* Ensures that chosen random kinh hadn't already been picked + not the correct kinh */
    private Kinh chooseRandomWrongKinh()
    {
        Kinh randomWrongKinh = correctKinh;
        do
        {
            // Get random kinh level
            int randomLevelIndex = Random.Range(0, numKinhLevels);

            // Get kinhlist on random difficulty level
            List<Kinh> kinhList = kinhs[randomLevelIndex];

            // Gets random kinh index
            int randomKinhIndex = Random.Range(0, kinhList.Count);

            randomWrongKinh = kinhList[randomKinhIndex];

            // Check if the random kinh we generated exists in wrong kinhlist
            // If not we add, else we choose again
            wrongKinhList.Find((kinh) => { return kinh.Name.Equals(randomWrongKinh.Name); });

        } while (randomWrongKinh == correctKinh);
        
        return randomWrongKinh;
    }

    private void chooseRandomCorrectAnswerButton()
    {
        int randomButtonNumber = Random.Range(0, 4);

        switch (randomButtonNumber)
        {
            case 0:
                correctAnswerButton = answer1;
                wrongAnswerButtonList = new List<Button>() { answer2, answer3, answer4 };
                break;
            case 1:
                correctAnswerButton = answer2;
                wrongAnswerButtonList = new List<Button>() { answer1, answer3, answer4 };
                break;
            case 2:
                correctAnswerButton = answer3;
                wrongAnswerButtonList = new List<Button>() { answer1, answer2, answer4 };
                break;
            case 3:
                correctAnswerButton = answer4;
                wrongAnswerButtonList = new List<Button>() { answer1, answer2, answer3 };
                break;
        }
    }

    private void assignRandomWrongAnswers()
    {
        // new kinh list because assigning new randomwronganswers
        wrongKinhList = new List<Kinh>();

        foreach (Button b in wrongAnswerButtonList)
        {
            Kinh randomWrongKinh = chooseRandomWrongKinh();

            b.transform.Find("Text").GetComponent<Text>().text = randomWrongKinh.getRandomContent();
        }
    }
    public void onClickAnswer(Button button)
    {
        if (button == correctAnswerButton)
        {
            string buttonText = button.transform.Find("Text").GetComponent<Text>().text;
            completedKinhContent.Add(buttonText);
            minigameIteration();
        }
            Debug.Log(button);
        // If correct answer
        // do minigameIteration();
    }

    private void gameHasEnded()
    {
        // Handle game ended
        // Replayability!
        correctKinh.resetContentIndex();

        minigameDisableClickPanel.SetActive(true);
        Debug.Log("Game has ended");

        // Show winner screen, maybe play again?
    }

    private class Kinh
    {
        public string Name { get; set; }
        private string[] Content;

        private int ContentIndex = 0;

        public Kinh(string name, string[] content)
        {
            this.Name = name;
            this.Content = content;
        }

        public string getNextContent()
        {
            if(ContentIndex >= Content.Length)
            {
                return null;
            }
            string contentToReturn = this.Content[ContentIndex];
            this.ContentIndex++;
            return contentToReturn;
        }

        public string getRandomContent()
        {
            // Prevent getting randomly the last word cause its usually "Amen"
            int randomIndex = Random.Range(0, this.Content.Length - 1);
            return this.Content[randomIndex];
        }

        public void resetContentIndex()
        {
            ContentIndex = 0;
        }

    }
}

