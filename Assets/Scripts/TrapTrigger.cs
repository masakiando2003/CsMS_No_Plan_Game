using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapTrigger : MonoBehaviour {

    [SerializeField] int trapIndex = 1;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        FindObjectOfType<Trap>().StartTrigger(trapIndex);
    }

}
