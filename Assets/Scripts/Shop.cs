﻿using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using System.Collections.Generic;  
using UnityEngine.SceneManagement;

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
    public float laserFoodDamageGain = .025f;
    public int foodLevelPrice;
    public int laserLevelPrice;
    public GameObject foodLevelButton;
    public GameObject foodButton;

    [Header("Unlocker Buttons")]
    public GameObject buyFishButton;
    public GameObject buyTurtleButton;
    public GameObject unlockFeederButton;
    public int unlockFeederPrice = 1000;
    public GameObject unlockLaserFoodButton;
    public int unlockLaserFoodPrice = 200;
    private bool laserFoodUnlocked = false;
    public GameObject winButton;
    public int winPrice = 60000;
    public GameObject unlockFoodButton;
    public int unlockFoodPrice = 50;

    [Header("TMP and Gameobjects")]
    public GameObject shop;
    public GameObject moneyDisplay;
    public GameObject passiveIncomeDisplay;
    public GameObject playButton;
    public GameObject muteButtonText;
    
    [Header("Food Stats Area")]
    public GameObject foodStats;
    public GameObject foodMaxPriceText; // where it says how much to get more food
    public GameObject foodPriceText; // where it says how much buying a food costs
    public GameObject foodLevelText; 
    public GameObject foodStatText;  // health or dps
    public GameObject foodDropdown;
    public GameObject foodsDisplay;

    [Header("Feeder Stats")]
    public GameObject feederStats;
    public int feederSpeedUpgradePrice = 50;
    public GameObject feeder;
    public GameObject feederUpgradeSpeedText;
    public GameObject feederRateText;
    public GameObject feederLevelText;
    public GameObject feederFoodDecoration;
    [Header("Misc")]
    public GameObject foodDecoration;
    public GameObject laserFoodDecoration;
    public GameObject combatLevelText;
    public GameObject numberPoofEffect;
    public GameObject endGameWinStatsText;
    public GameObject endGameLoseStatsText;
    public GameObject winMenu;
    public GameObject pauseMenu;
    public GameObject loseMenu;

    public int turtlePrice;

    private GameManager gm;
    private Object[] fishModels;
    private Transform[] buttons;
    private GameObject[] foodsOnScreen = new GameObject[0];
    [HideInInspector]public GameObject foodToSpawn; // what gets spawned when you click
    [HideInInspector]public GameObject pelletToSpawn; // the visual level of the pellet

    private int fishPrice = -1;
    private int FishPrice
    {
        get{return this.fishPrice;}
        set{
            fishPrice = value;
            SetButtonPriceText(buyFishButton, fishPrice);
        }
    }

    private int friendlyFishCount = 0;
    public int FriendlyFishCount
    {
        set{
            friendlyFishCount = value;
            if(value == 0){
                ShowLoseMenu();
            }
            Debug.Log("Fish in tank: " + friendlyFishCount);
            FishPrice = gm.scalingManager.ScaleFishPrice(friendlyFishCount);
            }
        get{
            return friendlyFishCount;
        }
    }

    private int turtleCount;
    public int TurtleCount{
        get{return turtleCount;}
        set{
            turtleCount = value;
            turtlePrice = gm.scalingManager.ScaleTurtlePrice(turtleCount);
            SetButtonPriceText(buyTurtleButton, turtlePrice);
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
                // update food upgrade area
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
            foodMaxPrice = gm.scalingManager.ScaleFoodMaxPrice(FoodMax);
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
            
            GameObject newPelletModel = pelletToSpawn.transform.Find("Model").gameObject;
            foodDecoration.GetComponent<MeshRenderer>().material= newPelletModel.GetComponent<MeshRenderer>().sharedMaterial;

            feederFoodDecoration.GetComponent<MeshRenderer>().material= newPelletModel.GetComponent<MeshRenderer>().sharedMaterial;
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
            foodStatText.GetComponent<TMPro.TextMeshProUGUI>().SetText(foodHPGain.ToString() + "hp");
        } else {
            foodLevelText.GetComponent<TMPro.TextMeshProUGUI>().SetText("Upgrade ");
            foodStatText.GetComponent<TMPro.TextMeshProUGUI>().SetText("$" + foodLevelPrice.ToString());
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

        SetButtonPriceText(buyFishButton, gm.scalingManager.ScaleFishPrice(friendlyFishCount));
        SetButtonPriceText(unlockFeederButton, unlockFeederPrice);
        SetButtonPriceText(unlockLaserFoodButton, unlockLaserFoodPrice);
        SetButtonPriceText(winButton, winPrice);
        SetButtonPriceText(unlockFoodButton, unlockFoodPrice);
        SetButtonPriceText(buyTurtleButton, turtlePrice);

        foodMaxPriceText.GetComponent<TMPro.TextMeshProUGUI>().SetText("$" + foodMaxPrice.ToString());

        // hide unlocker buttons
        buyFishButton.SetActive(true);
        buyTurtleButton.SetActive(false);
        unlockFeederButton.SetActive(false);
        unlockLaserFoodButton.SetActive(false);
        unlockFoodButton.SetActive(false);
        winButton.SetActive(false);
        feederStats.SetActive(false);
        foodStats.SetActive(false);

        foodToSpawn = gm.dataStore.foods[0];
        pelletToSpawn = gm.dataStore.foods[0];
        Money = startMoney;
        PassiveIncome = 0;
        FoodMax = 1;
        FoodsOnScreenDisplay = 0;
        foodLevelPrice = gm.scalingManager.ScaleFoodLevelPrice(foodLevel);
        FishPrice = gm.scalingManager.ScaleFishPrice(friendlyFishCount);
        TurtleCount = 0;
    }

    void Update()
    {
        Money += passiveIncome * Time.deltaTime / (60f );
    }

    public void BuyTurtle()
    {
        if (AttemptPurchase(turtlePrice)){
            if(winButton) winButton.SetActive(true);
            DropSomethingInTheTank((GameObject)Resources.Load("Prefabs/Fish/Turtle", typeof(GameObject)), false, true);
        }
    }

    private void SetButtonPriceText(GameObject button, int price){
        button.transform.Find("Price").GetComponent<TMPro.TextMeshProUGUI>().SetText("$"+price.ToString());
    }

    public void UnlockFood()
    {
        if (AttemptPurchase(unlockFoodPrice)){
            foodStats.SetActive(true);
            if(unlockLaserFoodButton) unlockLaserFoodButton.SetActive(true);
            Destroy(unlockFoodButton);
        }
    }

    public void BuyRandomFish()
    {
        if (AttemptPurchase(FishPrice)){
            DropSomethingInTheTank((GameObject)Resources.Load("Prefabs/Fish/Generic Fish", typeof(GameObject)), true,true);
            foodButton.SetActive(true);
            if(unlockFoodButton) unlockFoodButton.SetActive(true);
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
        feederUpgradeSpeedText.GetComponent<TMPro.TextMeshProUGUI>().SetText(feederSpeedUpgradePrice.ToString() + "$");
        SetTMPText(feederRateText, ((60f/f.dropRate)).ToString("F2") + " food/min");
    }
    public void UnlockFeeder(){
        if(AttemptPurchase(unlockFeederPrice))
        {
            gm.audioManager.PlaySound("Buy Upgrade");
            this.feederStats.gameObject.SetActive(true);
            buyTurtleButton.SetActive(true);
            Destroy(this.unlockFeederButton);
            UpdateFeederDisplay();
        }
    }
//
    public void UnlockLaserFood(){
        if(AttemptPurchase(unlockLaserFoodPrice))
        {
            if(unlockFeederButton) unlockFeederButton.SetActive(true);
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
        if(foodStats.activeSelf == false) return;
        foodsOnScreen = GameObject.FindGameObjectsWithTag("Food");
        FoodsOnScreenDisplay = foodsOnScreen.Length;
        if (foodsOnScreen.Length < gm.shop.FoodMax){ // if amount of foods on screen less than the amount allowed
            if(gm.shop.AttemptPurchase(currentSpawnFoodPrice)) {
                // add 2.1 for distance offset because fuck
                Vector3 spawnLocation = new Vector3(hit.point.x, hit.point.y + 2.1f, gm.fishLayerZ);
                PlayerSpawnFood(spawnLocation);
                gm.audioManager.PlaySound("Spawn Food");
                gm.foodsMade++;
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

    public void SetTMPText(GameObject tmpObject, string s)
    {
        tmpObject.GetComponent<TMPro.TextMeshProUGUI>().SetText(s);
    }

    // if its not money its damage
    public void MakeNumberPoof(string numberValue, Vector3 position, bool money)
    {
        GameObject poof = this.numberPoofEffect;
        poof = Instantiate(poof, position, Quaternion.identity);
        var tmp = poof.GetComponentInChildren<TMPro.TextMeshProUGUI>();
        if(money){
            tmp.color = Color.green;
        } else {
            tmp.color = Color.red;
        }
        tmp.SetText(numberValue);
        Destroy(poof, 3f);
    }

    public void ShowWinMenu(){
        if(AttemptPurchase(winPrice)){
            winButton.SetActive(false);
            winMenu.SetActive(true);
            string s = $@"You clicked {gm.timesClicked} times!
You got {gm.treasuresGot} treasures!
You are responsible for the death of {gm.fishDiedStat} fish!
You made {gm.foodsMade} foods!


Thats so many!";
            endGameWinStatsText.GetComponent<TMPro.TextMeshProUGUI>().SetText(s);
            gm.audioManager.audioSourceLoops.clip = (AudioClip)Resources.Load("Audio/Music/field");
            gm.audioManager.audioSourceLoops.Play();
        }
    }

    public void ShowLoseMenu(){
        loseMenu.SetActive(true);
        string s = $@"Your fish loved you up unill the bitter end.

But at least you
clicked {gm.timesClicked} times!
got {gm.treasuresGot} treasures!
were responsible for the death of {gm.fishDiedStat} fish!
made {gm.foodsMade} foods!


Give it another try!";
        endGameLoseStatsText.GetComponent<TMPro.TextMeshProUGUI>().SetText(s);
        gm.audioManager.audioSourceLoops.clip = (AudioClip)Resources.Load("Audio/Music/death");
        gm.audioManager.audioSourceLoops.Play();
        Time.timeScale = 0;
    }

    public void ContinueButton()
    {
        winMenu.SetActive(false);
    }

    public void AgainButton()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void PauseButton()
    {
        // pausing
        if(Time.timeScale == 1){
            gm.audioManager.PauseMenu(true);
            Time.timeScale = 0;
            playButton.GetComponent<TMPro.TextMeshProUGUI>().SetText("►");
            pauseMenu.SetActive(true);
        } else {
            gm.audioManager.PauseMenu(false);
            Time.timeScale = 1;
            playButton.GetComponent<TMPro.TextMeshProUGUI>().SetText("l l");
            pauseMenu.SetActive(false);
        }
    }

    public void MuteButton()
    {
        if( AudioListener.volume == 0){
            AudioListener.volume = 1;
            muteButtonText.GetComponent<TMPro.TextMeshProUGUI>().color = Color.white;
        } else {
            AudioListener.volume = 0;
            muteButtonText.GetComponent<TMPro.TextMeshProUGUI>().color = Color.red;
        }
    }

    

}
