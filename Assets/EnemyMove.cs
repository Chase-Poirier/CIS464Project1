using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMove : MonoBehaviour
{
    public int moveSpeed = 3;
    public float attackRange = 1.5f;
    public int attackDamage = 20;
    private float timeSinceAttack = 0.0f;
    private float attackDelay = 0.2f;
    private int facingDirection = 1;
    private int healthBounty = 30;
    [SerializeField] Transform playerTransform;
    [SerializeField]Animator enemyAnim;
    SpriteRenderer enemySR;
    public LayerMask playerLayers;
    public Transform attackPointLeft;
    public Transform attackPointRight;

    public Animator animator;
    public int maxHealth = 100;
    public float followRadius = 5.0f;
    int currentHealth;

    private bool playerHasDied = false;

    void Start()
    {
      //get the player transform
        playerTransform = GameObject.FindWithTag("Player").GetComponent<Transform>();
      //enemy animation and sprite renderer 
        enemyAnim = gameObject.GetComponent<Animator>();
        enemySR = GetComponent<SpriteRenderer>();
        currentHealth=maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        if(GameObject.FindWithTag("Player").GetComponent<HeroKnight>().checkIfDead()){
                    playerHasDied = true;
        }
        timeSinceAttack += Time.deltaTime;
        if (checkFollowRadius(playerTransform.position.x,transform.position.x))
        {
            //if player in front of the enemies
            if (playerTransform.position.x < transform.position.x)
            {

                if (checkAttackRadius(playerTransform.position.x, transform.position.x))
                {
                    //for attack animation
                    if (timeSinceAttack > 2.0f && !playerHasDied){
                        enemyAnim.SetBool("IsWalking", false);
                        enemyAnim.SetTrigger("Attack");
                        //Attack();
                        StartCoroutine(DelayedAttack(attackDelay));
                        timeSinceAttack = 0.0f;
                    }
                }
                else
                {
                    this.transform.position += new Vector3(-moveSpeed * Time.deltaTime, 0f, 0f);
                    //for attack animation
                    //enemyAnim.SetBool("Attack", false);
                    //walk
                    enemyAnim.SetBool("IsWalking", true);
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
                    if (timeSinceAttack > 2.0f && !playerHasDied){
                        enemyAnim.SetBool("IsWalking", false);
                        enemyAnim.SetTrigger("Attack");
                        //Attack();
                        StartCoroutine(DelayedAttack(attackDelay));
                        timeSinceAttack = 0.0f;
                    }
                }
                else
                {
                    this.transform.position += new Vector3(moveSpeed * Time.deltaTime, 0f, 0f);
                    //for attack animation
                    //enemyAnim.SetBool("Attack", false);
                    //walk
                    enemyAnim.SetBool("IsWalking", true);
                    enemySR.flipX = false;
                    facingDirection = 1;
                }
            }
        }
        else
        {
            enemyAnim.SetBool("IsWalking", false);
        }
    }
    void Attack(){
        if(facingDirection == 1){
            Collider2D[] hitPlayers = Physics2D.OverlapCircleAll(attackPointRight.position, attackRange, playerLayers);
            foreach(Collider2D player in hitPlayers){
                Debug.Log("hit player");
                if(player.GetComponent<HeroKnight>().CheckBlocking(facingDirection)){return;}
                player.GetComponent<HeroKnight>().LoseHealth(attackDamage);
                
            }
        } else if(facingDirection == -1){
            Collider2D[] hitPlayers = Physics2D.OverlapCircleAll(attackPointLeft.position, attackRange, playerLayers);
            foreach(Collider2D player in hitPlayers){
                Debug.Log("hit player behind");
                if(player.GetComponent<HeroKnight>().CheckBlocking(facingDirection)){return;}
                player.GetComponent<HeroKnight>().LoseHealth(attackDamage);
            }
        }
    }

    private IEnumerator DelayedAttack(float frames){
        yield return new WaitForSeconds(frames);
        Attack();
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

        GameObject.FindWithTag("Player").GetComponent<HeroKnight>().GainHealth(healthBounty);

        //GetComponent<Collider2D>().enabled = false;
        Destroy(this.gameObject, .55f);
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
        if (Mathf.Abs(playerPosition - enemyPosition) < attackRange)
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

