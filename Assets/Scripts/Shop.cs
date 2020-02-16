using UnityEngine;
using UnityEngine.UI;
using TMPro;

// handles all the UI

public class Shop : MonoBehaviour
{
    public float spawnedFishDownwardForce = -5f;
    public int fishPrice = 100;
    public int startingFoodCount = 1;
    public int foodCountPrice = 10; // starting prices that change
    public float foodCountPriceIncreaseRate = 10f; // linear
    public int startFeederPrice = 1000;
    public float feederPriceIncreaseRate = 500;
    public int friendlyFishCount = 0; // number of friendly fish alive right now also unused
    
    public GameObject foodToSpawn;
    public int foodUpgradeLevel; // TODO make a button for this

    public GameObject fishButton;
    public GameObject laserButton;
    public GameObject foodButton;
    public GameObject feederButton;

    public GameObject shop;
    public GameObject moneyDisplay;
    public GameObject moneyRateDisplay;
    public GameObject foodsDisplay;
    public GameObject dropDropdown;

    public GameObject fishPriceText;
    public GameObject foodPriceText;
    public GameObject feederPriceText;

    private GameManager gm;
    private Object[] fishMeshes;
    private Object[] fishMats;
    private Transform[] buttons;
    private GameObject[] foodsOnScreen = new GameObject[0];

    public int FoodToSpawnDropdownIndex
    {
        set {
            // if spawning pellet food
            if(value == 0)
            {
                int foodToGet = foodUpgradeLevel / 5;
                // prevent list index error
                if(foodToGet >= gm.dataStore.foods.Count) foodToGet =gm.dataStore.foods.Count-1;
                this.foodToSpawn = gm.dataStore.foods[foodToGet];
            }
            // if dropping turret
            else if (value == 1)
            {
                this.foodToSpawn = gm.dataStore.laserFood;
            }
        }
    }

    // the number of foods it says are on the screen
    private int foodsOnScreenDisplay;
    public int FoodsOnScreenDisplay
    {
        get{return foodsOnScreenDisplay;}
        set{
            foodsOnScreenDisplay = value;
            if(foodsOnScreenDisplay > foodCount) foodsOnScreenDisplay = foodCount;

            foodsDisplay.GetComponent<TMPro.TextMeshProUGUI>().SetText(foodsOnScreenDisplay + "/" + FoodCount);
        }
    }

    private int foodCount;
    public int FoodCount { // amount of food player can add to screen
        get{return foodCount;}
        set{
            foodCount = value;
            
            foodCountPrice = (int)(foodCountPrice + foodCountPriceIncreaseRate);
            foodPriceText.GetComponent<TMPro.TextMeshProUGUI>().SetText("$" + foodCountPrice.ToString());
            foodsDisplay.GetComponent<TMPro.TextMeshProUGUI>().SetText(foodsOnScreenDisplay + "/" + FoodCount);
            feederButton.SetActive(true);

        }
    }

    private int feederPrice;
    public int FeederPrice{
        get { return feederPrice; }
        set {
            feederPrice = value;
            feederPriceText.GetComponent<TMPro.TextMeshProUGUI>().SetText("$" + feederPrice.ToString());
        }
    }

    [HideInInspector] public int feederCount;

    public float startMoney;
    private float money;
    public float Money{
        get{ return money; }
        set
        {
            money = value;
            moneyDisplay.GetComponent<TMPro.TextMeshProUGUI>().SetText("$" + Mathf.RoundToInt(money).ToString());
        }
    }

    private float moneyRate; // money gained per minute
    public float MoneyRate{
        get{ return moneyRate; }
        set
        {
            moneyRate = value;
            moneyRateDisplay.GetComponent<TMPro.TextMeshProUGUI>().SetText("$" + Mathf.RoundToInt(moneyRate).ToString() + " / min");
        }
    }

    void Awake()
    {
        gm = (GameManager)FindObjectOfType(typeof(GameManager));
    }

    void Start(){
        fishMeshes = Resources.LoadAll("Meshes/TropicalFish", typeof(Mesh));
        fishMats = Resources.LoadAll("Meshes/TropicalFish", typeof(Material));

        // make buttons invisible
        foreach (Transform child in this.shop.GetComponent<Transform>())
        {
            child.gameObject.SetActive(false);
        }
        fishButton.SetActive(true);

        fishPriceText.GetComponent<TMPro.TextMeshProUGUI>().SetText("$" + fishPrice.ToString());
        foodPriceText.GetComponent<TMPro.TextMeshProUGUI>().SetText("$" + foodCountPrice.ToString());

        foodToSpawn = gm.dataStore.foods[0];
        Money = startMoney;
        MoneyRate = 0;
        FeederPrice = startFeederPrice;
        FoodCount = 1;
        FoodsOnScreenDisplay = 0;
    }

    void Update()
    {
        Money += moneyRate * Time.deltaTime/ (60f );
    }

    public void BuyRandomFish()
    {
        GameObject fish = (GameObject)Resources.Load("Prefabs/Fish/Generic Fish");
        int rand = (int)Random.Range(0, fishMeshes.Length);
        fish.GetComponentInChildren<MeshFilter>().mesh = (Mesh)fishMeshes[rand];
        fish.GetComponentInChildren<MeshRenderer>().material = (Material)fishMats[rand];
        fish.GetComponentInChildren<MeshCollider>().sharedMesh = null;
        fish.GetComponentInChildren<MeshCollider>().sharedMesh = fish.GetComponentInChildren<MeshFilter>().sharedMesh;
        BuyFish(fish);
    }

    public void BuyLaserFish()
    {
        // TODO multiple laser fish?
        //BuyFish(gm.dataStore.laserFish[0]);
    }

    private void BuyFish(GameObject fish){
        int price = fish.GetComponent<Fish>().price;
        if (AttemptPurchase(price)){
            SpawnFish(fish);
            foodButton.SetActive(true);
            laserButton.SetActive(true);
        }
    }

    public GameObject DropSomethingInTheTank(GameObject toSpawn, bool itsAFish=true, float destroyAfterSeconds = -1f, bool randomX=true)
    {
        // determine transform
        float x = 0f;
        if(randomX) x = Random.Range(gm.leftBoundary, gm.rightBoundary);
        float y = gm.topBoundary + 2f;
        float z = gm.fishLayerZ;
        if(!itsAFish) z = z+2f; // if its not a fish, put it 2 units in back
        Quaternion spawnRotation =  Quaternion.Euler(new Vector3( 0,0, 0));

        GameObject spawned = Instantiate(toSpawn, new Vector3(x, y, z), spawnRotation);
        Rigidbody r = spawned.GetComponent<Rigidbody>();
        // give it downward force
        if(r) r.AddForce(Vector3.up * spawnedFishDownwardForce, ForceMode.VelocityChange);

        if(destroyAfterSeconds>0)
        {
            Destroy(spawned, destroyAfterSeconds);
        }

        return spawned;
    }

    public void SpawnFish(GameObject fish){
        
        GameObject newFish = DropSomethingInTheTank(fish, true);
        
        gm.audioManager.PlaySound("Spawn Fish");
    }

    public void BuyFeeder(){
        if(AttemptPurchase(feederPrice))
        {
            feederCount++;
            Instantiate(gm.dataStore.feeder, new Vector3(2.78f, 20.92f, gm.fishLayerZ), Quaternion.identity);
            FeederPrice = (int)(feederPrice+feederPriceIncreaseRate);
        }
    }

    public void BuyFoodCount(){
        if (AttemptPurchase(foodCountPrice)){
            FoodCount++;
        }
    }

    public bool AttemptPurchase(int cost)
    {
        if (cost <= Money){
            Money -= cost;
            return true;
        }
        else
        {
            Debug.Log("Not enough money!");
            return false;
        }
    }

    // triggered when the player clicks the background and spawns a food
    public void TryToBuyFood(RaycastHit hit)
    {
        foodsOnScreen = GameObject.FindGameObjectsWithTag("Food");
        FoodsOnScreenDisplay = foodsOnScreen.Length;
        if (foodsOnScreen.Length < gm.shop.FoodCount){ // if amount of foods on screen less than the amount allowed
            if(gm.shop.AttemptPurchase(3)) {
                Vector3 spawnLocation = new Vector3(hit.point.x, hit.point.y, gm.fishLayerZ);
                SpawnFood(hit.point);
                gm.audioManager.PlaySound("Spawn Food");
            }
        }
        else{// if amount of foods on screen more than or =to the amount allowed
            gm.audioManager.PlaySound("Food Cap", false, 1f, .8f);
            foodsDisplay.GetComponent<Animation>().Play();
        }
        foodCount = foodCount + 0; // trigger setter
    }

    public GameObject SpawnFood(Vector3 position)
    {
        return Instantiate (foodToSpawn, position, foodToSpawn.transform.rotation);
    }

    

}
