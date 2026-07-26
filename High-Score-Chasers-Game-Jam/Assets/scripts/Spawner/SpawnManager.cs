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

    public void SpawnObject(GameObject spawnObject, Vector3 spawnPosition, float maxSpawnCount)
    {
        if (GameManager.Instance.CurrentGameState == GameManager.GameState.GAME_OVER || maxSpawnCount <= 0) return;
        Instantiate(spawnObject, spawnPosition, Quaternion.identity);
    }
}
