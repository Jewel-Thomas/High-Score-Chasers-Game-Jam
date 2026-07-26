using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    //The scirpt is for controlling all the UI in the game 
    
    public static UIManager Instance;

    public Transform feedParent;
    public GameObject scoreFeedPrefab;

    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private Slider healthSlider;
    [SerializeField] private GameObject gamePlayUIObject;
    [SerializeField] private GameObject gameOverUIObject;

    private List<GameObject> activeEntries = new List<GameObject>();

    [SerializeField] private int maxVisibleEntries = 5;
        
    private void Awake()
    {
        if (Instance && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        SetGameOverUI(false);
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

    public void UpdateHealthSlider(float currentHealth, float maxHealth = 0)
    {
        if (maxHealth > 0) healthSlider.maxValue = maxHealth;
        healthSlider.value = currentHealth;
    }

    public void SetGameOverUI(bool isGameOver)
    {
        gamePlayUIObject.SetActive(!isGameOver);
        gameOverUIObject.SetActive(isGameOver);
    }
}
