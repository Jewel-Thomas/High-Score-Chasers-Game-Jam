using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal : MonoBehaviour
{
    [SerializeField] private Scenes nextScene;
    
    private void OnTriggerEnter(Collider other)
    {

        if(other.CompareTag("Player"))
        {
            SceneManager.LoadScene(nextScene.ToString());
        }
    }

}
