using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject food;

    public float leftBoundary;
    public float rightBoundary;
    public float topBoundary;
    public float bottomBoundary;


    RaycastHit hit;
    Ray myRay;

    void Start(){
        leftBoundary = -8f;
        rightBoundary = 8f;
        bottomBoundary = 2f;
        topBoundary = 14f;
    }
    void Update() {
        
        myRay = Camera.main.ScreenPointToRay (Input.mousePosition); // telling my ray variable that the ray will go from the center of 
        if (Physics.Raycast (myRay, out hit)) {
            if (Input.GetMouseButtonDown (0)) {// what to do if i press the left mouse button
                Instantiate (food, hit.point, Quaternion.identity);// instatiate a prefab on the position where the ray hits the floor.
                Debug.Log (hit.point);// debugs the vector3 of the position where I clicked
         }// end upMousebutton
        }
    }
}
