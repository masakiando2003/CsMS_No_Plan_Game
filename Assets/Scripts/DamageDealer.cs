using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageDealer : MonoBehaviour {

    // Config parameter
    [SerializeField] int damage = 20, addHP = 100;
    [SerializeField] float reduceMP = 15.0f, addMP = 10.0f, addSP = 20.0f;

    public int GetDamage() { return damage; }
    public int GetAddHP() { return addHP; }
    public float GetReduceMP() { return reduceMP; }
    public float GetAddMP() { return addMP; }
    public float GetAddSP() { return addSP; }

    public void Hit() { Destroy(gameObject); }
}
