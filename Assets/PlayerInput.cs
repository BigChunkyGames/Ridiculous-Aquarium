using System.Collections;
using System.Collections.Generic;
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
                GameObject[] foods = GameObject.FindGameObjectsWithTag("Food");
                if (foods.Length < gm.foodCount){
                   // Vector3 spawnLocation = new Vector3(hit.point.x, hit.point.y, gm.fishLayerZ);
                    Instantiate (gm.food, hit.point, Quaternion.identity);
                }
            }
        }
    }

    void PickupDropable(GameObject drop)
    {
        Debug.Log("Picked up a " + drop);
        gm.money += drop.GetComponent<Dropable>().worth;
        Destroy(drop);
    }
}
