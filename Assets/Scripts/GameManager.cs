﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public bool DebugMode;

    [HideInInspector] public AudioManager audioManager;
    [HideInInspector] public Shop shop;
    [HideInInspector] public WeaponEffects weaponEffects;
    [HideInInspector] public CombatManager combatManager;
    [HideInInspector] public DataStore dataStore;

    [HideInInspector] public float leftBoundary;
    [HideInInspector] public float rightBoundary;
    [HideInInspector] public float topBoundary;
    [HideInInspector] public float bottomBoundary;
    public GameObject backBoundary; // the choral reaf thing

    public GameObject floor;
    public float fishLayerZ = 4f; // the layer that the fish and food are on
    public float dropLayerZ = 2f; // the layer that the drops are on (so they're in front)

    void Awake(){
        leftBoundary = -12;
        rightBoundary = 12;
        bottomBoundary = floor.transform.position.y;
        topBoundary = 18f;

        audioManager = GetComponentInChildren<AudioManager>();
        weaponEffects = GetComponentInChildren<WeaponEffects>();
        shop = (Shop)FindObjectOfType<Shop>();
        combatManager = (CombatManager)FindObjectOfType<CombatManager>();
        dataStore = this.GetComponent<DataStore>();
    }
    
}
