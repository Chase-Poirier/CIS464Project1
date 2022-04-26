using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class HeroKnight : MonoBehaviour {

    [SerializeField] float      m_speed = 5.0f;
    [SerializeField] float      m_jumpForce = 8.0f;
    [SerializeField] float      m_rollForce = 8.0f;
    //[SerializeField] bool       m_noBlood = false;
    [SerializeField] GameObject m_slideDust;
    [SerializeField] private float invincibilityDurationSeconds =0.75f;
    [SerializeField] private float invincibilityDeltaTime =0.15f;

    [SerializeField] private IntSO healthSO;

    private Animator            m_animator;
    private Rigidbody2D         m_body2d;
    private Sensor_HeroKnight   m_groundSensor;
    private Sensor_HeroKnight   m_wallSensorR1;
    private Sensor_HeroKnight   m_wallSensorR2;
    private Sensor_HeroKnight   m_wallSensorL1;
    private Sensor_HeroKnight   m_wallSensorL2;
    private bool                m_isWallSliding = false;
    private bool                m_grounded = false;
    private bool                m_rolling = false;
    private bool                m_blocking = false;
    private int                 m_facingDirection = 1;
    private int                 m_currentAttack = 0;
    private float               m_timeSinceAttack = 0.0f;
    private float               m_delayToIdle = 0.0f;
    private float               m_rollDuration = 8.0f / 14.0f;
    private float               m_rollCurrentTime;

    public Transform attackPoint;
    public Transform attackPointBehind;
    public float attackRange = 0.5f;
    public LayerMask enemyLayers;

    public int attackDamage = 35;

    public int maxHealth = 100;
    int currentHealth;
    private bool isInvincible = false;

    private bool isDead = false;
    public HealthBar healthBar;

    // Use this for initialization
    void Start ()
    {
        currentHealth = healthSO.Value;
        healthBar.SetMaxHealth(maxHealth);
        healthBar.SetHealth(currentHealth);
        m_animator = GetComponent<Animator>();
        m_body2d = GetComponent<Rigidbody2D>();
        m_groundSensor = transform.Find("GroundSensor").GetComponent<Sensor_HeroKnight>();
        m_wallSensorR1 = transform.Find("WallSensor_R1").GetComponent<Sensor_HeroKnight>();
        m_wallSensorR2 = transform.Find("WallSensor_R2").GetComponent<Sensor_HeroKnight>();
        m_wallSensorL1 = transform.Find("WallSensor_L1").GetComponent<Sensor_HeroKnight>();
        m_wallSensorL2 = transform.Find("WallSensor_L2").GetComponent<Sensor_HeroKnight>();

        InvokeRepeating("HealthBurn", 1f, 1f);  //1s delay, repeat every 1s
    }

    void HealthBurn(){
        currentHealth -= 1;
        healthBar.SetHealth(currentHealth);
        Debug.Log(currentHealth);

        if (currentHealth <= 0 && !isDead)
        {
            Die();
            return;
        }
    }

    // Update is called once per frame
    void Update ()
    {
        // Increase timer that controls attack combo
        m_timeSinceAttack += Time.deltaTime;

        // Increase timer that checks roll duration
        if(m_rolling)
            m_rollCurrentTime += Time.deltaTime;

        // Disable rolling if timer extends duration
        if(m_rollCurrentTime > m_rollDuration)
            m_rolling = false;

        //Check if character just landed on the ground
        if (!m_grounded && m_groundSensor.State())
        {
            m_grounded = true;
            m_animator.SetBool("Grounded", m_grounded);
        }

        //Check if character just started falling
        if (m_grounded && !m_groundSensor.State())
        {
            m_grounded = false;
            m_animator.SetBool("Grounded", m_grounded);
        }

        // -- Handle input and movement --
        float inputX = Input.GetAxis("Horizontal");

        // Swap direction of sprite depending on walk direction
        if (inputX > 0)
        {
            GetComponent<SpriteRenderer>().flipX = false;
            m_facingDirection = 1;
        }
            
        else if (inputX < 0)
        {
            GetComponent<SpriteRenderer>().flipX = true;
            m_facingDirection = -1;
        }

        // Move
        if (!m_rolling && !m_blocking){
            m_body2d.velocity = new Vector2(inputX * m_speed, m_body2d.velocity.y);
        } else if(m_blocking && m_grounded){
            m_body2d.velocity = new Vector2(0 * m_speed, m_body2d.velocity.y);
        }

        //Set AirSpeed in animator
        m_animator.SetFloat("AirSpeedY", m_body2d.velocity.y);

        // -- Handle Animations --
        //Wall Slide
        m_isWallSliding = (m_wallSensorR1.State() && m_wallSensorR2.State()) || (m_wallSensorL1.State() && m_wallSensorL2.State());
        m_animator.SetBool("WallSlide", m_isWallSliding);

        //Attack
        if(Input.GetKeyDown("j") && m_timeSinceAttack > 0.25f && !m_rolling)
        {
            m_currentAttack++;
            Attack();
            m_blocking = false;

            // Loop back to one after third attack
            if (m_currentAttack > 3)
                m_currentAttack = 1;

            // Reset Attack combo if time since last attack is too large
            if (m_timeSinceAttack > 1.0f)
                m_currentAttack = 1;

            // Call one of three attack animations "Attack1", "Attack2", "Attack3"
            m_animator.SetTrigger("Attack" + m_currentAttack);

            // Reset timer
            m_timeSinceAttack = 0.0f;
        }

        // Block
        else if (Input.GetKeyDown("k") && !m_rolling)
        {
            m_animator.SetTrigger("Block");
            m_animator.SetBool("IdleBlock", true);
            
            m_blocking = true;
        }

        else if (Input.GetKeyUp("k")){
            m_animator.SetBool("IdleBlock", false);
            m_blocking = false;
        }

        //"DEBUG"
        else if (Input.GetKeyUp("p")){
            currentHealth = 100;
            healthBar.SetHealth(currentHealth);
        }

        // Roll
        else if (Input.GetKeyDown("left shift") && !m_rolling && !m_isWallSliding)
        {
            m_rolling = true;
            m_blocking = false;
            m_animator.SetTrigger("Roll");
            FindObjectOfType<AudioManager>().Play("Roll");
            m_body2d.velocity = new Vector2(m_facingDirection * m_rollForce, m_body2d.velocity.y);
        }  

        //Jump
        else if (Input.GetKeyDown("space") && m_grounded && !m_rolling)
        {
            m_animator.SetTrigger("Jump");
            m_grounded = false;
            FindObjectOfType<AudioManager>().Play("Jump");
            m_animator.SetBool("Grounded", m_grounded);
            m_body2d.velocity = new Vector2(m_body2d.velocity.x, m_jumpForce);
            m_groundSensor.Disable(0.2f);
        }

        //Run
        else if (Mathf.Abs(inputX) > Mathf.Epsilon)
        {
            // Reset timer
            m_delayToIdle = 0.05f;
            m_animator.SetInteger("AnimState", 1);
        }

        //Idle
        else
        {
            // Prevents flickering transitions to idle
            m_delayToIdle -= Time.deltaTime;
                if(m_delayToIdle < 0)
                    m_animator.SetInteger("AnimState", 0);
        }
    }

    // Animation Events
    // Called in slide animation.
    void AE_SlideDust()
    {
        Vector3 spawnPosition;

        if (m_facingDirection == 1)
            spawnPosition = m_wallSensorR2.transform.position;
        else
            spawnPosition = m_wallSensorL2.transform.position;

        if (m_slideDust != null)
        {
            // Set correct arrow spawn position
            GameObject dust = Instantiate(m_slideDust, spawnPosition, gameObject.transform.localRotation) as GameObject;
            // Turn arrow in correct direction
            dust.transform.localScale = new Vector3(m_facingDirection, 1, 1);
        }
    }


    void OnCollisionEnter2D(Collision2D col) {
        if(col.gameObject.tag == "Enemy")
        {
            Debug.Log("I have collided!");
            LoseHealth(20);
            if(!isDead){
                m_body2d.AddForce(col.contacts[0].normal * 50f);
            }
        }
        if(col.gameObject.tag == "Transition"){
            healthSO.Value = currentHealth;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }

    private IEnumerator Hitstun(){
        this.enabled=false;
        yield return new WaitForSeconds(.5f);
        if(!isDead)
            this.enabled=true;
    }

    public void LoseHealth(int amount){
        if (isInvincible) return;
        StartCoroutine(Hitstun());
        m_blocking = false;
        currentHealth -= amount;
        healthBar.SetHealth(currentHealth);
        m_animator.SetTrigger("Hurt");
        Debug.Log("Health=" + currentHealth);

        // The player died
        if (currentHealth <= 0)
        {
            Die();
            return;
        }

        StartCoroutine(BecomeTemporarilyInvincible());
    }

    private void Die(){
        isDead = true;
        currentHealth = 0;
        Debug.Log("Death");
        //m_animator.SetBool("noBlood", m_noBlood);
        m_animator.SetTrigger("Death");
        this.enabled=false;
        StartCoroutine(DeathTransition());
        // Broadcast some sort of death event here before returning
        return;
    }

    private IEnumerator DeathTransition(){
        yield return new WaitForSeconds(3.5f);
        SceneManager.LoadScene(0);
    }

    public bool CheckBlocking(int facing){
        if(m_facingDirection!=facing && m_blocking){
            FindObjectOfType<AudioManager>().Play("Block");
            return true;
        } else {
            return false;
        }
    }

    public void GainHealth(int amount){
        currentHealth += amount;
        healthBar.SetHealth(currentHealth);
        if(currentHealth>maxHealth){
            currentHealth=maxHealth;
        }
        Debug.Log("Gained health, new health=" + currentHealth);
    }

    public bool checkIfDead(){
        if (isDead){
            return true;
        } else{
            return false;
        }
    }

    private IEnumerator BecomeTemporarilyInvincible()
    {
        Debug.Log("Player turned invincible!");
        isInvincible = true;

        for (float i = 0; i < invincibilityDurationSeconds; i += invincibilityDeltaTime)
        {
            // TODO: add any logic we want here
            yield return new WaitForSeconds(invincibilityDeltaTime);
        }

        Debug.Log("Player is no longer invincible!");
        isInvincible = false;
    }
    void Attack(){
        FindObjectOfType<AudioManager>().Play("PlayerAttack");
        if(m_facingDirection == 1){
            Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);
            foreach(Collider2D enemy in hitEnemies){
                Debug.Log("hit");
                if(enemy.GetComponent<EnemyMove>() != null){
                    enemy.GetComponent<EnemyMove>().TakeDamage(attackDamage);
                } else if(enemy.GetComponentInChildren<FlyingEnemySprite>() != null) {
                    enemy.GetComponentInChildren<FlyingEnemySprite>().TakeDamage(attackDamage);
                } else{
                    return;
                }
            }
        } else if(m_facingDirection == -1){
            Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPointBehind.position, attackRange, enemyLayers);
            foreach(Collider2D enemy in hitEnemies){
                Debug.Log("hit behind");
                if(enemy.GetComponent<EnemyMove>() != null){
                    enemy.GetComponent<EnemyMove>().TakeDamage(attackDamage);
                } else if(enemy.GetComponentInChildren<FlyingEnemySprite>() != null) {
                    enemy.GetComponentInChildren<FlyingEnemySprite>().TakeDamage(attackDamage);
                } else{
                    return;
                }
            }
        }
    }
}
