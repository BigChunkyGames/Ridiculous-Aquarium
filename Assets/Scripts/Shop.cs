using System.Collections;
using System.Collections.Generic;
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

    void Start(){
        gm = (GameManager)FindObjectOfType(typeof(GameManager));
    }

    public void BuyFish1(){
        BuyFish(gm.fish1);
    }

    private void BuyFish(GameObject fish){
        float price = fish.GetComponent<Fish>().price;
        if (price < gm.money){
            gm.money -= price;
            SpawnFish(fish);
        } else {
            Debug.Log("Not enough money!");
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
        Instantiate(gm.feeder, new Vector3(4.62302f, 15.46556f, 12.088f), Quaternion.identity);
    }

    public void BuyFoodCount(){
        float price = foodCountPrice;
        if (price < gm.money){
            gm.money -= price;
            gm.foodCount++;
            foodCountPrice += foodCountPrice*1.5f;
            priceText.GetComponent<TMPro.TextMeshProUGUI>().SetText("$" + foodCountPrice.ToString());
        } else {
            Debug.Log("Not enough money!");
        }
    }
}
