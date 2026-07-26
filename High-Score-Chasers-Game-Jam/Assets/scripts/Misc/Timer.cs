using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class Timer : MonoBehaviour
{
   [SerializeField] private float levelTime = 20f;
   

    private float currentTime;
    private bool timerRunning;

    public UnityEvent<float> OnTimerChanged;
    public UnityEvent OnTimerExpired;


    public float CurrentTime => currentTime;


    private void Start()
    {
        currentTime = levelTime;
        timerRunning = true;

        OnTimerChanged?.Invoke(currentTime);
    }


    private void Update()
    {
        if (!timerRunning)
            return;


        currentTime -= Time.deltaTime;

        currentTime = Mathf.Max(currentTime, 0);

        OnTimerChanged?.Invoke(currentTime);


        if(currentTime <= 0)
        {
            timerRunning = false;
            TimerExpired();
        }
    }


    private void TimerExpired()
    {
        Debug.Log("Time Up!");


        OnTimerExpired?.Invoke();
    }


    public void StopTimer()
    {
        timerRunning = false;
    }
}
