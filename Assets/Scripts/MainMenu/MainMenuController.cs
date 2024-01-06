using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{

    public GameObject newGameSelector;
    public GameObject exitSelector;
    public GameObject FadeInOutImg;
    public int currentOptionSelected;
    private Task task, task2;
    private bool waitingFadeToChangeScene;
    // Start is called before the first frame update
    void Start()
    {
        FadeInOutImg.SetActive(true);
        task2 = new Task(FadeInOutImg.GetComponent<TransitionFadeInOut>().FadeInOutEffect(false, 1));
        currentOptionSelected = 0;
        updateCurrentOptionSelected();        
    }


    // Update is called once per frame
    void Update()
    {
        if (waitingFadeToChangeScene)
        {
            if (!task.Running)
            {
                waitingFadeToChangeScene = false;
                SceneManager.LoadScene(1);
            }
        }
        else
        {
            keyboardListener();
        }
    }

    public void onClickAction(int actionId)
    {
        switch (actionId)
        {
            case 0:
                GameManager.sceneToLoad = 3;
                FadeInOutImg.SetActive(true);
                task = new Task(FadeInOutImg.GetComponent<TransitionFadeInOut>().FadeInOutEffect(true));
                waitingFadeToChangeScene = true;
                break;
            case 1: Application.Quit(); break;
        }
    }

    public void onHoverAction(int actionId) {
        currentOptionSelected = actionId;
        updateCurrentOptionSelected();
    }

    private void keyboardListener()
    {
        if(task2 != null && task2.Running && task != null && task.Running)
        {
            return;
        }

        if(Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
        {
            onClickAction(currentOptionSelected);
        }

        if(Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }

        if(Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (currentOptionSelected == 0) currentOptionSelected = 1;
            else currentOptionSelected = 0;
        }

        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (currentOptionSelected == 1) currentOptionSelected = 0;
            else currentOptionSelected = 1;
        }

        updateCurrentOptionSelected();
    }

    private void updateCurrentOptionSelected()
    {
        switch(currentOptionSelected)
        {
            case 0:
                newGameSelector.SetActive(true);
                exitSelector.SetActive(false);
                break;
            case 1: 
                newGameSelector.SetActive(false);
                exitSelector.SetActive(true);
                break;
        }
    }
}
