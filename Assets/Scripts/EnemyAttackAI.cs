using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackAI : MonoBehaviour
{

    [SerializeField] GameObject enemyObj;
    [SerializeField] float currentActionTime = 0.0f;
    [SerializeField] float minActionTime = 2.0f;
    [SerializeField] bool startAttack = false;

    private void Update()
    {
        MoveState();
        if (startAttack)
        {
            AttackState();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            SetAttackState(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        SetAttackState(false);
        MoveState();
    }

    private void AttackState()
    {
        enemyObj.GetComponent<EnemyMovement>().SetMoveSpeed(0f);
        enemyObj.GetComponent<EnemyMovement>().SetMoveAnimation(false);

        currentActionTime += Time.deltaTime;
        if (currentActionTime >= minActionTime)
        {
            int attackType = enemyObj.GetComponent<Enemy>().GetAttackType();
            switch (attackType)
            {
                case 0:
                    enemyObj.GetComponent<Enemy>().MeleeAttack();
                    break;
                case 1:
                    enemyObj.GetComponent<Enemy>().FireSpell();
                    break;
                default:
                    enemyObj.GetComponent<Enemy>().MeleeAttack();
                    break;

            }
            currentActionTime = 0.0f;
        }
    }

    public void SetActionTime(float configActionTime)
    {
        currentActionTime = minActionTime = configActionTime;
    }

    private void SetAttackState(bool isStartAttack)
    {
        startAttack = isStartAttack;
    }

    private void MoveState()
    {
        float moveSpeed = enemyObj.GetComponent<EnemyMovement>().GetDefaultMoveSpeed();
        enemyObj.GetComponent<EnemyMovement>().SetMoveSpeed(moveSpeed);
        enemyObj.GetComponent<EnemyMovement>().SetMoveAnimation(true);
    }
}
