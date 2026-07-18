using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    //The scirpt is for controlling all the UI in the game 
    
    public static UIManager Instance;
    public Transform feedParent;
    public GameObject scoreFeedPrefab;

    [SerializeField] private TMP_Text scoreText;

    private List<GameObject> activeEntries = new List<GameObject>();

    [SerializeField] private int maxVisibleEntries = 5;
        
    private void Awake()
    {
        Instance = this;
    }

    public void UpdateScore(int score)
    {
        scoreText.text = $"Score: {score}";
    }

    public void AddScoreFeed(ScoreEvent Event)
    {
        GameObject entry = Instantiate(
        scoreFeedPrefab,
        feedParent
        );

        TMP_Text text = entry.GetComponentInChildren<TMP_Text>();

        text.text =
            "+" + Event.points +
            " " +
            Event.reason;

        activeEntries.Add(entry);

        if(activeEntries.Count > maxVisibleEntries)
        {
            Destroy(activeEntries[0]);

            activeEntries.RemoveAt(0);
        }
    }
}
