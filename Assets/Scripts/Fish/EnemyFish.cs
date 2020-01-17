using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFish : Fish
{
    public float attackRate;
    [Range(0,100)]
    public float damage;
    public bool alwaysHungry = true;

    private GameObject targetFish = null;
    private bool attacking = false;
    private Fish fishBeingAttacked =  null;

    private void Start() {
        InvokeRepeating("TryToAttack", 1f, attackRate );   // drop dropable
        InvokeRepeating("BeFishyEnemy", 0.0f, activityFrequency);
    }

    void FixedUpdate(){
        base.FishFixedUpdate();
        // if doesnt have a target or target is dead, find another fish
        if(targetFish == null || targetFish.GetComponent<Fish>().dead){
            FindClosestFish();
        }
        else{ // go towards target
            transform.position = Vector3.MoveTowards(transform.position, targetFish.transform.position, speed*2 * Time.deltaTime);
            if (targetFish.transform.position.x > transform.position.x){
                // if food is to the right
                TurnRight();
            } else {
                TurnLeft();
            }
            return;
        }
    }

    // assigns the closest alive fish as the target fish
    public void FindClosestFish(){
        GameObject[] fish = GameObject.FindGameObjectsWithTag("Fish");

        GameObject closest = null;
        float distance = Mathf.Infinity;
        Vector3 position = transform.position;
        foreach (GameObject go in fish)
        {
            //skip dead fish
            if(go.GetComponent<Fish>().dead) continue;
            Vector3 diff = go.transform.position - position;
            float curDistance = diff.sqrMagnitude;
            if (curDistance < distance)
            {
                closest = go;
                distance = curDistance;
            }
        }
        targetFish = closest;
    }

    void TryToAttack()
    {
        // if attacking and there is a fish to attack 
        if(attacking && fishBeingAttacked != null)
        {
            if(fishBeingAttacked.dead)
            {
                return;
            }
            fishBeingAttacked.TakeDamage(damage);
            Debug.Log("doing damage");
        }
    }

    // called once per frame, updates attacking value
    void OnCollisionEnter(Collision col)
    {
        if(col.gameObject.tag == "Fish"){
            attacking = true;
            fishBeingAttacked = col.gameObject.GetComponent<Fish>();
            TryToAttack();
        }
    }

    void OnCollisionExit(Collision col)
    {
        attacking = false;
        fishBeingAttacked = null;
    }

    private void OnCollisionStay(Collision col) {
        if(col.gameObject.tag == "Fish"){
            attacking = true;
            fishBeingAttacked = col.gameObject.GetComponent<Fish>();
        }
    }

    // swim away from boundries
    public void BeFishyEnemy(){
        if (targetFish != null){
            // dont be fishy if seeking target
            return;
        }
        else{
            // do parent fishy stuff
            BeFishy();
        }
    }
    
    private void OnDestroy() {
        gm.combatManager.EnemyWasDestroyed();
    }
}
