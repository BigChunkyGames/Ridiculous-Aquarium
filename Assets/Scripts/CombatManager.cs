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
        inCombat = true;
        List<GameObject> evilFishToSpawn = new List<GameObject>();
        int points = combatLevel;
        // build list of baddies based off point system with highest prioritized
        // but round robin 
        while(points > 0)
        {
            if(points >= 5 )
            {
                points-=5;
                evilFishToSpawn.Add(gm.dataStore.evilFish[2]);
            }
            if(points >= 3)
            {
                points-=3;
                evilFishToSpawn.Add(gm.dataStore.evilFish[1]);
            }
            if(points >= 1)
            {
                points-=1;
                evilFishToSpawn.Add(gm.dataStore.evilFish[0]);
            }
        }
        // spawn up to combat level enemies
        foreach (GameObject evilfish in evilFishToSpawn)
        {
            enemiesInCurrentCombat++;
            gm.shop.DropSomethingInTheTank(evilfish,false,true);
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
        Debug.Log("Next combat in " + secondsBetweenCombat.ToString());
        Invoke("ShowWarning", secondsBetweenCombat - warningTimeOffset);
        Invoke("StartCombat", secondsBetweenCombat);
        gm.shop.combatLevelText.GetComponent<TMPro.TextMeshProUGUI>().SetText("Level " + this.combatLevel);
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
