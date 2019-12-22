using UnityEngine;
using UnityEngine.UI;
using TMPro;

// this goes on the container of buttons
// it handles money and shop ui

public class Shop : MonoBehaviour
{
    public float spawnedFishDownwardForce = -7f;
    public int fishPrice = 100;
    public int foodCountPrice = 50; // starting prices that change
    public float foodCountPriceIncreaseRate = 1.25f;
    public float feederPriceIncreaseRate = 1.25f;

    public GameObject fishButton;
    public GameObject foodButton;
    public GameObject feederButton;

    public GameObject moneyContainer;
    public GameObject fishPriceText;
    public GameObject foodPriceText;
    public GameObject foodMainText;
    public GameObject feederPriceText;

    private GameManager gm;
    private TextMeshProUGUI  moneyText;
    private Object[] fishMeshes;
    private Object[] fishMats;
    private Transform[] buttons;

    private int foodCount = 1;
    public int FoodCount { // amount of food player can add to screen
        get{return foodCount;}
        set{
            foodCount = value;
            
            string s = "Food (x"+foodCount.ToString()+")";
            foodMainText.GetComponent<TMPro.TextMeshProUGUI>().SetText(s);

            foodCountPrice = (int)(foodCountPrice*foodCountPriceIncreaseRate);
            foodPriceText.GetComponent<TMPro.TextMeshProUGUI>().SetText("$" + foodCountPrice.ToString());
            feederButton.SetActive(true);
            
        }
    }

    public int startFeederPrice;
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
            moneyText.SetText("$" + money.ToString());
        }
    }

    void Start(){
        gm = (GameManager)FindObjectOfType(typeof(GameManager));
        fishMeshes = Resources.LoadAll("Meshes/TropicalFish", typeof(Mesh));
        fishMats = Resources.LoadAll("Meshes/TropicalFish", typeof(Material));
        moneyText = moneyContainer.GetComponent<TMPro.TextMeshProUGUI>();

        // make buttons invisible
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }
        fishButton.SetActive(true);

        fishPriceText.GetComponent<TMPro.TextMeshProUGUI>().SetText("$" + fishPrice.ToString());
        foodPriceText.GetComponent<TMPro.TextMeshProUGUI>().SetText("$" + foodCountPrice.ToString());
        string s = "Food (x"+foodCount.ToString()+")";
        foodMainText.GetComponent<TMPro.TextMeshProUGUI>().SetText(s);

        Money = startMoney;
        FeederPrice = startFeederPrice;
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

    private void BuyFish(GameObject fish){
        int price = fish.GetComponent<Fish>().price;
        if (AttemptPurchase(price)){
            SpawnFish(fish);
            foodButton.SetActive(true);
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
            FeederPrice = (int)(feederPrice*feederPriceIncreaseRate);
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

    

}
