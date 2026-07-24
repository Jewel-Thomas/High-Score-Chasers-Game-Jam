using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;


public class ScoreManager : MonoBehaviour
{
    //This handles scores that are then passed on to the UI manager
    public static ScoreManager Instance;
    //[SerializeField] private int requiredScore = 500;
    
    public int TotalScore { get; private set; }
    public event Action<int> OnScoreChanged;
    private List<ScoreEvent> recentEvents = new List<ScoreEvent>();

    private void Awake()
    {
        TargetText.text = $"Target: {requiredScore}";
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        
        
    }

    string GetReason(ScoreType type)
    {
        switch(type)
        {
            case ScoreType.AirTime:
                return "Air Time";

            case ScoreType.Waypoint:
                return "Waypoint";

            case ScoreType.SpeedTicket:
                return "Speed Ticket";

            case ScoreType.EnemyHit:
                return "TakeDown";
                
            case ScoreType.CollectPower:
                return "Power Cell Collected";

            default:
                return "???";
        }
    }

     public void AddScore(int points, ScoreType type)
    {
        TotalScore += points;

        ScoreEvent newEvent = new ScoreEvent(points, GetReason(type));
        recentEvents.Add(newEvent);

        UIManager.Instance.UpdateScore(TotalScore);
        UIManager.Instance.AddScoreFeed(newEvent);
        
        OnScoreChanged?.Invoke(TotalScore);
    }


}


