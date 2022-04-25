using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathPlane : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private Transform respawnpoint;

    private void OnTriggerEnter2D(Collider2D other)
    {
        player.transform.position = respawnpoint.transform.position;
        player.GetComponent<HeroKnight>().LoseHealth(30);
    }


}
