using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class PlayerInput : MonoBehaviour
{
    GameManager gm;
    private RaycastHit hit;
    private Ray ray;
    private bool fire;

    void Start()
    {
        gm = (GameManager)FindObjectOfType(typeof(GameManager));
        
    }

    void FixedUpdate() {
        if (gm.turboFeedMode){
            fire = Input.GetButton("Fire1");
        } else {
            fire = Input.GetMouseButtonDown(0);
        }
        if (fire) {// left mouse button
            if (EventSystem.current.IsPointerOverGameObject()){
                // prevent clicking through ui
                Debug.Log("Prevented click through ui");
                return;
            }
            ray = Camera.main.ScreenPointToRay (Input.mousePosition);
            if (Physics.Raycast (ray, out hit)) {
                if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Dropable")){
                    // if clicking on a dropable
                    Debug.Log("Got a coin");
                    gm.money += hit.collider.GetComponent<Dropable>().worth;
                    Destroy(hit.collider.gameObject);
                    return;
                }
                GameObject[] foods = GameObject.FindGameObjectsWithTag("Food");
                if (foods.Length < gm.foodCount){
                    Instantiate (gm.food, hit.point, Quaternion.identity);
                }
            }
        }
    }
}
