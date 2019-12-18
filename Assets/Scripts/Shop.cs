using UnityEngine;
using UnityEngine.UI;
using TMPro;

// this is on each button

public class Shop : MonoBehaviour
{
    private GameManager gm;
    private float spawnedFishDownwardForce = -7f;
    private float foodCountPrice = 50f; // starting price that changes
    private float fishPrice = 100f;
    private float feederPrice = 1000f;

    public GameObject fishPriceText;
    public GameObject foodPriceText;
    public GameObject feederPriceText;

    private float foodCountPriceIncreaseRate = 1.25f;
    private float feederPriceIncreaseRate = 1.25f;

    private Object[] fishMeshes;
    private Object[] fishMats;

    void Start(){
        gm = (GameManager)FindObjectOfType(typeof(GameManager));
        fishMeshes = Resources.LoadAll("Meshes/TropicalFish", typeof(Mesh));
        fishMats = Resources.LoadAll("Meshes/TropicalFish", typeof(Material));
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
        float price = fish.GetComponent<Fish>().price;
        if (AttemptPurchase(price)){
            SpawnFish(fish);
        }
    }

    private void SpawnFish(GameObject fish){
        float x = Random.Range(gm.leftBoundary, gm.rightBoundary);
        float y = gm.topBoundary + 2f;
        Quaternion spawnRotation =  Quaternion.Euler(new Vector3( 0,0, 0));
        GameObject newFish = Instantiate(fish, new Vector3(x, y, 0), spawnRotation);
        newFish.GetComponent<Rigidbody>().AddForce(Vector3.up * spawnedFishDownwardForce, ForceMode.VelocityChange);
    }

    public void BuyFeeder(){
        if(AttemptPurchase(feederPrice))
        {
            Instantiate(gm.feeder, new Vector3(4.62302f, 15.46556f, 12.088f), Quaternion.identity);
            feederPrice = (int)(feederPrice*feederPriceIncreaseRate);
            feederPriceText.GetComponent<TMPro.TextMeshProUGUI>().SetText("$" + feederPrice.ToString());
        }
    }

    public void BuyFoodCount(){
        if (AttemptPurchase(foodCountPrice)){
            gm.foodCount++;
            foodCountPrice = (int)(foodCountPrice*foodCountPriceIncreaseRate);
            foodPriceText.GetComponent<TMPro.TextMeshProUGUI>().SetText("$" + foodCountPrice.ToString());
        }
    }

    private bool AttemptPurchase(float cost)
    {
        if (cost < gm.money){
            gm.money -= cost;
            return true;
        }
        else
        {
            Debug.Log("Not enough money!");
            return false;
        }
    }
}
