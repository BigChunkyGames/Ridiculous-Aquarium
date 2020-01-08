using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : MonoBehaviour
{
    private GameManager gm;
    public float nourishment = 1f;

    void Start(){
        gm = (GameManager)FindObjectOfType(typeof(GameManager));
        gm.shop.FoodsOnScreenDisplay++;
    }

    private void OnDestroy() {
        gm.shop.FoodsOnScreenDisplay--;
    }

}
