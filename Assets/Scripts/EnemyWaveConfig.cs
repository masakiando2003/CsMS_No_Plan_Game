using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Enemy Wave Config")]
public class EnemyWaveConfig : ScriptableObject
{

    [SerializeField] GameObject enemyPrefab, pathPrefab;
    [SerializeField] float timeBetweenSpawns = 0.5f, spawnRandomFactor = 0.3f, moveSpeed = 2f, actionTime = 2f;
    [SerializeField] int numberOfEnemies = 3, attackType = 0, enemyHP = 100;
    [SerializeField] bool faceLeft = false;

    public GameObject GetEnemyPrefab() { return enemyPrefab; }

    public List<Transform> GetWayPoints()
    {
        var waveWayPoints = new List<Transform>();
        foreach (Transform child in pathPrefab.transform)
        {
            waveWayPoints.Add(child);
        }
        return waveWayPoints;
    }

    public float GetTimeBetweenSpawns() { return timeBetweenSpawns; }

    public float GetSpawnRandomFactor() { return spawnRandomFactor; }

    public float GetMoveSpeed() { return moveSpeed; }

    public int GetNumberOfEnemies() { return numberOfEnemies; }

    public bool GetFaceLeft() { return faceLeft; }

    public int GetAttackType() { return attackType; }

    public int GetHP() { return enemyHP; }

    public float GetActionTime() { return actionTime; }
}