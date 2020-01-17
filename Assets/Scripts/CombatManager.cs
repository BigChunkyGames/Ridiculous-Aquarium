﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// schedules combat events'

// wait until its time for combat
// show a warning a few seconds earlier
// spawn evil fish

public class CombatManager : MonoBehaviour
{
    public float secondsBetweenCombat = 60f;
    public float warningTimeOffset = 10f;

    private GameObject warningLight;
    private bool inCombat = false;
    private int enemiesInCurrentCombat = 0;

    private GameManager gm;

    void Start()
    {
        gm = (GameManager)FindObjectOfType(typeof(GameManager));

        PlanNextCombat();
        
    }

    private void StartCombat()
    {
        inCombat = true;
        GameObject evilFishToSpawn = gm.evilFish;

        enemiesInCurrentCombat++;
        gm.shop.SpawnFish(evilFishToSpawn);
    }

    private void ShowWarning()
    {
        // spawn the warning thing
        warningLight = gm.shop.DropSomethingInTheTank((GameObject)Resources.Load("Prefabs/Props/Warning Light"), false, -1f, false);

        
        // TODO Warning noise
    }

    private void PlanNextCombat()
    {
        Invoke("ShowWarning", secondsBetweenCombat - warningTimeOffset);
        Invoke("StartCombat", secondsBetweenCombat);
    }

    // called by OnDestory() of enemyfish
    public void EnemyWasDestroyed()
    {
        enemiesInCurrentCombat--;
        if(enemiesInCurrentCombat == 0)
        {
            EndCombat();
        }
    }

    private void EndCombat()
    {
        Destroy(warningLight);
        PlanNextCombat();
    }
}