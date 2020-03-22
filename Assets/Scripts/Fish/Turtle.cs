using UnityEngine;

public class Turtle : Fish
{
    [Header("Turtle Stats")]
    
    private GameObject targetTreasure;
    public int treasuresNeededToGrow = 2;
    public int growRequirementIncrease = 3;
    public int treasuresConsumedSinceLastGrowth =0;
    public int level;

    void Start()
    {
        InvokeRepeating("BeFishy", 0.0f, activityFrequency);
        InvokeRepeating("PassiveHeal", 0.0f, 1f);
        InvokeRepeating("UpdateTarget", 0.0f, 1f);
        gm.shop.TurtleCount++;
    }

    void PassiveHeal(){
        if(currentHealth < maxHealth)
        {
            TakeDamage(-1);
        }
    }

    void UpdateTarget()
    {
        GameObject[] treasures = GameObject.FindGameObjectsWithTag("Treasure");
        if(treasures.Length == 0)
        {
            targetTreasure = null;
            return;
        }
        GameObject closest = null;
        float distance = Mathf.Infinity;
        Vector3 position = transform.position;
        foreach (GameObject go in treasures)
        {
            Vector3 diff = go.transform.position - position;
            float curDistance = diff.sqrMagnitude;
            if (curDistance < distance)
            {
                closest = go;
                distance = curDistance;
            }
        }
        targetTreasure = closest;
    }

    void OnDestroy()
    {
        gm.shop.TurtleCount--;
    }

    void FixedUpdate()
    {
        TurtleFixedUpdate();
    }

    // for children to call
    protected void TurtleFixedUpdate()
    {
        base.FishFixedUpdate();
        // seek food
        if(targetTreasure){
             // if there is target food
            transform.position = Vector3.MoveTowards(model.transform.position, targetTreasure.transform.position, targetSeekSpeed * Time.deltaTime);
            if (targetTreasure.transform.position.x > transform.position.x){
                // if food is to the right
                TurnRight();
            } else {
                TurnLeft();
            }
            return;
        } 
    }

    void OnCollisionEnter(Collision col){
        if (dead){
            return;
        }
        if((col.gameObject.tag == "Treasure")){
            // eating
            gm.playerInput.PickupTreasure(col.transform);
            treasuresConsumedSinceLastGrowth++;
            if(treasuresConsumedSinceLastGrowth >= treasuresNeededToGrow){
                // grow
                transform.localScale *= 1.08f;
                treasuresConsumedSinceLastGrowth = 0;
                treasuresNeededToGrow += growRequirementIncrease;
                targetSeekSpeed *= .95f;
                Destroy(Instantiate(levelUpEffect, transform.position, Quaternion.identity),5);
            }
        }
        if (col.gameObject.layer == LayerMask.NameToLayer("Boundary") && dead){
            Destroy(this); // destory if hit bottom and dead
        }
    }

}
