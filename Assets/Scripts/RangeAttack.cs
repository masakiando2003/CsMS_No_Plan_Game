using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeAttack : MonoBehaviour {

    [SerializeField] int playerID = 0;

    private void OnTriggerEnter2D(Collider2D other)
    {
        Destroy(gameObject);
        //GameObject.Find("Player2").GetComponent<Player>().SetShooted(false);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Destroy(gameObject);
        //GameObject.Find("Player2").GetComponent<Player>().SetShooted(false);
    }

    public void SetBelongToPlayerID (int belongPlayerID)
    {
        playerID = belongPlayerID;
    }

    public int GetBelongToPlayerID()
    {
        return playerID;
    }
}
