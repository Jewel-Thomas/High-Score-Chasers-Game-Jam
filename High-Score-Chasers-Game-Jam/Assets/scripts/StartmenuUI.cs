using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;


public class StartmenuUI : MonoBehaviour
{
   [SerializeField] private GameObject mainMenuPanel;
   [SerializeField] private GameObject howToPanel;

   [SerializeField] private string gameSceneName = "PastScene"; // Change to your scene name

    private void Start()
    {
        mainMenuPanel.SetActive(true);
        howToPanel.SetActive(false);
    }

    public void PlayGame()
    {
        SceneManager.LoadScene(gameSceneName);
    }

    public void HowTo()
    {
        mainMenuPanel.SetActive(false);
        howToPanel.SetActive(true);
    }

    public void CloseHowTo()
    {
        howToPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }
}
