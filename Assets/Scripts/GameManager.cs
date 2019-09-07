using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GameManager : MonoBehaviour
{
    public GameObject food;
    public float money = 0f;
    public GameObject fish1;
    public GameObject feeder;
    public GameObject coin;
    public int foodCount = 1; // amount of food player can add to screen

    public Color hungryColor;
    public Color deadColor;

    [HideInInspector] public float leftBoundary;
    [HideInInspector] public float rightBoundary;
    [HideInInspector] public float topBoundary;
    [HideInInspector] public float bottomBoundary;

    public bool turboFeedMode = false;

    private RaycastHit hit;
    private Ray ray;
    private bool fire;

    void Start(){
        leftBoundary = -8f;
        rightBoundary = 8f;
        bottomBoundary = 2f;
        topBoundary = 18f;
    }
    void Update() {
        if (turboFeedMode){
            fire = Input.GetButton("Fire1");
        } else {
            fire = Input.GetMouseButtonDown(0);
        }
        if (fire) {// left mouse button
            if (EventSystem.current.IsPointerOverGameObject()){
                return;
            }
            ray = Camera.main.ScreenPointToRay (Input.mousePosition);
            if (Physics.Raycast (ray, out hit)) {
                if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Dropable")){
                    Debug.Log("Got a coin");
                    money += hit.collider.GetComponent<Dropable>().worth;
                    Destroy(hit.collider.gameObject);
                    return;
                }
                GameObject[] foods = GameObject.FindGameObjectsWithTag("Food");
                if (foods.Length < foodCount){
                    Instantiate (food, hit.point, Quaternion.identity);
                }
            }
        }
    }
}
