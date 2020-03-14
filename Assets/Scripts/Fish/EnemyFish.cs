using UnityEngine;

public class EnemyFish : Fish
{
    
    [Header("Enemy Fish Stats")]
    public float attackRate; // seconds between attacks
    [Range(0,100)]
    public float damage;
    public bool alwaysHungry = true; // UNUSED
    public int enemyLevel = 1;
    public int dropLevel;


    private GameObject targetFish = null;
    private bool attacking = false;
    private Fish fishBeingAttacked =  null;

    private void Start() {
        InvokeRepeating("TryToAttack", 1f, attackRate );  
        InvokeRepeating("BeFishyEnemy", 0.0f, activityFrequency);
    }

    void FixedUpdate(){
        base.FishFixedUpdate();
        if(dead) {
            CancelInvoke("TryToAttack");
            CancelInvoke("BeFishyEnemy");
            return;
        }
        // if doesnt have a target or target is dead, find another fish
        if(targetFish == null || targetFish.GetComponent<Fish>().dead){
            FindClosestFish();
        }
        else{ // go towards target
            transform.position = Vector3.MoveTowards(transform.position, targetFish.transform.position, targetSeekSpeed * Time.deltaTime);
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
                Hungry = false;
                return;
            }
            fishBeingAttacked.TakeDamage(damage);
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
    
    public void EnemyDie() {
        gm.combatManager.EnemyWasDestroyed();
        //Drop dropable here or it won't work
        GameObject drop = gm.dataStore.drops[dropLevel];
        Vector3 ds = dropSpot.transform.position;
        Vector3 closerLocation = new Vector3(ds.x, ds.y+2 , gm.dropLayerZ);
        GameObject dropped = Instantiate(drop, closerLocation, drop.transform.rotation);
        Destroy(dropped, dropLifetime);
    }
}
