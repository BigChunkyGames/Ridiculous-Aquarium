using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Feeder : MonoBehaviour
{

    private GameManager gm;
    public float dropRate;
    public float foodLifetime = 10f;

    void Start(){
        gm = (GameManager)FindObjectOfType(typeof(GameManager));
        InvokeRepeating("DropFood", 0f, dropRate );
    }


    public void DropFood(){
        GameObject dropped = Instantiate(gm.food, gameObject.transform.position, gameObject.transform.rotation);

        Destroy(dropped, foodLifetime);
    }
}
