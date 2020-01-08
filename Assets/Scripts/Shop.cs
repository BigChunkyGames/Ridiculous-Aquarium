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

    public GameObject fishButton;
    public GameObject laserButton;
    public GameObject foodButton;
    public GameObject feederButton;

    public GameObject shop;
    public GameObject moneyDisplay;
    public GameObject foodsDisplay;

    public GameObject fishPriceText;
    public GameObject foodPriceText;
    public GameObject foodMainText;
    public GameObject feederPriceText;

    private GameManager gm;
    private Object[] fishMeshes;
    private Object[] fishMats;
    private Transform[] buttons;
    private GameObject[] foodsOnScreen = new GameObject[0];

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

    public int startMoney;
    private int money;
    public int Money{
        get{ return money; }
        set
        {
            money = value;
            moneyDisplay.GetComponent<TMPro.TextMeshProUGUI>().SetText("$" + money.ToString());
        }
    }

    void Start(){
        gm = (GameManager)FindObjectOfType(typeof(GameManager));
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

        Money = startMoney;
        FeederPrice = startFeederPrice;
        FoodCount = 1;
        FoodsOnScreenDisplay = 0;
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
        BuyFish(gm.laserFish);
    }

    private void BuyFish(GameObject fish){
        int price = fish.GetComponent<Fish>().price;
        if (AttemptPurchase(price)){
            SpawnFish(fish);
            foodButton.SetActive(true);
            laserButton.SetActive(true);
        }
    }

    private void SpawnFish(GameObject fish){
        float x = Random.Range(gm.leftBoundary, gm.rightBoundary);
        float y = gm.topBoundary + 2f;
        Quaternion spawnRotation =  Quaternion.Euler(new Vector3( 0,0, 0));
        GameObject newFish = Instantiate(fish, new Vector3(x, y, gm.fishLayerZ), spawnRotation);
        newFish.GetComponent<Rigidbody>().AddForce(Vector3.up * spawnedFishDownwardForce, ForceMode.VelocityChange);
        gm.audioManager.PlaySound("Spawn Fish");
    }

    public void BuyFeeder(){
        if(AttemptPurchase(feederPrice))
        {
            feederCount++;
            Instantiate(gm.feeder, new Vector3(4.62302f, 15.46556f, 12.088f), Quaternion.identity);
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
        return Instantiate (gm.food, position, Quaternion.identity);
    }

    

}
