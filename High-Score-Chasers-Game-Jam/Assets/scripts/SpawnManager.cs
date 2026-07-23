using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public static SpawnManager Instance;

    private void Awake()
    {
        if(Instance && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void SpawnObject(GameObject spawnObject, Vector3 spawnPosition)
    {
        Instantiate(spawnObject, spawnPosition, Quaternion.identity);
    }
}
