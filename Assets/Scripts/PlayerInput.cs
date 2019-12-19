using UnityEngine;
using UnityEngine.EventSystems;


public class PlayerInput : MonoBehaviour
{
    GameManager gm;
    private RaycastHit hit;
    private Ray ray;
    private bool clicking;

    public bool turboFeedMode = false; // unused

    void Start()
    {
        gm = (GameManager)FindObjectOfType(typeof(GameManager));
        
    }

    void Update() {
        if (this.turboFeedMode){
            clicking = Input.GetButton("Fire1");
        } else {
            clicking = Input.GetMouseButtonDown(0);
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
                if (hit.transform.tag == "Dropable" ){
                    PickupDropable(hit.transform.gameObject);
                    return;
                }

                // try to place food
                GameObject[] foods = GameObject.FindGameObjectsWithTag("Food");
                if (foods.Length < gm.shop.FoodCount){
                    if(gm.shop.AttemptPurchase(3)) {
                        Instantiate (gm.food, hit.point, Quaternion.identity);
                        gm.audioManager.PlaySound("Spawn Food");
                    }
                }
            }
        }
    }

    void PickupDropable(GameObject drop)
    {
        Debug.Log("Picked up a " + drop);
        gm.shop.Money += drop.GetComponent<Dropable>().worth;
        gm.audioManager.PlaySound("Coin");
        Destroy(drop);
    }
}
