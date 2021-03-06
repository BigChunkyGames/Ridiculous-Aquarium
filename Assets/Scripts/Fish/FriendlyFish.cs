﻿using UnityEngine;

public enum FishTypeEnum {generic,laser};

public class FriendlyFish : Fish
{
    [Header("Friendly Fish Stats")]
    
    [Range(.1f,20)]
    [Tooltip("seconds between drops")]
    public float treasureDropRate = 14f;
    [Range(.1f,10)]
    private int passiveIncomePerMinute = 15; 
    public int PassiveIncomePerMinute{
        set{
            gm.shop.PassiveIncome -= this.passiveIncomePerMinute;
            this.passiveIncomePerMinute = value;
            gm.shop.PassiveIncome += this.passiveIncomePerMinute;
        }
    }

    [Header("Specialization")]
    public FishTypeEnum fishType;
    public FishTypeEnum FishType{
        set{
            // if changing to laser fish
            this.fishType = value;
            if(value == FishTypeEnum.laser)
            {
                // BECOME LASER FISH
                CancelInvoke("UpdateTarget");
                this.eyes = model.transform.Find("Eyes").transform;
                this.laser = gm.weaponEffects.RegisterLaser(0);
                model.GetComponent<Outline>().OutlineColor = Color.red;
                InvokeRepeating("UpdateTarget", .2f, .3f);
            }
        }
    }
    public float damageDealtPerFrame = 0.1f;

    ///////////////// laser fish stuff
    private LineRenderer laser;
    private GameObject targetEnemy; // enemy
    private Transform eyes;
    private int lasersEaten = 0;

    void AttackTarget()
    {
        laser.enabled = true;
        gm.weaponEffects.AttackWithLaser(eyes.position, targetEnemy.transform.position, laser);
        targetEnemy.GetComponent<Fish>().TakeDamage(damageDealtPerFrame);
    }

        // assigns target as closest enemy
    void UpdateTarget()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        if(enemies.Length == 0)
        {
            targetEnemy = null;
            return;
        }
        GameObject closest = null;
        float distance = Mathf.Infinity;
        Vector3 position = transform.position;
        foreach (GameObject go in enemies)
        {
            // don't include dead fish
            if(go.GetComponent<Fish>().dead) continue;
            Vector3 diff = go.transform.position - position;
            float curDistance = diff.sqrMagnitude;
            if (curDistance < distance)
            {
                closest = go;
                distance = curDistance;
            }
        }
        targetEnemy = closest;
    }

    ////////////////////////////////////////

    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("BeFishy", 0.0f, activityFrequency);
        InvokeRepeating("DropTreasure", 1f, treasureDropRate );   // drop dropable
        InvokeRepeating("FindClosestFood", 1f, .1f);
        gm.shop.FriendlyFishCount++;
        gm.shop.PassiveIncome += this.passiveIncomePerMinute;
        this.FishType = this.fishType; // trigger setter
    }

    void OnDestroy()
    {
        if(laser!=null)Destroy(laser.gameObject);
        gm.shop.PassiveIncome -= this.passiveIncomePerMinute;
    }

    public void FriendlyFishDie()
    {
        gm.shop.FriendlyFishCount--;
    }

    void FixedUpdate()
    {
        FriendlyFishFixedUpdate();

        // if laser fish
        if(this.fishType == FishTypeEnum.laser)
            {
                // if there is a target and fish isnt hungry and isnt dead and target is alive
            if(targetEnemy != null && !Hungry && !dead) 
            {
                AttackTarget();
            }
            else
            {
                laser.enabled = false;
            }
        }
    }

    // for children to call
    protected void FriendlyFishFixedUpdate()
    {
        base.FishFixedUpdate();
        // seek food
        if(Hungry){ 
            if(targetFood == null){
                return;
            }   
            else { // if there is target food
                seekingFood = true;
                // seek food
                transform.position = Vector3.MoveTowards(model.transform.position, targetFood.transform.position, targetSeekSpeed * Time.deltaTime);
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

    public void DropTreasure(){
        //growth   // 0 1 2 3 4 5 6 7 8 9...
        //drops    //   0 1 2 3 4 5
        // count = 6

        if(growthLevel > 0){
            // check that growth level-1 under size of list
            if(growthLevel-1 < gm.dataStore.treasures.Count)
            {
                if(gm.dataStore.treasures[growthLevel-1] != null)
                {
                    Drop(gm.dataStore.treasures[growthLevel-1]);
                    //Debug.Log("Dropping #" + (growthLevel-1));
                }  
                return;
            }
            Drop(gm.dataStore.treasures[gm.dataStore.treasures.Count-1]);
        }
    }

    void OnCollisionStay(Collision col){
        if (dead){
            return;
        }
        if((col.gameObject.tag == "Food") && Hungry){
            // eating
            Eat(col.transform.parent.gameObject);
        }
        if (col.gameObject.layer == LayerMask.NameToLayer("Boundary") && dead){
            Destroy(this.gameObject); // destory if hit bottom and dead
        }
        // if another fish is trying to get food, swim at right angle between them and food
        if (col.gameObject.GetComponent<FriendlyFish>()){
            FriendlyFish f = col.gameObject.GetComponent<FriendlyFish>();
            // if you have a target and i dont, i get pushed out of the way
            if (f.targetFood != null && !targetFood){
                Vector3 vectorToFoodFromSeekingFish = f.transform.position - f.targetFood.transform.position;
                Vector3 dirToSwim = Quaternion.Euler(0, 0,90) * vectorToFoodFromSeekingFish.normalized * .1f;
                GetComponent<Rigidbody>().velocity += dirToSwim;
            }
        }
    }

    private void Eat(GameObject food)
    {
        Hungry=false;
        currentHealth += food.GetComponent<Food>().healthGain;
        healthBar.UpdateDisplay(currentHealth/maxHealth);
        if(currentHealth >= maxHealth) 
        {
            currentHealth = maxHealth;
        }
        timesEatenSinceLastGrowth+= food.GetComponent<Food>().nourishment;
        if (timesEatenSinceLastGrowth >= growthLevel * additionalFoodsNeededToGrow + foodsNeededToGrow){
            Grow();
        }
        Destroy(food);
        targetFood = null;
        gm.audioManager.PlaySound("Fish Ate");
        
        // change type
        if(food.GetComponent<Food>().foodType == FoodTypeEnum.laser)
        {
            lasersEaten++;
            this.PassiveIncomePerMinute = gm.scalingManager.ScaleFishPassiveIncome(growthLevel, true);
            this.FishType = FishTypeEnum.laser;
            damageDealtPerFrame = gm.scalingManager.ScaleLaserFishDPF(lasersEaten);
        }
    }

    private void Grow(){
        timesEatenSinceLastGrowth = 0;
        growthLevel++;

        // change stats
        this.PassiveIncomePerMinute = gm.scalingManager.ScaleFishPassiveIncome(growthLevel);
        float size = transform.localScale.x * growthScaleMultiplier;
        transform.localScale = new Vector3(size, size, size); // grow based on rate * previous size
        this.speed -= this.speed * .08f; // get slower
        this.targetSeekSpeed -= this.targetSeekSpeed * .08f;
        audioSource.pitch = 2.0f - .15f * growthLevel;
        maxHealth += 10;
        currentHealth += 10;
        audioSource.PlayOneShot(audioSource.clip);
        GameObject effect = Instantiate(levelUpEffect, this.transform.position, Quaternion.identity);
        Destroy(effect, 3);

    }

    // assigns the closest food as the target food
    public void FindClosestFood(){
        if (!Hungry){
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

    public void FriendlyFishUniqueation(){
        treasureDropRate += 2 * uniqueness;
        damageDealtPerFrame += uniqueness * .01f;
    }

}
