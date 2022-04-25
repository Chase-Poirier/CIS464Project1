using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class FlyingEnemySprite : MonoBehaviour
{
    public AIPath aiPath;

    private int facingDirection = 1;
    public int attackDamage = 30;
    public float attackRange = 1.5f;
    private int healthBounty = 50;
    private float timeSinceAttack = 0.0f;
    private float attackDelay = 0.25f;
    public LayerMask playerLayers;
    public Animator animator;
    public int maxHealth = 100;
    int currentHealth;
    SpriteRenderer enemySR;
    public Transform attackPoint;

    private bool isDead = false;

    [SerializeField] Transform playerTransform;
    private bool playerHasDied = false;

    void Start()
    {
      //get the player transform
        playerTransform = GameObject.FindWithTag("Player").GetComponent<Transform>();
      //enemy animation and sprite renderer 
        animator = gameObject.GetComponent<Animator>();
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
        if (checkAttackRadius(playerTransform.position.x, transform.position.x)){
            //for attack animation
            if (timeSinceAttack > 2.0f && !playerHasDied){
                animator.SetTrigger("Attack");
                //Attack();
                StartCoroutine(DelayedAttack(attackDelay));
                timeSinceAttack = 0.0f;
            }
        }

        if (aiPath.velocity.x >= 0.01f)
        {
            //transform.localScale = new Vector3(-2.5f, 2.5f, 1f);
            enemySR.flipX = false;
            facingDirection = 1;
        }else if (aiPath.velocity.x <= -0.01)
        {
            //transform.localScale = new Vector3(2.5f, 2.5f, 1f);
            enemySR.flipX = true;
            facingDirection = -1;
        }  
    }

    void Attack(){
        if(!isDead){
            Collider2D[] hitPlayers = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, playerLayers);
            foreach(Collider2D player in hitPlayers){
                Debug.Log("hit player");
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
        isDead = true;
        animator.SetBool("IsDead", true);
        GameObject.FindWithTag("Player").GetComponent<HeroKnight>().GainHealth(healthBounty);
        //GetComponent<Collider2D>().enabled = false;
        Destroy(transform.parent.gameObject, .55f);
        //this.parent.enabled = false;
    }

    public bool checkAttackRadius(float playerPosition, float enemyPosition){
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
