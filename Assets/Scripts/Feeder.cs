using UnityEngine;

public class Feeder : MonoBehaviour
{

    private GameManager gm;
    public float foodLifetime = 10f;
    public GameObject feederFood;
    public int level = 1;
    public float dropRate = 60; // seconds
    public float DropRate{
        set{
            dropRate = value;
            CancelInvoke();
            InvokeRepeating("DropFood", dropRate, dropRate );
        }
    }
    

    void Start(){
        gm = (GameManager)FindObjectOfType(typeof(GameManager));
        InvokeRepeating("DropFood", dropRate, dropRate );
    }

    public void DropFood(){
        GameObject[] foods = GameObject.FindGameObjectsWithTag("Food");
        if (foods.Length >= (gm.shop.FoodMax - 1) ){
            return;
        }
        if(gm.shop.AttemptPurchase(gm.shop.spawnPelletFoodPrice))
        {
            GameObject dropped = Instantiate (gm.shop.pelletToSpawn, gameObject.transform.position, gm.shop.pelletToSpawn.transform.rotation);
            Destroy(dropped, foodLifetime);
        }
        
    }

}
