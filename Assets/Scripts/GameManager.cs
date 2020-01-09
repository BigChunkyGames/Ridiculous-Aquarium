using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject food;
    
    public GameObject fish1;
    public GameObject laserFish;
    public GameObject feeder;
    public List<GameObject> drops;

    [HideInInspector] public AudioManager audioManager;
    [HideInInspector] public Shop shop;
    [HideInInspector] public WeaponEffects weaponEffects;

    [HideInInspector] public float leftBoundary;
    [HideInInspector] public float rightBoundary;
    [HideInInspector] public float topBoundary;
    [HideInInspector] public float bottomBoundary;
    public GameObject floor;
    public float fishLayerZ = 4f; // the layer that the fish and food are on
    public float dropLayerZ = 2f; // the layer that the drops are on (so they're in front)

    void Awake(){
        leftBoundary = -14;
        rightBoundary = 14;
        bottomBoundary = floor.transform.position.y;
        topBoundary = 18f;

        audioManager = GetComponentInChildren<AudioManager>();
        weaponEffects = GetComponentInChildren<WeaponEffects>();
        shop = (Shop)FindObjectOfType<Shop>();
    }
    
}
