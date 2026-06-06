using UnityEngine;

public class LevelInfo : MonoBehaviour
{
    private Vector3 spawnPoint;


    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    public Vector3 getSpawnPoint()
    {
        return spawnPoint;
    }
    public void setSpawnPoint(Vector3 v3)
    {
        spawnPoint = v3;
    }
}
