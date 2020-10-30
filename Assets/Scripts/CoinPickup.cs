using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinPickup : MonoBehaviour {

    [SerializeField] AudioClip coinPickupSFX;
    [SerializeField] int coinScore = 100;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetType() == typeof(BoxCollider2D)) { return; }
        int playerID = collision.GetComponentInParent<Player>().GetPlayerID();

        Debug.Log("Coin Collected");
        Debug.Log("Camera position: "+ Camera.main.transform.position);
        FindObjectOfType<GameSession>().AddToScore(coinScore, playerID);
        AudioSource.PlayClipAtPoint(coinPickupSFX, Camera.main.transform.position);
        Destroy(gameObject);
    }
}
