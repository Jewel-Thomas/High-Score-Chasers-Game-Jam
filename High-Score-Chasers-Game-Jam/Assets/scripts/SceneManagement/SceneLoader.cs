using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [SerializeField] private Scenes nextScene;

    public void LoadNextSceneSync(bool isResetData = false)
    {
        if (isResetData) DestroyDontDestroyOnLoadObjects();
        SceneManager.LoadScene(nextScene.ToString());
    }

    public void LoadNextSceneAsync(bool isResetData = false)
    {
        if (isResetData) DestroyDontDestroyOnLoadObjects();
        SceneManager.LoadSceneAsync(nextScene.ToString());
    }

    private void DestroyDontDestroyOnLoadObjects()
    {
        List<GameObject> ddolObjects = new List<GameObject>();

        GameObject tempGO = new GameObject("TempDDOL");
        DontDestroyOnLoad(tempGO);

        Scene ddolScene = tempGO.scene;

        if (ddolScene.IsValid())
        {
            ddolScene.GetRootGameObjects(ddolObjects);
        }

        GameObject selfRoot = transform.root.gameObject;

        for (int i = ddolObjects.Count - 1; i >= 0; i--)
        {
            GameObject obj = ddolObjects[i];

            if (obj == tempGO || obj == selfRoot) continue;

            Destroy(obj);
        }

        Destroy(tempGO);

        if (selfRoot.scene == ddolScene)
        {
            Destroy(selfRoot);
        }
    }
}
