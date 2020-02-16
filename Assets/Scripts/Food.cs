using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : MonoBehaviour
{
    private GameManager gm;
    public float nourishment = 1f;
    public float healthGain = 10f;
    // types: default (food), laser
    public string type = "default";

    void Start(){
        gm = (GameManager)FindObjectOfType(typeof(GameManager));
        gm.shop.FoodsOnScreenDisplay++;
    }

    private void OnDestroy() {
        gm.shop.FoodsOnScreenDisplay--;
    }

}
