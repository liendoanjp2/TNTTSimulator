using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CleanUpUI : MonoBehaviour
{
    public GameObject thisInstance;
    public GameObject trashObject1;
    public GameObject trashObject2;
    private int numberOfTrashPickedUp = 0;
    
    public bool pickUpTrash()
    {
        if(numberOfTrashPickedUp < 3)
        {
            numberOfTrashPickedUp++;

            if (numberOfTrashPickedUp == 1)
            {
                thisInstance.SetActive(true);
            }
            else if (numberOfTrashPickedUp == 2)
            {
                trashObject2.GetComponent<Image>().color = new Color(255, 255, 255, 255);
            }
            else
            {
                trashObject1.GetComponent<Image>().color = new Color(255, 255, 255, 255);
            }
            return true;
        }
        else
        {
            return false;
        }
    }

    public void dumpTrash()
    {
        trashObject1.GetComponent<Image>().color = new Color32(255, 255, 255, 50);
        trashObject2.GetComponent<Image>().color = new Color32(255, 255, 255, 50);
        thisInstance.SetActive(false);
        numberOfTrashPickedUp = 0;
    }

    // Start is called before the first frame update
    void Start()
    {
        thisInstance.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
