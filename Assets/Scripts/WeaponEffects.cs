using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponEffects : MonoBehaviour
{
    private Object[] lasers; // list of prefabs
    private List<LineRenderer> registeredLasers;
    void Awake()
    {
        registeredLasers = new List<LineRenderer>();
        lasers = Resources.LoadAll("Prefabs/Weapons/Lasers", typeof(GameObject));
    }

    // points a laser at a target
    // hide laser if target is null
    public void AttackWithLaser(Vector3 startPosition, Vector3 targetPosition, LineRenderer laser )
    {
        laser.SetPosition (0, startPosition);
        laser.SetPosition (1, targetPosition);

    }

    public LineRenderer RegisterLaser(int laserNumber)
    {
        // create new gameobejct
        GameObject laserHolder = Instantiate(lasers[laserNumber]) as GameObject;
        // make it a child of this
        laserHolder.transform.parent = transform;
        return laserHolder.GetComponent<LineRenderer>();
    }

}
