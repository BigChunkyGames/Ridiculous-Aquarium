using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FriendlyFish : Fish
{
    [Header("Normal Fish Stats")]
    
    [Range(.1f,10)]
    [Tooltip("seconds between drops")]
    public float dropRate = 7f;
    

    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("BeFishy", 0.0f, activityFrequency);
        InvokeRepeating("DropDropable", 1f, dropRate );   // drop dropable
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // seek food
        if(hungry){ 
            if(targetFood == null){
                FindClosestFood();
            }   
            else { // if there is target food
                seekingFood = true;
                transform.position = Vector3.MoveTowards(model.transform.position, targetFood.transform.position, speed*2 * Time.deltaTime);
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
            if(growthLevel-1 < gm.drops.Count)
            {
                if(gm.drops[growthLevel-1] != null)
                {
                    Drop(gm.drops[growthLevel-1]);
                }  
                return;
            }
            Drop(gm.drops[gm.drops.Count-1]);
        }
    }

    
    void OnCollisionEnter(Collision col){
        if (dead){
            return;
        }
        if((col.gameObject.tag == "Food") && hungry){
            // eating
            timesEatenSinceLastGrowth++;
            if (timesEatenSinceLastGrowth >= growthLevel * additionalFoodsNeededToGrow + foodsNeededToGrow){
                Grow();
            }
            Destroy(col.gameObject);
            hungry = false;
            targetFood = null;
            rend.material.SetColor("_Color", Color.white);
            gm.audioManager.PlaySound("Fish Ate");
        }
        if (col.gameObject.layer == LayerMask.NameToLayer("Boundary") && dead){
            Destroy(this); // destory if hit bottom and dead
        }
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
