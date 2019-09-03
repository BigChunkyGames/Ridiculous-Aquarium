using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buttons : MonoBehaviour
{
    private GameManager gm;
    public float spawnedFishDownwardForce = -7f;

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
}
