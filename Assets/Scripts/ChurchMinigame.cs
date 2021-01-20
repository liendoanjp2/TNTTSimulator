using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChurchMinigame : MonoBehaviour
{
    Button answer1;
    Button answer2; 
    Button answer3;

    GameObject stairs;
    List<string> kinh = new List<string>();
    List<string> completedKinh = new List<string>();
    // Start is called before the first frame update
    void Start()
    {
        answer1 = gameObject.transform.Find("Panel").Find("Answer1").GetComponent<Button>();
        answer2 = gameObject.transform.Find("Panel").Find("Answer2").GetComponent<Button>();
        answer3 = gameObject.transform.Find("Panel").Find("Answer3").GetComponent<Button>();
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
        playerMovement.movement = new Vector2(1, 0);

        // Character walk in to bottom step then zoom out
        Transform baseStairsTransform = stairs.transform.Find("Bottom");
        Debug.Log("Player: " + gameObject.transform.position + "| " + baseStairsTransform.position);
        playerMovement.MoveTo(baseStairsTransform.position, 2f);

        while (playerMovement.isMoveToOn())
        {
            yield return null;
        }

        playerCam.GetComponent<CameraMovement>().MoveTo(new Vector3(-13.5f, 6.5f, -18.75f), 2f);

        while (cameraMovement.isMoveToOn())
        {
            yield return null;
        }

        // Fade in start menu

        answer1.transform.Find("Text").GetComponent<Text>().text = kinh[0];
        answer2.transform.Find("Text").GetComponent<Text>().text = "2";
        answer3.transform.Find("Text").GetComponent<Text>().text = "3";


    }
}
