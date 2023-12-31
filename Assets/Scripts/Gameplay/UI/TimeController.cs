using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TimeController : MonoBehaviour
{
    public TMP_Text timeText;
    private float timeCounter;
    // Start is called before the first frame update
    void Start()
    {
        timeText.text = "00:00";
    }

    // Update is called once per frame
    void Update()
    {
        timeCounter += Time.deltaTime;
        updateTime();
    }

    private void updateTime()
    {
        float minutes = Mathf.FloorToInt(timeCounter / 60);
        float seconds = Mathf.FloorToInt(timeCounter % 60);
        timeText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public string getGameTime()
    {
        return timeText.text;
    }
}
