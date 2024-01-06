using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingManager : MonoBehaviour
{

    public GameObject FadeInOutImg;
    private Task t1, t2, t3;
    private bool waitingForFirstFade;
    private bool waitingFadeToChangeScene;
    private bool waitingForSceneLoadToFadeOut;
    AsyncOperation sm;
    // Start is called before the first frame update
    void Start()
    {
        FadeInOutImg.SetActive(true);
        t1 = new Task(FadeInOutImg.GetComponent<TransitionFadeInOut>().FadeInOutEffect(false));
        waitingForFirstFade = true;
    }

    private void Update()
    {
        if (waitingForFirstFade && !t1.Running)
        {
            Invoke("startLoad", 2f);
            waitingForFirstFade = false;
        }

        if (waitingForSceneLoadToFadeOut && !t2.Running)
        {
            FadeInOutImg.SetActive(true);
            t3 = new Task(FadeInOutImg.GetComponent<TransitionFadeInOut>().FadeInOutEffect(true));
            waitingForSceneLoadToFadeOut = false;
            waitingFadeToChangeScene = true;
        }

        if(waitingFadeToChangeScene && !t3.Running)
        {
            sm.allowSceneActivation = true;
            waitingFadeToChangeScene = false;
        }
    }

    private void startLoad()
    {
        t2 = new Task(load());
        waitingForSceneLoadToFadeOut = true;
    }

    public IEnumerator load()
    {
        sm = SceneManager.LoadSceneAsync(GameManager.sceneToLoad);
        sm.allowSceneActivation = false;
        yield return null;
    }
}
