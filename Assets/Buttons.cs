using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buttons : MonoBehaviour
{
    private GameManager gm;

    void Start(){
        gm = (GameManager)FindObjectOfType(typeof(GameManager));
    }

    public void BuyFish1(){
        SpawnFish(gm.fish1);
    }

    private void SpawnFish(GameObject fish){
        float x = Random.Range(gm.leftBoundary, gm.rightBoundary);
        float y = gm.topBoundary + 2f;
        GameObject newFish = Instantiate(fish, new Vector3(x, y, 0), Quaternion.identity);

    }
}
