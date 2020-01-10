using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

// the things that all fish have

public class Fish : MonoBehaviour
{
    [Header("All Fish Stats")]
    [Range(0,1)]
    [Tooltip("seconds between fishy actions")]
    public float activityFrequency;
    [Range(0,3)]
    public float speed;
    [Range(2,30)]
    public float hungerTimer;
    
    public float startHealth = 100f;
    public float currentHealth;
    public bool unique = true;
    public bool hungry = false;
    public bool dead = false;
    public bool immortal = false;
    public float growthScaleMultiplier = 1.1f;
    public float dropLifetime = 10f;
    public int price = 100;

    public Color hungryColor;
    public Color deadColor;
    
    public GameObject model;
    protected GameObject dropSpot;
    protected GameObject targetFood = null;

    // runtime
    protected GameManager gm;
    protected Renderer rend;
    protected Material startMat;
    protected Rigidbody rb;
    protected AudioSource audioSource;
    protected HealthBar healthBar;

    protected bool seekingFood = false; // prevents random swimming when true
    protected float gravity = 3;
    protected float jumpVelocity = 1;
    [HideInInspector] protected bool justAte = false;

    public int timesEatenSinceLastGrowth = 0; // since last growth
    public int foodsNeededToGrow = 4;
    [Tooltip("Additional foods needed to get to next grow level for each prior level")]public int additionalFoodsNeededToGrow = 2;
    public int growthLevel = 0;

    private bool facingRight = true;
    private bool fadingAway = false;
    private bool turningAround = false;
    private float timeToFade = 10f;
    private float uniqueness; // random between -1 and 1

    // make sure model is set to look to the right at start
    private Quaternion originalRotation; 
    private Quaternion flippedRotation; 
    private Quaternion rotationToTurnTo;

// dont let child classes have awake function! use start instead. its just easeir that way
    virtual public void Awake(){
        gm = (GameManager)FindObjectOfType(typeof(GameManager));
        audioSource = GetComponent<AudioSource>();
        rb = GetComponent<Rigidbody>();
        rend = model.GetComponent<Renderer>();
        healthBar = GetComponentInChildren<HealthBar>();
        
        if(unique){
            uniqueness = Random.Range(-1f, 1f);
            Uniqueation();
        } else {
            uniqueness = 0f;
        }

        healthBar.Initialize(startHealth);
        currentHealth = startHealth;
        startMat = rend.material;
        originalRotation = model.transform.rotation;
        flippedRotation = originalRotation * Quaternion.Euler(0f,0f,180f);
          
        InvokeRepeating("BecomeHungry", hungerTimer, hungerTimer);  
    }

    void FixedUpdate(){
        // contain within boundaries
        if(this.transform.position.y > gm.topBoundary)
        {
            this.transform.position = new Vector3(this.transform.position.x,gm.topBoundary, this.transform.position.z);
        }
        if(this.transform.position.x < gm.leftBoundary)
        {
            this.transform.position = new Vector3(gm.leftBoundary, this.transform.position.y, this.transform.position.z);
        }
        if(this.transform.position.x > gm.rightBoundary)
        {
            this.transform.position = new Vector3(gm.rightBoundary, this.transform.position.y, this.transform.position.z);
        }

        // gravity
        rb.AddForce(Physics.gravity * rb.mass * gravity); 

        // rotation
        if (turningAround){
            Debug.Log("test");

            float turningTime = .1f;
            Quaternion oppositeSide = 
            model.transform.rotation = Quaternion.Lerp(model.transform.rotation, rotationToTurnTo,  Time.deltaTime/turningTime);
            if (Time.deltaTime/turningTime >= 1){
                turningAround = false;
            }
        }

        // death
        if(dead){
            if (fadingAway){
                //t += Time.deltaTime / 5.0f;
                Color gone = new Color(1f, 1f, 1f, 0f);
                rend.material.color = Color.Lerp(rend.material.color, gone,  Time.deltaTime/timeToFade);
            }
            return;
        }
    }

    // swim away from boundries
    public void BeFishy(){
        if (seekingFood){
            // if hungry itll be swimming towards food
            return;
        }

        if(gm.topBoundary != 0) Debug.Log("test2");

        int padding = 1;
        if (transform.position.y > gm.topBoundary - padding){ // if too high jump down
            JumpNow(true);
            return;
        }
        if (transform.position.y < gm.bottomBoundary){
            JumpNow(); // if too low, go up
            return;
        } 
        if (transform.position.x < gm.leftBoundary + padding){
            TurnRight();
            GoForward();
            return;
        }
        if (transform.position.x > gm.rightBoundary - padding){
            TurnLeft();
            GoForward();
            return;
        }

    // swim around randomly
        float rand = Random.Range(0f, 10.0f);
        if (rand < 1f ){
            TurnAround();
            GoForward();
            return;
        }
        if (rand < 5f){
            GoForward();
            return;
        }
        if (rand < 6.5f){
            JumpNow();
            GoForward();
        }
        if (rand < 10f){
            if (transform.position.y < 7f){ 
                JumpNow();
            }
            return;
        }
    }


    void Uniqueation(){
        float u = .1f* uniqueness;
        float scalar = transform.localScale.x * u + transform.localScale.x;
        transform.localScale = new Vector3(scalar,scalar,scalar);
        jumpVelocity = jumpVelocity + u;
        speed = speed + u;
        activityFrequency = activityFrequency + (u);
        if (activityFrequency<=0) activityFrequency = .1f;
        startHealth+=u;
    }

    protected void Drop(GameObject drop){
        Vector3 ds = dropSpot.transform.position;
        Vector3 closerLocation = new Vector3(ds.x, ds.y+2 , gm.dropLayerZ);
        GameObject dropped = Instantiate(drop, closerLocation, drop.transform.rotation);
        Destroy(dropped, dropLifetime);
    }
    
    public void TakeDamage(float damage)
    {
        if(dead) return;
        currentHealth -= damage;
        if(currentHealth <= 0)
        {
            currentHealth = 0f;
            Die();
        }
        healthBar.UpdateDisplay(currentHealth/startHealth);
    }

    void Die(){
        if(immortal) return;
        CancelInvoke();
        dead = true;
        rend.material.SetColor("_Color", deadColor);
        model.transform.eulerAngles += new Vector3(180f,0f,0f);
        fadingAway = true;
        InvokeRepeating("GetDestroyed", timeToFade, timeToFade ); 
    }

    void GetDestroyed(){
        Destroy(this.gameObject);
        CancelInvoke(); // not sure if this line gets hit but whatever
    }

    public void BecomeHungry(){
        if(justAte)
        {
            justAte = false;
            return;
        }
        if (hungry){
            Die();
        }
        rend.material.SetColor("_Color", hungryColor);
        hungry = true;
    }

    public void TurnAround(){

        if(facingRight){
            facingRight = false;
            rotationToTurnTo = flippedRotation;
        } else {
            facingRight = true;
            rotationToTurnTo = originalRotation;
        }
        turningAround = true;
    }

    public void TurnLeft(){
        if(facingRight){
            TurnAround();
        }
    }

    public void TurnRight(){
        if(!facingRight){
            TurnAround();
        }
    }

    public void GoForward(){
        if (!facingRight){
            rb.velocity += Vector3.left * speed;
        } else {
            rb.velocity += Vector3.right * speed;
        }
    }

    public void JumpNow(bool jumpDownwards = false){
        int mult = 1;
        if(jumpDownwards) mult = -1;
        rb.velocity += Vector3.up * jumpVelocity * mult;
    }
}
