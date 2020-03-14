using System.Collections.Generic;
using UnityEngine;

// schedules combat events'

// wait until its time for combat
// show a warning a few seconds earlier
// spawn evil fish

public class CombatManager : MonoBehaviour
{
    public float secondsBetweenCombat = 20f;
    public float warningTimeOffset = 10f;
    public int combatLevel = 1;
    public bool startCombatNow = false; // for debugging

    private GameObject warningLight;
    private bool inCombat = false;
    private int enemiesInCurrentCombat = 0;

    private GameManager gm;

    void Start()
    {
        gm = (GameManager)FindObjectOfType(typeof(GameManager));

        PlanNextCombat();
        if(startCombatNow)
        {
            StartCombat();
        }
        
    }

    private void StartCombat()
    {
        // TODO find a way of juding how much combat to have
        // build list of baddies

        List<GameObject> evilFishToSpawn = new List<GameObject>();

        // make a little fish for each level
        for (int i = 0; i < combatLevel; i++)
        {
            evilFishToSpawn.Add(gm.dataStore.evilFish[0]);
        }

        // make a big fish for each 2 level
        for (int i = 0; i < (combatLevel-1)/2; i++)
        {
            evilFishToSpawn.Add(gm.dataStore.evilFish[1]);
        }
         
        inCombat = true;

        foreach (GameObject evilFish in evilFishToSpawn)
        {
            enemiesInCurrentCombat++;
            gm.shop.DropSomethingInTheTank(evilFish,false,true);
        }
        gm.audioManager.CombatTime();
        combatLevel++;
    }

    private void ShowWarning()
    {
        // spawn the warning thing
        warningLight = gm.shop.DropSomethingInTheTank((GameObject)Resources.Load("Prefabs/Props/Warning Light"), false, false, -1f, false);
        gm.audioManager.FadeAudioLoops(warningTimeOffset, false);
        
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
            Destroy(warningLight);
            PlanNextCombat();
            gm.audioManager.CombatOver();
        }
        if(enemiesInCurrentCombat<0) enemiesInCurrentCombat=0;
    }

}
