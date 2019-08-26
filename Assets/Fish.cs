using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fish : MonoBehaviour
{
    // assignable
    [Range(1,10)]
    public float speed;
    [Range(0,1)]
    public float gravity;
    [Range(0,1)]
    public float activity;
    public bool hungry = false;
    public float hungerTimer;
    public int timesEatenSinceLastGrowth = 0; // since last growth
    public int foodsNeededToGrow = 5;
    public int growthLevel = 1;
    public float amountToGrow = .5f;
    public float dropRate = 1f;

    public Material sickMat;
    public Material deadMat;
    public GameObject model;
    public GameObject dropable; // coin
    public GameObject dropSpot;

    // runtime
    GameManager gm;
    Renderer rend;
    Material startMat;
    Rigidbody rb;
    Jump jump;

    private bool facingRight = true;private float uniqueness;
    private GameObject targetFood = null;
    private bool dead = false;

    void Awake(){
        gm = (GameManager)FindObjectOfType(typeof(GameManager));
        uniqueness = Random.Range(-1f, 1f);
        rend = model.GetComponent<Renderer>();
        startMat = rend.material;
        rb = GetComponent<Rigidbody>();
        jump = GetComponent<Jump>();
        InvokeRepeating("BeFishy", 0.0f, 1f - activity);  
        InvokeRepeating("BecomeHungry", hungerTimer + uniqueness*2f, hungerTimer + uniqueness* 2); 
        InvokeRepeating("FindClosestFood", 0f, 0.5f );   // search for food every second
        InvokeRepeating("DropDropable", 0f, dropRate );   // drop dropable

    }

    void FixedUpdate(){
        rb.AddForce(Physics.gravity * rb.mass * gravity); // gravity
        if(dead){
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

    public void DropDropable(){
        if(growthLevel == 2){
            Instantiate(dropable, dropSpot.transform.position, dropable.transform.rotation);
        }
    }

    public void BeFishy(){
        if (hungry){
            return;
        }
        if (transform.position.y < gm.bottomBoundary){
            jump.JumpNow(); // if too low, go up
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
            jump.JumpNow();
            GoForward();
        }
        if (rand < 10f){
            if (transform.position.y < 7f){ 
                jump.JumpNow();
            }
            return;
        }
    }

    void Die(){
        dead = true;
        rend.material = deadMat;
        transform.rotation *= Quaternion.Euler(0,0,180f);
        CancelInvoke();
    }

    void OnCollisionEnter(Collision col){
        if (dead){
            return;
        }
        if(col.gameObject.tag == "Food" && hungry){
            // eating
            timesEatenSinceLastGrowth++;
            if (timesEatenSinceLastGrowth % foodsNeededToGrow == 0){
                Grow();
            }
            Destroy(col.gameObject);
            hungry = false;
            targetFood = null;
            rend.material = startMat;
        }
    }

    private void Grow(){
        timesEatenSinceLastGrowth = 0;
        growthLevel++;
        transform.localScale += new Vector3(amountToGrow, amountToGrow, amountToGrow); // grow linearly
    }

    public void FindClosestFood(){
        GameObject[] foods = GameObject.FindGameObjectsWithTag("Food");
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
        rend.material = sickMat;
        hungry = true;
    }

    public void TurnAround(){
        transform.rotation *= Quaternion.Euler(0,180f,0);
        if(facingRight){
            facingRight = false;
        } else {
            facingRight = true;
        }
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
        rb.velocity += transform.forward * speed;
    }
}
