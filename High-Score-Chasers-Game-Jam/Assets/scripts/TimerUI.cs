using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;


public class TimerUI : MonoBehaviour
{
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private Timer timer;


    private void Start()
    {
        timer.OnTimerChanged.AddListener(UpdateTimer);
    }

    private void UpdateTimer(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60);
        int seconds = Mathf.FloorToInt(time % 60);

        timerText.text = 
            string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}
