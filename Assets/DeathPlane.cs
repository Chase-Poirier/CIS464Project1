using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathPlane : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private Transform respawnpoint;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.tag == "Player"){
            player.GetComponent<HeroKnight>().LoseHealth(30);
            if(!GameObject.FindWithTag("Player").GetComponent<HeroKnight>().checkIfDead()){
                player.transform.position = respawnpoint.transform.position;
            }
        }
    }


}
