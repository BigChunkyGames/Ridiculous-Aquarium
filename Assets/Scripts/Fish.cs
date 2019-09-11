using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Fish : MonoBehaviour
{
    // assignable
    [Range(1,10)]
    public float speed;
    [Range(0,1)]
    public float gravity;
    [Range(0,1)]
    public float activity;
    [Range(1,10)]
    public float jumpVelocity;
    [Range(.1f,10)]
    public float dropRate = 7f;
    public bool hungry = false;
    public float hungerTimer;
    public int timesEatenSinceLastGrowth = 0; // since last growth
    public int foodsNeededToGrow = 4;
    [Tooltip("Additional foods needed to get to next grow level for each prior level")]public int additionalFoodsNeededToGrow = 2;
    public int growthLevel = 1;
    public float growRate = 1.1f;
    public float dropLifetime = 10f;
    public float price = 100f;
    public bool unique = true;
    private float timeToFade = 10f;
    
    public GameObject model;
    public GameObject dropSpot;

    // runtime
    GameManager gm;
    Renderer rend;
    Material startMat;
    Rigidbody rb;

    private bool facingRight = true;
    private float uniqueness;
    private GameObject targetFood = null;
    public bool dead = false;
    private bool fadingAway = false;
    private bool turningAround = false;
    private Quaternion originalRotation; 
    private Quaternion flippedRotation; 
    private Quaternion rotationToTurnTo;
    private float t = 0f; // keeps track of time sometimes


    void Awake(){
        gm = (GameManager)FindObjectOfType(typeof(GameManager));
        if(unique){
            uniqueness = Random.Range(-1f, 1f);
            Uniqueation();
        } else {
            uniqueness = 0f;
        }
        rend = model.GetComponent<Renderer>();
        startMat = rend.material;
        rb = GetComponent<Rigidbody>();
        originalRotation = transform.rotation;
        flippedRotation = originalRotation * Quaternion.Euler(0f,180f,0f);
        InvokeRepeating("BeFishy", 0.0f, 1f - activity);  
        InvokeRepeating("BecomeHungry", hungerTimer + uniqueness*2f, hungerTimer + uniqueness* 2f); 
        InvokeRepeating("FindClosestFood", 0f, 0.1f );   // search for food every second
        InvokeRepeating("DropDropable", 0f, dropRate );   // drop dropable
    }

    void FixedUpdate(){
        rb.AddForce(Physics.gravity * rb.mass * gravity); // gravity
        if (turningAround){
            float turningTime = .1f;
            Quaternion oppositeSide = 
            transform.rotation = Quaternion.Lerp(transform.rotation, rotationToTurnTo,  Time.deltaTime/turningTime);
            if (Time.deltaTime/turningTime >= 1){
                turningAround = false;
            }
        }
        if(dead){
            if (fadingAway){
                //t += Time.deltaTime / 5.0f;
                Color gone = new Color(1f, 1f, 1f, 0f);
                rend.material.color = Color.Lerp(rend.material.color, gone,  Time.deltaTime/timeToFade);
            }
            return;
        }
        // seek food
        if(hungry){ 
            if (targetFood != null){ // if there is food
                transform.position = Vector3.MoveTowards(transform.position, targetFood.transform.position, speed*2 * Time.deltaTime);
                if (targetFood.transform.position.x > transform.position.x){
                    // if food is to the right
                    TurnRight();
                } else {
                    TurnLeft();
                }
                return;
            } 
        }
        
    }

    void Uniqueation(){
        float u = .1f* uniqueness;
        float scalar = transform.localScale.x * u + transform.localScale.x;
        transform.localScale = new Vector3(scalar,scalar,scalar);
        jumpVelocity = jumpVelocity + u;
        speed = speed + u;
    }

    public void DropDropable(){
        if(growthLevel == 2){
            Drop(gm.coin);
        }
        
    }

    private void Drop(GameObject drop){
        Vector3 ds = dropSpot.transform.position;
        Vector3 closerLocation = new Vector3(ds.x, ds.y , ds.z - 3f);
        GameObject dropped = Instantiate(drop, closerLocation, drop.transform.rotation);
        Destroy(dropped, dropLifetime);
    }

    public void BeFishy(){
        if (hungry){
            return;
        }
        if (transform.position.y < gm.bottomBoundary){
            JumpNow(); // if too low, go up
            return;
        } 
        if (transform.position.x < gm.leftBoundary){
            TurnRight();
            GoForward();
            return;
        }
        if (transform.position.x > gm.rightBoundary){
            TurnLeft();
            GoForward();
            return;
        }

        float rand = Random.Range(0f, 10.0f);
         
        if (transform.position.y > gm.topBoundary){ // if too high just wait 
            return;
        }
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

    void Die(){
        dead = true;
        rend.material.SetColor("_Color", gm.deadColor);
        transform.rotation *= Quaternion.Euler(180f,0f,0f);
        CancelInvoke();
        fadingAway = true;
        InvokeRepeating("GetDestroyed", timeToFade, timeToFade ); 
    }

    void GetDestroyed(){
        Destroy(this.gameObject);
        CancelInvoke(); // not sure if this line gets hit but whatever
    }

    void OnCollisionEnter(Collision col){
        if (dead){
            return;
        }
        if((col.gameObject.tag == "Food" || col.gameObject.tag == "Feeder Food") && hungry){
            // eating
            timesEatenSinceLastGrowth++;
            if (timesEatenSinceLastGrowth >= growthLevel * additionalFoodsNeededToGrow + foodsNeededToGrow){
                Grow();
            }
            Destroy(col.gameObject);
            hungry = false;
            targetFood = null;
            rend.material.SetColor("_Color", Color.white);
        }
        if (col.gameObject.layer == LayerMask.NameToLayer("Boundary") && dead){
            Destroy(this); // destory if hit bottom and dead
        }
    }

    private void Grow(){
        timesEatenSinceLastGrowth = 0;
        growthLevel++;
        float size = transform.localScale.x * growRate;
        transform.localScale = new Vector3(size, size, size); // grow based on rate * previous size
    }

    public void FindClosestFood(){
        if (!hungry){
            return;
        }
        GameObject[] foods = GameObject.FindGameObjectsWithTag("Food");
        GameObject[] foods2 = GameObject.FindGameObjectsWithTag("Feeder Food");
        foods = foods.Concat(foods2).ToArray();

        GameObject closest = null;
        float distance = Mathf.Infinity;
        Vector3 position = transform.position;
        foreach (GameObject go in foods)
        {
            Vector3 diff = go.transform.position - position;
            float curDistance = diff.sqrMagnitude;
            if (curDistance < distance)
            {
                closest = go;
                distance = curDistance;
            }
        }
        targetFood = closest;
    }

    public void BecomeHungry(){
        if (hungry){
            Die();
        }
        rend.material.SetColor("_Color", gm.hungryColor);
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
    public void JumpNow(){
        rb.velocity += Vector3.up * jumpVelocity;
    }
}
