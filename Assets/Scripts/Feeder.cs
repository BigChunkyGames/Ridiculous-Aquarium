using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Feeder : MonoBehaviour
{

    private GameManager gm;
    public float dropRate; // seconds
    public float foodLifetime = 10f;
    public GameObject feederFood;

    void Start(){
        gm = (GameManager)FindObjectOfType(typeof(GameManager));
        InvokeRepeating("DropFood", 0f, dropRate );
    }


    public void DropFood(){
        // only drop as much food as food count * feeder count
        GameObject[] foods = GameObject.FindGameObjectsWithTag("Food");
        if (foods.Length >= (gm.shop.FoodMax - 1) * gm.shop.feederCount){
            return;
        }
        GameObject dropped = Instantiate (gm.shop.pelletToSpawn, gameObject.transform.position, gm.shop.pelletToSpawn.transform.rotation);

        Destroy(dropped, foodLifetime);
    }

}
