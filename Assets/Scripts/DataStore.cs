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

    void Start()
    {
        Object[] treasureObjects = Resources.LoadAll("Prefabs/Drops/Treasures", typeof(GameObject));
        for (int i = 0; i < treasureObjects.Length-1; i++)
        {
            treasures.Add((GameObject)treasureObjects[i]);
        }

    }
}
