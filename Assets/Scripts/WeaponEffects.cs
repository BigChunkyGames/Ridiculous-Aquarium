using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponEffects : MonoBehaviour
{
    private LineRenderer[] lasers;
    void Start()
    {
        lasers = GetComponentsInChildren<LineRenderer>();
    }

    public void AttackWithLaser(Vector3 startPosition, Vector3 targetPosition, int laserNumber)
    {
        LineRenderer lineRenderer = lasers[laserNumber];
        lineRenderer.SetPosition (0, startPosition);
        lineRenderer.SetPosition (1, targetPosition);

    }

}
