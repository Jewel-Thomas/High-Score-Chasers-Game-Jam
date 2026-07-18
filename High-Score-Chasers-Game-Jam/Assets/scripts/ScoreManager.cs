using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class ScoreManager : MonoBehaviour
{
    //This handles scores that are then passed on to the UI manager
    public static ScoreManager Instance;

    public int TotalScore { get; private set; }

    private List<ScoreEvent> recentEvents = new List<ScoreEvent>();

    private void Awake()
    {
        Instance = this;
        
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
        Debug.Log("score added");
    }


}


