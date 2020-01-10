using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatFish : FriendlyFish
{
    [Header("Combat Fish")]
    public float damage;
    public float damagesPerSecond;

    public GameObject barrel;
    public Transform yawSegment;
    public Transform pitchSegment;
    public float yawSpeed = 30f;
    public float pitchSpeed = 30f;
    public float yawLimit = 90f;
    public float pitchLimit = 90f;

    private LineRenderer laser;
    private Quaternion yawSegmentStartRotation;
    private Quaternion pitchSegmentStartRotation;
    private GameObject target;

    void Start()
    {
        this.laser = gm.weaponEffects.RegisterLaser(0);
        this.yawSegmentStartRotation = this.yawSegment.localRotation;
        this.pitchSegmentStartRotation = this.pitchSegment.localRotation;
        InvokeRepeating("UpdateTarget", 1.0f, .3f);
    }

    private void Update() {
        // if there is a target and fish isnt hungry and isnt dead
        if(target != null && !hungry && !dead ) 
        {
            laser.enabled = true;
            gm.weaponEffects.AttackWithLaser(barrel.transform.position, target.transform.position, laser);
            
            this.pitchSegment.LookAt(this.target.transform.position);
            Debug.DrawLine(this.pitchSegment.position, this.target.transform.position, Color.red);
            Debug.DrawRay(this.pitchSegment.position, this.pitchSegment.forward * (this.target.transform.position - this.pitchSegment.position).magnitude, Color.green);
        }
        else
        {
            laser.enabled = false;
        }
    }

    // assigns target as closest enemy
    void UpdateTarget()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        if(enemies.Length == 0)
        {
            target = null;
            return;
        }

        GameObject closest = null;
        float distance = Mathf.Infinity;
        Vector3 position = transform.position;
        foreach (GameObject go in enemies)
        {
            Vector3 diff = go.transform.position - position;
            float curDistance = diff.sqrMagnitude;
            if (curDistance < distance)
            {
                closest = go;
                distance = curDistance;
            }
        }
        target = closest;
    }

    private void OnDestroy() {
        if(laser!=null)Destroy(laser.gameObject);
    }
}
