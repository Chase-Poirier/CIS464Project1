using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public Animator animator;
    public int maxHealth = 100;

    public float attackRadius = 2.5f;
    public float followRadius = 5.0f;
    int currentHealth;
    // Start is called before the first frame update
    void Start()
    {
        currentHealth=maxHealth;
    }

    public void TakeDamage(int damage){
        currentHealth -= damage;
        animator.SetTrigger("Hurt");

        if (currentHealth<=0){
            Die();
        }
    }

    void Die(){
        Debug.Log("dead");
        animator.SetBool("IsDead", true);

        GetComponent<Collider2D>().enabled = false;
        this.enabled = false;
    }

    //if player in radius move toward him 
    public bool checkFollowRadius(float playerPosition, float enemyPosition)
    {
        if(Mathf.Abs(playerPosition -enemyPosition) < followRadius)
        {
            //player in range
            return true;
        }
        else
        {
            return false;
        }
    }

    //if player in radius attack him
    public bool checkAttackRadius(float playerPosition, float enemyPosition)
    {
        if (Mathf.Abs(playerPosition - enemyPosition) < attackRadius)
        {
            //in range for attack
            return true;
        }
        else
        {
            return false;
        }
    }

}
