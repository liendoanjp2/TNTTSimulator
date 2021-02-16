using System.Collections;
using System.Collections.Generic;
using System.IO;
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
    GameObject startMenuPanel;
    GameObject disableClickPanel;

    Text kinhTitle;
    Text kinhText;

    int difficultyLevel = 1;                                 // Difficulty level of the kinh to choose from
    Button correctAnswerButton;                              // Button in which the answer should be
    Kinh correctKinh;                                        // Kinh being used for the answer
    List<Button> wrongAnswerButtonList = new List<Button>(); // List of buttons for the wrong answers
    List<Kinh> wrongKinhList = new List<Kinh>();             // List of kinhs for the wrong answers

    string KinhListJsonFileNamePath = Application.streamingAssetsPath + "/KinhFiles/kinh.json";
    string fillInTheBlankString = "________";

    List<string> completedKinhContent = new List<string>();   // List of sentences of the kinh answered correctly
    // Data structure of kinhs, it will be indexed by key value pair: (kinhLevel, listOfKinhsWithThatLevel)
    Dictionary<int, List<Kinh>> kinhs = new Dictionary<int, List<Kinh>>();
    int numKinhLevels = 4;                                    // The amount of levels of the kinh (1 to 4, 4 being difficult)

    // Location for camera to move to on initial load
    Vector3 cameraLocationForStairs = new Vector3(-13.5f, 6.5f, -18.75f);

    bool gameEnded = false;

    GameObject stairs;

    GameObject player;
    Transform character;
    Camera playerCam;
    CameraMovement cameraMovement;
    PlayerMovement playerMovement;
    GameObject playerUI;
    PlayerUIAnimator playerUIAnimator;
    SceneController playerSceneController;

    // Start is called before the first frame update
    void Start()
    {
        // Save minigamepanel and disable it
        minigamePanel = gameObject.transform.Find("MinigamePanel").gameObject;
        minigamePanel.SetActive(false);

        // save startmenupanel and disable it
        startMenuPanel = gameObject.transform.Find("StartMenuPanel").gameObject;
        startMenuPanel.SetActive(false);

        // Get disable click panel
        disableClickPanel = gameObject.transform.Find("DisableClickPanel").gameObject;

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
        loadKinhFromFile();

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
        // Overall game object + character
        player = GameObject.Find("Player");
        character = player.transform.Find("Character");

        // Camera
        playerCam = player.transform.Find("Camera").GetComponent<Camera>();
        cameraMovement = playerCam.GetComponent<CameraMovement>();

        // Movement
        playerMovement = character.GetComponent<PlayerMovement>();

        // Scene controller
        playerSceneController = character.GetComponent<SceneController>();

        // UI
        playerUI = player.transform.Find("PlayerUI").gameObject;
        playerUIAnimator = playerUI.GetComponent<PlayerUIAnimator>();
        
    }

    // Player is ready, lets run
    public void run()
    {
        StartCoroutine(runSetupCoroutine());
    }

    private IEnumerator runSetupCoroutine()
    {
        // Initialize references to player components/gameobjects
        initializePlayerRefs();

        // Stop the playercam from following
        cameraMovement.stopFollowPlayer();

        playerCam.GetComponent<CameraMovement>().MoveTo(cameraLocationForStairs, 2f);

        // Character walk in to bottom step and zoom out
        Transform baseStairsTransform = stairs.transform.Find("Bottom");
        Debug.Log("Player: " + gameObject.transform.position + "| " + baseStairsTransform.position);
        playerMovement.MoveTo(baseStairsTransform.position, 2f);

        while (playerMovement.isMoveToOn() || cameraMovement.isMoveToOn())
        {
            yield return null;
        }

        // Fade in start menu
        playerUIAnimator.showPanelSlideBottomToTop(startMenuPanel, disableClickPanel);

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
        playerUIAnimator.hidePanelSlideUp(startMenuPanel, disableClickPanel);

        while (playerUIAnimator.isPlaying())
        {
            yield return null;
        }

        // Generate random kinh at difficulty 0 and set global
        chooseRandomKinh(difficultyLevel);

        // From here on loop this...
        // Do minigame iteration
        minigameIteration();

        // Slide minigamepanel up
        playerUIAnimator.showPanelSlideBottomToTop(minigamePanel, disableClickPanel);

        while (playerUIAnimator.isPlaying())
        {
            yield return null;
        }
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


    private void loadKinhFromFile()
    {
        StreamReader input_stream = new StreamReader(KinhListJsonFileNamePath);
        string fileContents = input_stream.ReadToEnd();

        //print(fileContents);

        // Get kinh list from json
        KinhListModel kinhListModel = JsonUtility.FromJson<KinhListModel>(fileContents);

        // Close to cleanup
        input_stream.Close();

        // Parse kinh list into our data structure
        // Loop through each kinh model and populate based on name, content, level
        kinhListModel.KinhList.ForEach((KinhModel kinhModel) =>
        {
            populateKinh(kinhModel.Name, kinhModel.Content, kinhModel.Level);
        });
    }

    // populate all the kinhs in this function
    private void populateKinh(string kinhName, string kinhContentString, int kinhLevel)
    {
        // parse the kinh by sentences/phrases/clauses
        char[] delimiterChars = { ',', '.'};
        string[] kinhContent = Regex.Split(kinhContentString, @"(?<=[.,;])");
        //string[] kinhContent = kinhContentString.Split(delimiterChars, System.StringSplitOptions.RemoveEmptyEntries);

        // Fix the content, regex.split has empty entry
        string[] fixedContent = new string[kinhContent.Length - 1];
        for(int index = 0; index < fixedContent.Length; index++)
        {
            fixedContent[index] = kinhContent[index];
        }
        

        Kinh kinh = new Kinh(kinhName, fixedContent);

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

        // Generate the kinh text for completedKinhContent list to display for user of the completed kinh
        string kinhTextString = "";
        foreach (string kinhLine in completedKinhContent)
        {
            kinhTextString += kinhLine;
        }

        // Initialize the start of the game with the title + kinh text
        kinhTitle.text = correctKinh.Name;
        // Set the text of the kinh on the UI
        kinhText.text = kinhTextString;

        // If there is no next kinh, we assume game is over...
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

        // Then set the correct kinh text to the correct answer button we chose
        correctAnswerButton.transform.Find("Text").GetComponent<Text>().text = nextKinhLine;

        // Populate the other buttons with random wrong answers using the initialized wrong button list
        assignRandomWrongAnswers();
    }



    /* Ensures that chosen random kinh hadn't already been picked + not the correct kinh */
    private Kinh chooseRandomWrongKinh()
    {
        Kinh randomWrongKinh = correctKinh;
        Kinh wrongKinhExists;
        do
        {
            // Get random kinh level, TODO make it so it picks only from the same level as the correct kinh
            int randomLevelIndex = Random.Range(0, numKinhLevels) + 1;

            // Get kinhlist on random difficulty level
            List<Kinh> kinhList = kinhs[randomLevelIndex];

            // Gets random kinh index
            int randomKinhIndex = Random.Range(0, kinhList.Count);

            randomWrongKinh = kinhList[randomKinhIndex];

            // Check if the random kinh we generated exists in wrong kinhlist
            // If not we add, else we choose again
            wrongKinhExists = wrongKinhList.Find((kinh) => { return kinh.Name.Equals(randomWrongKinh.Name); });


        } while (randomWrongKinh == correctKinh || wrongKinhExists != null);
        
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
            wrongKinhList.Add(randomWrongKinh);
            b.transform.Find("Text").GetComponent<Text>().text = randomWrongKinh.getRandomContent();
        }
    }
    public void onClickAnswer(Button button)
    {
        if (gameEnded)
            return;

        if (button == correctAnswerButton)
        {

            string buttonText = button.transform.Find("Text").GetComponent<Text>().text;
            completedKinhContent.Add(buttonText);

            // Get top floor of stairs position and map amount of kinhs to the vertical distance
            Vector3 topStairPosition = stairs.transform.Find("Top").position;
            // Get bottom floor
            Vector3 botStairPosition = stairs.transform.Find("Bottom").position;

            // Ratio completed of the content
            float contentRatio = correctKinh.getContentCompletedRatio();

            // Lerp vertical distance
            float stairPositionY = Mathf.Lerp(botStairPosition.y, topStairPosition.y, contentRatio);

            // Randomize horizontal distance, +- 1 unit?
            float randomX = Random.Range(botStairPosition.x - 1, botStairPosition.x + 1);

            // Add movement for player here
            playerMovement.MoveTo(new Vector3(randomX, stairPositionY, 0), 1f);

            // Continue with game
            minigameIteration();

            if (!gameEnded)
            {
                // Reset all buttons only if game hasnt ended
                resetButtons();
            }
        }
        else
        {
            // Mark answer wrong and not clickable
            onWrongAnswerClick(button);
            
        }
            Debug.Log(button);
    }

    private void gameHasEnded()
    {
        gameEnded = true;
        // Handle game ended
        // Replayability!?
        //correctKinh.resetContentIndex();

        //disableClickPanel.SetActive(true);
        Debug.Log("Game has ended");

        // Show winner screen, maybe play again?
        // Show congratulations then disable the clickpanel so that we could leave
    }

    private void onWrongAnswerClick(Button button)
    {
        // Disable button
        button.interactable = false;

        // Change normal color, can move to gamescene...
        ColorBlock buttonCB = button.colors;
        buttonCB.disabledColor = new Color(255, 100, 100, 210);
        
    }

    private void resetButtons()
    {
        answer1.interactable = true;
        answer2.interactable = true;
        answer3.interactable = true;
        answer4.interactable = true;
    }

    // Panel that the leave button is on (minigame/start)
    public void onLeaveButtonClicked(GameObject panel)
    {
        StartCoroutine(onLeaveHelper(panel));
    }

    private IEnumerator onLeaveHelper(GameObject panel)
    {
        // Slide the ui down
        // Fade out start menu
        playerUIAnimator.hidePanelSlideDown(panel, disableClickPanel);

        if (gameEnded)
        {
            // Get position of character leaving map
            Vector3 pointOffMap = new Vector3(0, 18, 0);

            // Move character off screen above if game has ended
            playerMovement.MoveTo(pointOffMap, 1f);
        }
        else
        {
            // Get position of character leaving map
            Vector3 pointOffMap = new Vector3(0, -5, 0);

            // Calculate time, 20f is estimate vertical distance
            float timeRatio = Vector3.Distance(character.position, pointOffMap) / 20f * 5f;

            // Move character off screen below if game hasnt ended
            playerMovement.MoveTo(pointOffMap, timeRatio);
        }

        while (playerUIAnimator.isPlaying() || playerMovement.isMoveToOn())
        {
            yield return null;
        }

        // Switch scene
        SceneController.SceneType sceneType = SceneController.SceneType.MainScene;
        string sceneName = SceneController.getSceneNameString(sceneType);
        playerSceneController.loadSceneMinigame(sceneName);

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

        public float getContentCompletedRatio()
        {
            return ((float)ContentIndex) / Content.Length;
        }

        public void resetContentIndex()
        {
            ContentIndex = 0;
        }

    }
}

