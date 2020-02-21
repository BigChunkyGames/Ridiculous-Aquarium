using UnityEngine;

public enum FishTypeEnum {generic,laser};

public class FriendlyFish : Fish
{
    [Header("Friendly Fish Stats")]
    
    [Range(.1f,10)]
    [Tooltip("seconds between drops")]
    public float dropRate = 7f;
    [Range(.1f,10)]
    public float passiveIncomePerMinute = 15; // TODO change this depending on stuff

    [Header("Specialization")]
    public FishTypeEnum fishType = FishTypeEnum.generic;
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
                InvokeRepeating("UpdateTarget", 1.0f, .3f);
            }
        }
    }
    public float damageDealtPerFrame = 0.1f;

    ///////////////// laser fish stuff
    private LineRenderer laser;
    private GameObject targetEnemy; // enemy
    private Transform eyes;

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
        InvokeRepeating("DropDropable", 1f, dropRate );   // drop dropable
        gm.shop.FriendlyFishCount++;
        gm.shop.MoneyRate += this.passiveIncomePerMinute;
    }

    void OnDestroy()
    {
        if(laser!=null)Destroy(laser.gameObject);
        gm.shop.FriendlyFishCount--;
        gm.shop.MoneyRate -= this.passiveIncomePerMinute;
    }

    void FixedUpdate()
    {
        FriendlyFishFixedUpdate();

        // if laser fish
        if(this.fishType == FishTypeEnum.laser)
            {
                // if there is a target and fish isnt hungry and isnt dead and target is alive
            if(targetEnemy != null && !Hungry && !dead && !targetEnemy.GetComponent<Fish>().dead) 
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
                FindClosestFood();
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
                    Debug.Log("Dropping #" + (growthLevel-1));

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
        if((col.gameObject.tag == "Food") && Hungry){
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
        if(currentHealth > maxHealth) currentHealth = maxHealth;

        timesEatenSinceLastGrowth++;
        if (timesEatenSinceLastGrowth >= growthLevel * additionalFoodsNeededToGrow + foodsNeededToGrow){
            Grow();
        }
        Destroy(food);
        Hungry = false;
        targetFood = null;
        
        gm.audioManager.PlaySound("Fish Ate");
        
        // change type
        if(food.GetComponent<Food>().foodType == FoodTypeEnum.laser)
        {
            this.FishType = FishTypeEnum.laser;
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
        if (!Hungry){
            return;
        }
        GameObject[] foods = GameObject.FindGameObjectsWithTag("Food");

        GameObject closest = null;
        float distance = Mathf.Infinity;
        Vector3 position = transform.position;
        foreach (GameObject go in foods)
        {
            if(fishType == FishTypeEnum.laser && go.GetComponent<Food>().foodType == FoodTypeEnum.laser)
            {
                // laser fish dont want more laser food
                continue;
            }
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
