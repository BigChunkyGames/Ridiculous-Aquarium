using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject food;
    
    public GameObject fish1;
    public GameObject feeder;
    public List<GameObject> drops;

    [HideInInspector] public AudioManager audioManager;
    [HideInInspector] public Shop shop;

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

        audioManager = GetComponentInChildren<AudioManager>();
        shop = (Shop)FindObjectOfType<Shop>();
    }
    
}
