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
    public int foodCount = 1; // amount of food player can add to screen

    public Color hungryColor;
    public Color deadColor;

    [HideInInspector] public float leftBoundary;
    [HideInInspector] public float rightBoundary;
    [HideInInspector] public float topBoundary;
    [HideInInspector] public float bottomBoundary;
    public float fishLayerZ = 4f; // the layer that the fish and food are on
    public float dropLayerZ = 2f; // the layer that the drops are on

    void Start(){
        leftBoundary = -8f;
        rightBoundary = 8f;
        bottomBoundary = 2f;
        topBoundary = 18f;
    }
    
}
