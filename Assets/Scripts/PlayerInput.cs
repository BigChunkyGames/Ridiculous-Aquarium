using UnityEngine;
using UnityEngine.EventSystems;


public class PlayerInput : MonoBehaviour
{
    GameManager gm;
    private RaycastHit hit;
    private Ray ray;
    private bool clicking;
    public float attackDamage = 3;

    public bool turboFeedMode = false; // unused

    void Start()
    {
        gm = (GameManager)FindObjectOfType(typeof(GameManager));
        //clickToDropPrefab = gm.dataStore.
        
    }

    void Update() {
        if (this.turboFeedMode){
            clicking = Input.GetButton("Fire1");
        } else {
            // allow clicking with lrm
            if(Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(2))
            {
                clicking = true;
            } else { clicking = false;}
            
        }
        if (clicking) {// left mouse button
            if (EventSystem.current.IsPointerOverGameObject()){
                Debug.Log("Prevented click through ui");
                return;
            }
            ray = Camera.main.ScreenPointToRay (Input.mousePosition);
            Debug.DrawRay(ray.origin, ray.direction * 1000, Color.black);
            // if the ray hits
            if (Physics.Raycast (ray, out hit)) {
                // pickup treasure
                if (hit.transform.tag == "Treasure" ){
                    PickupTreasure(hit.transform.gameObject);
                    return;
                }
                // shoot enemy
                else if(hit.transform.tag == "Enemy")
                {
                    hit.transform.gameObject.GetComponent<EnemyFish>().TakeDamage(gm.playerInput.attackDamage);
                    gm.audioManager.PlaySound("Shoot Fish");
                    Vector3 hitPoint = ray.origin + ray.direction * hit.distance;
                    Instantiate(gm.dataStore.hitmarkerEffect, hitPoint, Quaternion.identity);
                }
                // poke fish
                else if(hit.transform.tag == "Fish")
                {
                    hit.transform.gameObject.GetComponent<Fish>().GetPoked();
                }
                else
                {
                // place food
                    gm.shop.TryToBuyFood(hit);
                }
            }
        }
    }

    void PickupTreasure(GameObject treasure)
    {
        Debug.Log("Picked up a " + treasure);
        gm.shop.Money += gm.scalingManager.ScaleTreasureWorth(treasure.GetComponent<Treasure>().level);
        gm.audioManager.PlaySound("Coin");
        Destroy(treasure);
    }
}
