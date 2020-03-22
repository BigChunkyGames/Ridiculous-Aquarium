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
                clicking = true; // true while a mouse button is down
            } else { clicking = false;}
        }
        if (clicking) {// left mouse button
            gm.timesClicked++;
            if (EventSystem.current.IsPointerOverGameObject()){
                Debug.Log("Prevented click through ui");
                return;
            }
            if(Time.timeScale == 0){
                // no clicking while paused!
                return;
            }
            ray = Camera.main.ScreenPointToRay (Input.mousePosition);
            Debug.DrawRay(ray.origin, ray.direction * 1000, Color.black);
            // if the ray hits
            if (Physics.Raycast (ray, out hit)) {
                // pickup treasure
                if (hit.transform.tag == "Treasure" ){
                    PickupTreasure(hit.transform);
                    return;
                }
                // shoot enemy
                else if(hit.transform.tag == "Enemy")
                {
                    hit.transform.gameObject.GetComponent<EnemyFish>().TakeDamage(gm.playerInput.attackDamage);
                    Debug.Log("Did damage to enemy");
                    gm.audioManager.PlaySound("Shoot Fish");
                    Vector3 hitPoint = ray.origin + ray.direction * hit.distance;
                    Destroy(Instantiate(gm.dataStore.hitmarkerEffect, hitPoint, Quaternion.identity), 3);
                    gm.shop.MakeNumberPoof("   " + gm.playerInput.attackDamage.ToString(), hit.transform.position, false);
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

    public void PickupTreasure(Transform hit)
    {
        int worth = gm.scalingManager.ScaleTreasureWorth(hit.gameObject.GetComponent<Treasure>().level);
        gm.shop.Money += worth;
        gm.shop.MakeNumberPoof("$" + worth.ToString(), hit.transform.position, true);
        Debug.Log("Picked up a " + hit.gameObject.name + " worth " + worth);
        gm.audioManager.PlaySound("Get Coin", 1.6f+hit.gameObject.GetComponent<Treasure>().level * .1f);
        Destroy(hit.gameObject);
        gm.treasuresGot++;
    }
}
