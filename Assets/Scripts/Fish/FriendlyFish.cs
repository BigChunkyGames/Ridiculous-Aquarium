using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// seeks food

public class FriendlyFish : Fish
{
    [Header("Friendly Fish Stats")]
    
    [Range(.1f,10)]
    [Tooltip("seconds between drops")]
    public float dropRate = 7f;
    [Range(.1f,10)]
    public float seekingFoodSpeedMultiplier;
    

    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("BeFishy", 0.0f, activityFrequency);
        InvokeRepeating("DropDropable", 1f, dropRate );   // drop dropable
    }

    void FixedUpdate()
    {
        FriendlyFishFixedUpdate();
    }

    // for children to call
    protected void FriendlyFishFixedUpdate()
    {
        base.FishFixedUpdate();
        // seek food
        if(hungry){ 
            if(targetFood == null){
                FindClosestFood();
            }   
            else { // if there is target food
                seekingFood = true;
                // seek food
                transform.position = Vector3.MoveTowards(model.transform.position, targetFood.transform.position, speed*seekingFoodSpeedMultiplier * Time.deltaTime);
                if (targetFood.transform.position.x > transform.position.x){
                    // if food is to the right
                    TurnRight();
                } else {
                    TurnLeft();
                }
                return;
            } 
        }
        seekingFood = false;
    }

    public void DropDropable(){
        //growth   // 0 1 2 3 4 5 6 7 8 9...
        //drops    //   0 1 2 3 4 5
        // count = 6
        if(growthLevel > 0){
            // check that growth level-1 under size of list
            if(growthLevel-1 < gm.dataStore.drops.Count)
            {
                if(gm.dataStore.drops[growthLevel-1] != null)
                {
                    Drop(gm.dataStore.drops[growthLevel-1]);
                }  
                return;
            }
            Drop(gm.dataStore.drops[gm.dataStore.drops.Count-1]);
        }
    }

    void OnCollisionEnter(Collision col){
        if (dead){
            return;
        }
        if((col.gameObject.tag == "Food") && hungry){
            // eating
            Eat(col.gameObject);
        }
        if (col.gameObject.layer == LayerMask.NameToLayer("Boundary") && dead){
            Destroy(this); // destory if hit bottom and dead
        }
    }

    private void Eat(GameObject food)
    {
        currentHealth += food.GetComponent<Food>().healthGain;

        timesEatenSinceLastGrowth++;
        if (timesEatenSinceLastGrowth >= growthLevel * additionalFoodsNeededToGrow + foodsNeededToGrow){
            Grow();
        }
        Destroy(food);
        hungry = false;
        targetFood = null;
        rend.material.SetColor("_Color", Color.white);
        gm.audioManager.PlaySound("Fish Ate");
        Invoke("BecomeHungry", hungerTimer);
    }

    private void Grow(){
        timesEatenSinceLastGrowth = 0;
        growthLevel++;
        float size = transform.localScale.x * growthScaleMultiplier;
        transform.localScale = new Vector3(size, size, size); // grow based on rate * previous size
        audioSource.pitch = 1.5f - .1f * growthLevel;
        audioSource.PlayOneShot(audioSource.clip);
    }

    // assigns the closest food as the target food
    public void FindClosestFood(){
        if (!hungry){
            return;
        }
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

}
