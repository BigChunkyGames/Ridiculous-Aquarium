using System.Collections.Generic;
using UnityEngine;

public class DataStore : MonoBehaviour
{
    public List<GameObject> foods;
    public GameObject feeder;
    public GameObject laserFood;
    public GameObject hitmarkerEffect;
    public List<GameObject> friendlyFish;
    public List<GameObject> evilFish;
    public List<GameObject> treasures;
    public Color fishHungryColor;
    public Color fishDeadColor;

    void Start()
    {
        // assign treasures their level
        Object[] treasureObjects = Resources.LoadAll("Prefabs/Drops/Treasures", typeof(GameObject));
        for (int i = 0; i < treasureObjects.Length; i++)
        {
            treasures.Add((GameObject)treasureObjects[i]);
            treasures[i].GetComponent<Treasure>().level = i+1;
        }

    }
}
