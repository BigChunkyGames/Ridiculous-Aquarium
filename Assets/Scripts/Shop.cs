using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Shop : MonoBehaviour
{
    private GameManager gm;
    private float spawnedFishDownwardForce = -7f;
    private float foodCountPrice = 50f; // starting price that changes
    private float fishPrice = 100f;
    private float feederPrice = 1000f;
    public GameObject priceText;

    private float foodCountPriceIncreaseRate = 1.25f;
    private float feederPriceIncreaseRate = 1.25f;

    void Start(){
        gm = (GameManager)FindObjectOfType(typeof(GameManager));
    }

    public void BuyFish1(){
        BuyFish(gm.fish1);
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
        Quaternion spawnRotation =  Quaternion.Euler(new Vector3( 0,90f, 0));
        GameObject newFish = Instantiate(fish, new Vector3(x, y, 0), spawnRotation);
        newFish.GetComponent<Rigidbody>().AddForce(Vector3.up * spawnedFishDownwardForce, ForceMode.VelocityChange);
    }

    public void BuyFeeder(){
        if(AttemptPurchase(feederPrice))
        {
            Instantiate(gm.feeder, new Vector3(4.62302f, 15.46556f, 12.088f), Quaternion.identity);
            feederPrice = (int)(feederPrice*feederPriceIncreaseRate);
            priceText.GetComponent<TMPro.TextMeshProUGUI>().SetText("$" + feederPrice.ToString());
        }
    }

    public void BuyFoodCount(){
        if (AttemptPurchase(foodCountPrice)){
            gm.foodCount++;
            foodCountPrice = (int)(foodCountPrice*foodCountPriceIncreaseRate);
            priceText.GetComponent<TMPro.TextMeshProUGUI>().SetText("$" + foodCountPrice.ToString());
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
