using UnityEngine;

public class StartmenuUI : MonoBehaviour
{
   [SerializeField] private GameObject mainMenuPanel;
   [SerializeField] private GameObject howToPanel;

    private void Start()
    {
        mainMenuPanel.SetActive(true);
        howToPanel.SetActive(false);
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
