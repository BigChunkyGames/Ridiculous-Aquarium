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

    public Material sickMat;
    public GameObject model;

    // runtime
    private GameObject targetFood = null;
    Renderer rend;
    private Material startMat;
    Rigidbody rb;
    Jump jump;

    private bool facingRight = true;
    

    void Awake(){
        rend = model.GetComponent<Renderer>();
        startMat = rend.material;
        rb = GetComponent<Rigidbody>();
        jump = GetComponent<Jump>();
        InvokeRepeating("BeFishy", 0.0f, 1f - activity);  
        InvokeRepeating("BecomeHungry", 0f, 5f); 
        InvokeRepeating("FindClosestFood", 0f, 1f);   // search for food every second

    }

    void FixedUpdate(){
        rb.AddForce(Physics.gravity * rb.mass * gravity); // gravity

        // seek food
        if(hungry){
            if (targetFood != null){ // if there is food
                transform.position = Vector3.MoveTowards(transform.position, targetFood.transform.position, speed*2 * Time.deltaTime);
                return;
            }
        }

        if (transform.position.y < 2f){
            jump.JumpNow();
        }   
        if (transform.position.x < -8f){
            TurnRight();
            GoForward();
            return;
        }
        if (transform.position.x > 8f){
            TurnLeft();
            GoForward();
            return;
        }
    }

    void OnCollisionEnter(Collision col){
        if(col.gameObject.tag == "Food" && hungry){
            Destroy(col.gameObject);
            hungry = false;
            targetFood = null;
            rend.material = startMat;
        }
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
        rend.material = sickMat;
        hungry = true;
    }

    public void BeFishy(){
        float rand = Random.Range(0f, 10.0f);

        if (transform.position.y > 14f){ // if too high just wait 
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

        
        if (rand < 10f){
            if (transform.position.y < 7f){ // if too high just wait 
                jump.JumpNow();
            }
            return;

        }
        
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
