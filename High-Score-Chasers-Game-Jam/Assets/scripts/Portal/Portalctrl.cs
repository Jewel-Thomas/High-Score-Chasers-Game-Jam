using UnityEngine;
using TMPro;

public class Portalctrl : MonoBehaviour
{
   [SerializeField] private int requiredScore = 500;
   [SerializeField] private GameObject portal;
   [SerializeField] private TMP_Text TargetText;
   [SerializeField] private GameObject portalText;
     

    private void Start()
    {
        portal.SetActive(false);
        portalText.SetActive(false);

        ScoreManager.Instance.OnScoreChanged += CheckScore;

        // Check in case player already has enough score
        CheckScore(ScoreManager.Instance.TotalScore);

        
        TargetText.text = $"Target Score: {requiredScore}";
    }


    public void CheckScore(int score)
    {
        if(score >= requiredScore)
        {
            portal.SetActive(true);
            portalText.SetActive(true);
            Debug.Log("Portal Open!");
        }
    }

    private void OnDestroy()
    {
        if(ScoreManager.Instance != null)
        {
            ScoreManager.Instance.OnScoreChanged -= CheckScore;
        }
    } 
}
