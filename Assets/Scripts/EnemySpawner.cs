using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour {

    [SerializeField] List<EnemyWaveConfig> waveConfigs;
    [SerializeField] int startingWave = 0;
    [SerializeField] bool looping = false;

    // Use this for initialization
    IEnumerator Start()
    {
        do
        {
            yield return StartCoroutine(SpawnAllWaves());
        } while (looping);

    }

    private IEnumerator SpawnAllWaves()
    {
        for (int waveIndex = startingWave; waveIndex < waveConfigs.Count; waveIndex++)
        {
            var currentWave = waveConfigs[waveIndex];
            yield return StartCoroutine(SpawnAllEnemiesInWaves(currentWave));
        }
    }

    private IEnumerator SpawnAllEnemiesInWaves(EnemyWaveConfig waveConfig)
    {
        FindObjectOfType<GameSession>().IncreaseNumOfEnemies(waveConfig.GetNumberOfEnemies());
        for (int enemyCount = 0; enemyCount < waveConfig.GetNumberOfEnemies(); enemyCount++)
        {
            var newEnemy = Instantiate(
                waveConfig.GetEnemyPrefab(),
                waveConfig.GetWayPoints()[0].transform.position,
                Quaternion.identity);
            newEnemy.GetComponent<EnemyMovement>().FlipFacingDirection(waveConfig.GetFaceLeft());
            newEnemy.GetComponent<EnemyMovement>().SetMoveSpeed(waveConfig.GetMoveSpeed());
            newEnemy.GetComponent<Enemy>().SetAttackType(waveConfig.GetAttackType());
            newEnemy.GetComponent<Enemy>().SetEnemyHP(waveConfig.GetHP());
            newEnemy.GetComponentInChildren<EnemyAttackAI>().SetActionTime(waveConfig.GetActionTime());
            yield return new WaitForSeconds(waveConfig.GetTimeBetweenSpawns());
        }

    }
}
