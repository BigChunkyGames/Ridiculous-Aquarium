using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject food;
    public float money = 0f;
    public GameObject fish1;
    public GameObject feeder;
    public GameObject coin;

    public Color hungryColor;
    public Color deadColor;

    [HideInInspector] public float leftBoundary;
    [HideInInspector] public float rightBoundary;
    [HideInInspector] public float topBoundary;
    [HideInInspector] public float bottomBoundary;


    RaycastHit hit;
    Ray ray;

    void Start(){
        leftBoundary = -8f;
        rightBoundary = 8f;
        bottomBoundary = 2f;
        topBoundary = 18f;
    }
    void Update() {
        if (Input.GetMouseButtonDown (0)) {// left mouse button
            ray = Camera.main.ScreenPointToRay (Input.mousePosition);
            if (Physics.Raycast (ray, out hit)) {
                if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Dropable")){
                    Debug.Log("Got a coin");
                    money += hit.collider.GetComponent<Dropable>().worth;
                    Destroy(hit.collider.gameObject);
                    return;
                }
                Instantiate (food, hit.point, Quaternion.identity);
            }
        }
    }
}
