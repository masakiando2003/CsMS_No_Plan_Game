using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class EnemyWolf : MonoBehaviour
{

    // Config
    [Header("Config")]
    [SerializeField] int currentHP = 300, maxHP = 300, score = 100;
    [SerializeField] GameObject deathVFX;
    [SerializeField] float explosionDuration = 1f;
    [SerializeField] bool isHitPlayer = false;
    [SerializeField] bool isDie = false;
    [SerializeField] SimpleHealthBar enemyHPBar;
    [SerializeField] int attackType = 0;
    [SerializeField] GameObject meleeAttack;
    [SerializeField] float minMeleeAttackTime = 1f;
    [SerializeField] bool startAttack = false;

    [Header("ProjectTile")]
    [SerializeField] GameObject fireBallPrefab;
    [SerializeField] float projectTileSpeed = 10f;


    public Animator myAnimator;

    private void Update()
    {
        if(isDie) { return; }
        UpdateHPBar();
        AttackAnimUpdate();
    }

    float attackAnimTime = 3.5f;
    private void AttackAnimUpdate()
    {
        if (attackAnimTime < 0)
        {
            myAnimator.SetBool("Attack", false);
        }
        else
        {
            attackAnimTime -= Time.deltaTime;
        }
    }

    public void FireballAttackAnim()
    {
        attackAnimTime = 1.5f;
        myAnimator.SetBool("Attack", true);
    }

    private void UpdateHPBar()
    {
        enemyHPBar.UpdateBar(currentHP, maxHP);
    }

    public void FireSpell()
    {
        Debug.Log("Enemy Fire Spell!");
        FireballAttackAnim();
        //StartCoroutine(AttackDelay());
        //if (startAttack)
        //{
            GameObject fireBall = Instantiate(
                fireBallPrefab,
                new Vector2(transform.position.x, transform.position.y - 0.1f),
                Quaternion.identity) as GameObject;
            fireBall.GetComponent<Rigidbody2D>().velocity = new Vector2(transform.localScale.x * projectTileSpeed, 0f);
            fireBall.GetComponent<Transform>().localScale = transform.localScale;
        //}
        //startAttack = false;
    }

    IEnumerator AttackDelay()
    {
        yield return new WaitForSecondsRealtime(attackAnimTime);
        startAttack = true;
    }

    public void MeleeAttack()
    {
        meleeAttack.GetComponent<BoxCollider2D>().enabled = true;
        StartCoroutine(EndMeleeAttack());
    }

    IEnumerator EndMeleeAttack()
    {
        yield return new WaitForSecondsRealtime(minMeleeAttackTime);
        meleeAttack.GetComponent<BoxCollider2D>().enabled = false;
        myAnimator.SetBool("Attack", false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (FindObjectsOfType<GameSession>().Length < 1) { return; }
        if (other.gameObject.tag == "Range Attack")
        {
            DamageDealer damageDealer = other.GetComponent<DamageDealer>();
            if (!damageDealer) { return; }
            int playerID = other.GetComponent<RangeAttack>().GetBelongToPlayerID();
            ProcessHit(other.GetComponentInParent<DamageDealer>(), playerID, 1);
        }
    }

    public void ProcessHit(DamageDealer damageDealer, int playerID, int hitByAttack)
    {
        currentHP -= damageDealer.GetDamage();
        Debug.Log("Current HP: " + currentHP);
        if (currentHP <= 0 && isDie == false)
        {
            FindObjectOfType<GameSession>().DecreaseNumOfEnemies();
            Die();
            if (hitByAttack == 1)
            {
                FindObjectOfType<GameSession>().AddToScore(score, playerID);
            }
        }
    }

    private void Die()
    {
        GameObject explosion = Instantiate(deathVFX, transform.position, transform.rotation);
        Destroy(explosion, explosionDuration);
        //Destroy(gameObject);
        isDie = true;
    }

    public bool IsHitPlayer()
    {
        return isHitPlayer;
    }

    public void SetHitPlayer()
    {
        isHitPlayer = true;
    }

    public void SetAttackType(int attackType)
    {
        this.attackType = attackType;
    }

    public int GetAttackType()
    {
        return attackType;
    }

    public void SetEnemyHP(int configHP)
    {
        currentHP = maxHP = configHP;
    }
}
