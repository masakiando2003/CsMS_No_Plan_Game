using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeAttack : MonoBehaviour {

    [SerializeField] int playerID = 1;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.tag == "Enemy")
        {
            DamageDealer damageDealer = other.GetComponent<DamageDealer>();
            if (!damageDealer) { return; }
            other.gameObject.GetComponent<Enemy>().ProcessHit(other.GetComponentInParent<DamageDealer>(), playerID, 1);
        }
    }
}
