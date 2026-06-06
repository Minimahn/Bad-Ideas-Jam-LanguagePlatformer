using UnityEngine;

public class LevelInfo : MonoBehaviour
{
    private Vector3 spawnPoint;

    public Vector3 getSpawnPoint()
    {
        return spawnPoint;
    }
    public void setSpawnPoint(Vector3 v3)
    {
        spawnPoint = v3;
    }
}
