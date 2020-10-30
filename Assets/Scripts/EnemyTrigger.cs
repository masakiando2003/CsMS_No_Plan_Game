using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTrigger : MonoBehaviour {

    [SerializeField] int triggerIndex = 0;
    [SerializeField] GameObject[] boundaryObject;
    [SerializeField] GameObject[] enemySpawnerObject;
    [SerializeField] bool clearGameAfterBattle = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        SetEnemyTriggerIndex();
        ActivateBoundary();
        SetClearGameAfterBattle();
        EnterBattleState();
    }

    private void SetEnemyTriggerIndex()
    {
        FindObjectOfType<GameSession>().SetEnemyTriggerIndex(triggerIndex);
    }

    private void EnterBattleState()
    {
        string battleState = "Battle" + triggerIndex + "State";
        GameObject.Find("Battle Animator").GetComponent<Animator>().SetBool(battleState, true);
        for(int i = 0; i < enemySpawnerObject.Length; i++)
        {

            enemySpawnerObject[i].SetActive(true);
        }
    }

    private void ActivateBoundary()
    {
        FindObjectOfType<GameSession>().SetBoundaryObject(boundaryObject);
        for (int i = 0; i < boundaryObject.Length; i++)
        {
            boundaryObject[i].GetComponent<BoxCollider2D>().enabled = true;
        }
    }

    private void SetClearGameAfterBattle()
    {
        if(FindObjectsOfType<GameSession>().Length < 1)
        {
            return;
        }

        FindObjectOfType<GameSession>().SetClearFlag(clearGameAfterBattle);
    }

    public bool GetClearGameAfterBattle()
    {
        return clearGameAfterBattle;
    }
}
