using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMove : Enemy
{
    public int moveSpeed = 3;
    public float attackRange = 0.5f;
    public int attackDamage = 20;
    private float timeSinceAttack = 0.0f;
    private int facingDirection = 1;
    [SerializeField] Transform playerTransform;
    [SerializeField]Animator enemyAnim;
    SpriteRenderer enemySR;
    public LayerMask playerLayers;
    public Transform attackPointLeft;
    public Transform attackPointRight;

    void Start()
    {
      //get the player transform   
        playerTransform = GameObject.FindWithTag("Player").GetComponent<Transform>();
      //enemy animation and sprite renderer 
        enemyAnim = gameObject.GetComponent<Animator>();
        enemySR = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        timeSinceAttack += Time.deltaTime;
        if (checkFollowRadius(playerTransform.position.x,transform.position.x))
        {
            //if player in front of the enemies
            if (playerTransform.position.x < transform.position.x)
            {

                if (checkAttackRadius(playerTransform.position.x, transform.position.x))
                {
                    //for attack animation
                    if (timeSinceAttack > 2.0f){
                        enemyAnim.SetTrigger("Attack");
                        Attack();
                        timeSinceAttack = 0.0f;
                    }
                }
                else
                {
                    this.transform.position += new Vector3(-moveSpeed * Time.deltaTime, 0f, 0f);
                    //for attack animation
                    //enemyAnim.SetBool("Attack", false);
                    //walk
                    //enemyAnim.SetBool("Walking", true);
                    enemySR.flipX = true;
                    facingDirection = -1;
                }

            }
            //if player is behind enemies
            else if(playerTransform.position.x > transform.position.x)
            {
                if (checkAttackRadius(playerTransform.position.x, transform.position.x))
                {
                    //for attack animation
                    if (timeSinceAttack > 2.0f){
                        enemyAnim.SetTrigger("Attack");
                        Attack();
                        timeSinceAttack = 0.0f;
                    }
                }
                else
                {
                    this.transform.position += new Vector3(moveSpeed * Time.deltaTime, 0f, 0f);
                    //for attack animation
                    //enemyAnim.SetBool("Attack", false);
                    //walk
                    //enemyAnim.SetBool("Walking", true);
                    enemySR.flipX = false;
                    facingDirection = 1;
                }
            }
        }
        else
        {
            //enemyAnim.SetBool("Walking", false);
        }
    }
    void Attack(){
        if(facingDirection == 1){
            Collider2D[] hitPlayers = Physics2D.OverlapCircleAll(attackPointRight.position, attackRange, playerLayers);
            foreach(Collider2D player in hitPlayers){
                Debug.Log("hit player");
                player.GetComponent<HeroKnight>().LoseHealth(attackDamage);
            }
        } else if(facingDirection == -1){
            Collider2D[] hitPlayers = Physics2D.OverlapCircleAll(attackPointLeft.position, attackRange, playerLayers);
            foreach(Collider2D player in hitPlayers){
                Debug.Log("hit player behind");
                player.GetComponent<HeroKnight>().LoseHealth(attackDamage);
            }
        }
    }

}

