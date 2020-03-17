﻿using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;  

// handles all the UI

public class Shop : MonoBehaviour
{
    [Header("Shop")]
    public float startMoney;
    public float spawnedFishDownwardForce = -5f;

    [Header("Food")]
    public int spawnPelletFoodPrice = 3;
    public int spawnLaserFoodPrice = 100;
    public int currentSpawnFoodPrice = 3;
    public int startingFoodCount = 1;
    public int foodMaxPrice = 10; // starting prices that change
    public int foodHPGain = 10;
    public int foodLevelPrice;

    public GameObject fishButton;
    public GameObject foodLevelButton;
    public GameObject foodButton;

    [Header("Unlocker Buttons")]
    public GameObject unlockFeederButton;
    public int unlockFeederPrice = 1000;
    public GameObject unlockLaserFoodButton;
    public int unlockLaserFoodPrice = 200;
    private bool laserFoodUnlocked = false;
    public GameObject winButton;
    public int winPrice = 60000;


    [Header("TMP and Gameobjects")]
    public GameObject shop;
    public GameObject moneyDisplay;
    public GameObject passiveIncomeDisplay;
    public GameObject foodsDisplay;
    public GameObject fishPriceText;
    public GameObject foodMaxPriceText; // where it says how much to get more food
    public GameObject foodPriceText; // where it says how much buying a food costs
    public GameObject foodLevelText; 
    public GameObject foodHPText; 
    public GameObject foodDropdown;
    [Header("Feeder")]
    public int feederSpeedUpgradePrice = 50;
    public GameObject feeder;
    public GameObject feederPriceText;
    public GameObject feederArea;
    public GameObject feederUpgradeSpeedText;
    public GameObject feederRateText;
    public GameObject feederLevelText;
    [Header("Misc")]
    public GameObject unlockLaserFoodPriceText;
    public GameObject foodDecoration;
    public GameObject laserFoodDecoration;
    public GameObject combatLevelText;

    private GameManager gm;
    private Object[] fishModels;
    private Transform[] buttons;
    private GameObject[] foodsOnScreen = new GameObject[0];
    [HideInInspector]public GameObject foodToSpawn; // what gets spawned when you click
    [HideInInspector]public GameObject pelletToSpawn; // the visual level of the pellet

    private int fishPrice = 100;
    private int FishPrice
    {
        get{return this.fishPrice;}
        set{
            fishPrice = value;
            fishPriceText.GetComponent<TMPro.TextMeshProUGUI>().SetText("$" + fishPrice.ToString());
        }
    }

    private int friendlyFishCount = 0;
    public int FriendlyFishCount
    {
        set{
            friendlyFishCount = value;
            Debug.Log("Fish in tank: " + friendlyFishCount);
            FishPrice = gm.scalingManager.ScaleFishPrice(friendlyFishCount);
            }
        get{
            return friendlyFishCount;
        }
    }

    // teh value of the food dropdown
    private int foodToSpawnDropdownIndex = 0;
    public int FoodToSpawnDropdownIndex
    {
        set {
            foodToSpawnDropdownIndex = value;
            // if spawning pellet food
            if(value == 0)
            {
                this.foodToSpawn = this.pelletToSpawn;
                this.foodPriceText.GetComponent<TMPro.TextMeshProUGUI>().SetText("Cost $" + spawnPelletFoodPrice);
                currentSpawnFoodPrice = spawnPelletFoodPrice;
                foodDecoration.SetActive(true);
                laserFoodDecoration.SetActive(false);
            }
            // if dropping laser food
            else if (value == 1 && laserFoodUnlocked)
            {
                this.foodToSpawn = gm.dataStore.laserFood;
                this.foodPriceText.GetComponent<TMPro.TextMeshProUGUI>().SetText("Cost $" + spawnLaserFoodPrice);
                currentSpawnFoodPrice = spawnLaserFoodPrice;
                foodDecoration.SetActive(false);
                laserFoodDecoration.SetActive(true);
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
            if(foodsOnScreenDisplay > foodMax) foodsOnScreenDisplay = foodMax;

            foodsDisplay.GetComponent<TMPro.TextMeshProUGUI>().SetText(foodsOnScreenDisplay + "/" + FoodMax);
        }
    }

    private int foodMax;
    public int FoodMax { // amount of food player can add to screen
        get{return foodMax;}
        set{
            foodMax = value;
            foodMaxPriceText.GetComponent<TMPro.TextMeshProUGUI>().SetText("$" + foodMaxPrice.ToString());
            foodsDisplay.GetComponent<TMPro.TextMeshProUGUI>().SetText(foodsOnScreenDisplay + "/" + FoodMax);
        }
    }

    private int foodLevel = 1; // how good the food is
    public int FoodLevel{
        get{return this.foodLevel;}
        set{
            foodLevel = value;
            foodHPGain = gm.scalingManager.ScaleFoodHPGain(foodLevel);
            foodLevelPrice = gm.scalingManager.ScaleFoodLevelPrice(foodLevel);

            // every 5 levels, drop a higher grade food
            int foodToGet = foodLevel / 5;
            // prevent list index error
            if(foodToGet >= gm.dataStore.foods.Count) foodToGet = gm.dataStore.foods.Count-1;
            this.pelletToSpawn = gm.dataStore.foods[foodToGet];
            // update food to spawn badly
            FoodToSpawnDropdownIndex = foodToSpawnDropdownIndex;
            ShowFoodPrice(false);
        }
    }

    // if true, show level, if false, show price
    public void ShowFoodPrice(bool levelSide)
    {
        if(levelSide){
            foodLevelText.GetComponent<TMPro.TextMeshProUGUI>().SetText("Level " + foodLevel.ToString());
            foodHPText.GetComponent<TMPro.TextMeshProUGUI>().SetText(foodHPGain.ToString() + "HP");
        } else {
            foodLevelText.GetComponent<TMPro.TextMeshProUGUI>().SetText("Upgrade ");
            foodHPText.GetComponent<TMPro.TextMeshProUGUI>().SetText("$" + foodLevelPrice.ToString());
        }

    }

    private float money;
    public float Money{
        get{ return money; }
        set
        {
            money = value;
            moneyDisplay.GetComponent<TMPro.TextMeshProUGUI>().SetText("$" + Mathf.RoundToInt(money).ToString());
        }
    }

    private float passiveIncome; // money gained per minute
    public float PassiveIncome{
        get{ return passiveIncome; }
        set
        {
            passiveIncome = value;
            passiveIncomeDisplay.GetComponent<TMPro.TextMeshProUGUI>().SetText("$" + Mathf.RoundToInt(passiveIncome).ToString() + " / min");
        }
    }

    void Awake()
    {
        gm = (GameManager)FindObjectOfType(typeof(GameManager));
    }

    void Start(){
        fishModels = Resources.LoadAll("Prefabs/Fish/Models", typeof(GameObject));

        // make buttons invisible
        foreach (Transform child in this.shop.GetComponent<Transform>())
        {
            child.gameObject.SetActive(false);
        }
        fishButton.SetActive(true);
        unlockLaserFoodButton.SetActive(false);
        unlockFeederButton.SetActive(false);
        feederArea.SetActive(false);

        fishPriceText.GetComponent<TMPro.TextMeshProUGUI>().SetText("$" + FishPrice.ToString());
        foodMaxPriceText.GetComponent<TMPro.TextMeshProUGUI>().SetText("$" + foodMaxPrice.ToString());
        feederPriceText.GetComponent<TMPro.TextMeshProUGUI>().SetText("$" + unlockFeederPrice.ToString());
        unlockLaserFoodPriceText.GetComponent<TMPro.TextMeshProUGUI>().SetText("$" + unlockLaserFoodPrice.ToString());

        foodToSpawn = gm.dataStore.foods[0];
        pelletToSpawn = gm.dataStore.foods[0];
        Money = startMoney;
        PassiveIncome = 0;
        FoodMax = 1;
        FoodsOnScreenDisplay = 0;
        foodLevelPrice = 50; // like ok this is needed i guess
    }

    void Update()
    {
        Money += passiveIncome * Time.deltaTime/ (60f );
    }

    public void BuyRandomFish()
    {
        if (AttemptPurchase(FishPrice)){
            DropSomethingInTheTank((GameObject)Resources.Load("Prefabs/Fish/Generic Fish", typeof(GameObject)), true,true);
            foodButton.SetActive(true);
            if(unlockFeederButton) unlockFeederButton.SetActive(true);
            if(unlockLaserFoodButton) unlockLaserFoodButton.SetActive(true);
        }
    }

    public GameObject DropSomethingInTheTank(GameObject toSpawn, bool itsARandomFish=false, bool itsAFish=false, float destroyAfterSeconds = -1f, bool randomX=true)
    {
        // determine transform of spawn location
        float x = 0f;
        if(randomX) x = Random.Range(gm.leftBoundary, gm.rightBoundary);
        float y = gm.topBoundary + 2f;
        float z = gm.fishLayerZ;
        if(!itsAFish) z = z+2f; // if its not a fish, put it 2 units in back
        else gm.audioManager.PlaySound("Spawn Fish");
        Quaternion spawnRotation =  Quaternion.Euler(new Vector3( 0,0, 0));
        GameObject spawned = Instantiate(toSpawn, new Vector3(x, y, z), spawnRotation);
        if(itsARandomFish)
        {
            // delete default model
            GameObject currentModelContainer = spawned.transform.Find("Model Container 1").gameObject;
            if(currentModelContainer) DestroyImmediate(currentModelContainer);
            // set model
            int rand = (int)Random.Range(0, fishModels.Length);
            GameObject newModelContainer = (GameObject)fishModels[rand];
            newModelContainer = Instantiate(newModelContainer, spawned.transform.position, newModelContainer.transform.rotation );
            newModelContainer.transform.parent = spawned.transform;
            newModelContainer.transform.localScale = new Vector3(1, 1,1);
            spawned.GetComponent<Fish>().ModelContainer = newModelContainer;
        }

        Rigidbody r = spawned.GetComponent<Rigidbody>();
        // give it downward force
        if(r) r.AddForce(Vector3.up * spawnedFishDownwardForce, ForceMode.VelocityChange);
        if(destroyAfterSeconds>0)
        {
            Destroy(spawned, destroyAfterSeconds);
        }
        return spawned;
    }

    public void BuyFoodMaxIncrease(){
        if (AttemptPurchase(foodMaxPrice)){
            gm.audioManager.PlaySound("Buy Upgrade");
            FoodMax++;
            foodMaxPrice = gm.scalingManager.ScaleFoodMaxPrice(FoodMax);
        }
    }

    public void BuyFoodLevel(){
        if(AttemptPurchase(foodLevelPrice)){
            gm.audioManager.PlaySound("Buy Upgrade");
            FoodLevel++;
        }
    }
// Feeder
    public void UpgradeFeederSpeed()
    {
        if(AttemptPurchase(feederSpeedUpgradePrice)){
            gm.audioManager.PlaySound("Buy Upgrade");
            Feeder f = feeder.GetComponent<Feeder>();
            f.level++;
            this.feederSpeedUpgradePrice = gm.scalingManager.ScaleFeederSpeedUpgradePrice(f.level);
            UpdateFeederDisplay();
        }
    }
    public void UpdateFeederDisplay()
    {
        Feeder f = feeder.GetComponent<Feeder>();
        f.DropRate = gm.scalingManager.ScaleFeederDropRate(f.level);
        feederLevelText.GetComponent<TMPro.TextMeshProUGUI>().SetText("Level " +f.level.ToString());
        feederUpgradeSpeedText.GetComponent<TMPro.TextMeshProUGUI>().SetText("Upgrade " +feederSpeedUpgradePrice.ToString() + "$");
        SetText(feederRateText, ((60f/f.dropRate)).ToString("F2") + " food/min");
    }
    public void UnlockFeeder(){
        if(AttemptPurchase(unlockFeederPrice))
        {
            gm.audioManager.PlaySound("Buy Upgrade");
            this.feederArea.gameObject.SetActive(true);
            Destroy(this.unlockFeederButton);
            UpdateFeederDisplay();
        }
    }
//
    public void UnlockLaserFood(){
        if(AttemptPurchase(unlockLaserFoodPrice))
        {
            gm.audioManager.PlaySound("Buy Upgrade");
            this.laserFoodUnlocked = true;
            Destroy(this.unlockLaserFoodButton);
            List<string> newLaserItem = new List<string>();
            newLaserItem.Add("Laser");
            foodDropdown.GetComponent<TMP_Dropdown>().AddOptions(newLaserItem);
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
            gm.audioManager.PlaySound("Error");
            moneyDisplay.GetComponent<Animation>().Play();
            return false;
        }
    }

    // triggered when the player clicks the background and spawns a food
    public void TryToBuyFood(RaycastHit hit)
    {
        foodsOnScreen = GameObject.FindGameObjectsWithTag("Food");
        FoodsOnScreenDisplay = foodsOnScreen.Length;
        if (foodsOnScreen.Length < gm.shop.FoodMax){ // if amount of foods on screen less than the amount allowed
            if(gm.shop.AttemptPurchase(currentSpawnFoodPrice)) {
                // add 2.1 for distance offset because fuck
                Vector3 spawnLocation = new Vector3(hit.point.x, hit.point.y + 2.1f, gm.fishLayerZ);
                PlayerSpawnFood(spawnLocation);
                gm.audioManager.PlaySound("Spawn Food");
            }
        }
        else{// if amount of foods on screen more than or =to the amount allowed
            gm.audioManager.PlaySound("Error");
            foodsDisplay.GetComponent<Animation>().Play();
        }
        FoodMax = FoodMax + 0; // trigger setter
    }

    public GameObject PlayerSpawnFood(Vector3 position)
    {
        foodToSpawn.GetComponent<Food>().healthGain = foodHPGain;
        return Instantiate (foodToSpawn, position, foodToSpawn.transform.rotation);
    }

    public void SetText(GameObject tmpObject, string s)
    {
        tmpObject.GetComponent<TMPro.TextMeshProUGUI>().SetText(s);
    }

    

}
