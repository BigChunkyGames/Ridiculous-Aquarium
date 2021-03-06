﻿using UnityEngine;

// the things that all fish have

public class Fish : MonoBehaviour
{
    [Header("Movement Stats")]
    [Range(0,1)]
    [Tooltip("seconds between fishy actions")]
    public float activityFrequency;
    [Range(0,3)]
    public float speed;
    [Range(0,10)]
    public float targetSeekSpeed = 7;
    public Vector3 flipAxis = new Vector3(0,0,1); // z by default

    [Header("Food Stats")]
    [Range(2,30)]
    public float hungerTimer;
    public float growthScaleMultiplier = 1.1f;
    public int timesEatenSinceLastGrowth = 0; // since last growth
    public int foodsNeededToGrow = 4;
    [Tooltip("Additional foods needed to get to next grow level for each prior level")]public int additionalFoodsNeededToGrow = 2;
    public int growthLevel = 0;
    
    [Header("Life Stats")]
    public float maxHealth = 100f;
    public float currentHealth;
    public bool hungry = false;
    public bool Hungry
    {
        get{return this.hungry;}
        set{
            hungry = value;
            if(hungry == false)
            {
                rend.material.SetColor("_Color", startColor);
                CancelInvoke("BecomeHungry");
                CancelInvoke("Starve");
                if(getsHungry) Invoke("BecomeHungry", hungerTimer);  
                starving=false;
            }
            // if becoming hungry
            else
            {
                rend.material.SetColor("_Color", gm.dataStore.fishHungryColor);
                InvokeRepeating("Starve", hungerTimer, 1f);
            }
        }
    }
    public bool dead = false;
    public bool immortal = false;

    [Header("Fish Stats")]
    public bool unique = true;
    private Color gone = new Color(0f, 0f, 0f, 0f);
    private Color startColor;
    public bool getsHungry = true;     
    
    // the thing to rotate
    public GameObject modelContainer;
    public GameObject ModelContainer
    {
        set{
            modelContainer = value;
            Model = modelContainer.transform.Find("Model").gameObject;
        }
    }
    // the thing that renders the fish and holds teh eyes and drop
    [HideInInspector]public GameObject model;
    public GameObject Model
    {
        set{
            this.model = value;
            rend = model.GetComponent<Renderer>();
            startMat = rend.material;
            dropSpot = model.transform.Find("DropSpot");
        }
    }
    [HideInInspector] public Transform dropSpot;

    protected GameObject targetFood = null;
    protected bool starving=false;

    // runtime
    protected GameManager gm;
    protected Renderer rend;
    protected Material startMat;
    protected Rigidbody rb;
    protected AudioSource audioSource;
    protected HealthBar healthBar;
    public GameObject levelUpEffect;

    protected bool seekingFood = false; // prevents random swimming when true
    protected float jumpVelocity = 1;

    private bool facingRight = true;
    private bool fadingAway = false;
    private bool turningAround = false;
    private float timeToFade = 10f;
    protected float uniqueness; // random between -1 and 1
    private float gravity = 0.06f;

    // make sure model is set to look to the right at start
    private Quaternion originalRotation; 
    private Quaternion flippedRotation; 
    private Quaternion rotationToTurnTo;

    // dont let child classes have awake function! use start instead. its just easeir that way
    virtual public void Awake(){
        gm = (GameManager)FindObjectOfType(typeof(GameManager));
        audioSource = GetComponent<AudioSource>();
        rb = GetComponent<Rigidbody>();
        healthBar = GetComponentInChildren<HealthBar>();
        
        if(unique){
            uniqueness = Random.Range(-1f, 1f);
            Uniqueation();
        } else {
            uniqueness = 0f;
        }

        // do the getter
        ModelContainer = modelContainer;
        dropSpot = model.transform.Find("DropSpot");
        Debug.Log(dropSpot);
        healthBar.Initialize(maxHealth);
        currentHealth = maxHealth;
        originalRotation = modelContainer.transform.rotation;
        flippedRotation = originalRotation * Quaternion.Euler(180f*flipAxis.x,180f*flipAxis.y,180f*flipAxis.z);
        startColor = rend.material.color;
          
        if(getsHungry) Invoke("BecomeHungry", hungerTimer);  
    }

    // call this in each child's fixed update
    public void FishFixedUpdate(){
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
        if (turningAround && !dead){
            float turningTime = .1f;
            Quaternion oppositeSide = 
            modelContainer.transform.rotation = Quaternion.Lerp(modelContainer.transform.rotation, rotationToTurnTo,  Time.deltaTime/turningTime);
            if (Time.deltaTime/turningTime >= 1){
                turningAround = false;
            }
        }

        // death
        if(dead){
            if (fadingAway){
                t += Time.deltaTime/timeToFade;
                rend.material.color = Color.Lerp(gm.dataStore.fishDeadColor, gone, t);
            }
            return;
        }
    }
    private float t = 0;

    // swim away from boundries
    public void BeFishy(){
        if (seekingFood){
            // if hungry itll be swimming towards food
            return;
        }

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
        if(speed < 1) speed = 1;
        activityFrequency = activityFrequency + (u);
        if (activityFrequency<=0) activityFrequency = .1f;
        maxHealth+=u;
        gravity+= u;
        hungerTimer += .5f * uniqueness;
        targetSeekSpeed += uniqueness;
        if(GetComponent<FriendlyFish>())
        {
            GetComponent<FriendlyFish>().FriendlyFishUniqueation();
        }
    }

    protected void Drop(GameObject drop){
        Vector3 ds = dropSpot.transform.position;
        Vector3 closerLocation = new Vector3(ds.x, ds.y+2 , gm.dropLayerZ);
        GameObject dropped = Instantiate(drop, closerLocation, drop.transform.rotation);
        Destroy(dropped, 40f);
}
    
    // can be negative to heal
    public void TakeDamage(float damage)
    {
        if(dead) return;
        currentHealth -= damage;
        if(currentHealth <= 0)
        {
            currentHealth = 0f;
            Die();
        }
        else if (currentHealth >= maxHealth)
        {
            currentHealth = maxHealth;
        }
        healthBar.UpdateDisplay(currentHealth/maxHealth);
    }

    void Die(){
        if(immortal) return;
        CancelInvoke();
        dead = true;
        if(model.GetComponent<Outline>()) model.GetComponent<Outline>().OutlineColor = gm.dataStore.fishDeadColor;
        modelContainer.transform.eulerAngles += new Vector3(180f,0f,0f);
        model.GetComponent<Collider>().enabled = false;
        fadingAway = true;
        Invoke("GetDestroyed", timeToFade ); 
        transform.Find("HealthCanvas").gameObject.SetActive(false);
        gm.fishDiedStat++;
        GameObject deathEffect = Instantiate((GameObject)Resources.Load("Prefabs/FX/Death Effect"), this.transform.position, Quaternion.identity);
        deathEffect.transform.parent = this.transform;
        deathEffect.transform.localScale = new Vector3(1,1,1);
        if(GetComponent<EnemyFish>())
        {
            GetComponent<EnemyFish>().EnemyDie();
        }
        else {
            gm.audioManager.PlaySound("Fish Death");
        }
        if(GetComponent<FriendlyFish>())
        {
            GetComponent<FriendlyFish>().FriendlyFishDie();
        }
    }

    void GetDestroyed(){
        Destroy(this.gameObject);
        CancelInvoke(); // not sure if this line gets hit but whatever
    }

    // called by invokes
    public void BecomeHungry(){

        Hungry = true;
        InvokeRepeating("Starve", hungerTimer, .1f);
    }

    public void Starve()
    {
        if(!hungry) CancelInvoke("Starve");
        else{
            starving=true;
            this.TakeDamage(0.5f);
        }
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

    public void GetPoked()
    {
        float x = Random.Range(-1f, 1f);
        float y = Random.Range(-1f, 1f);
        rb.velocity += new Vector3(x,y,0) * speed * 3;
        gm.audioManager.PlaySound("Poke");
    }
}
