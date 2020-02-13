using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// laser fish

public class CombatFish : FriendlyFish
{
    [Header("Combat Fish Stats")]
    public float damageDealtPerFrame = 0.1f;

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

    // NOTE start/awake/update method of parent classes is not called for child classes
    void Start()
    {
        this.laser = gm.weaponEffects.RegisterLaser(0);
        this.yawSegmentStartRotation = this.yawSegment.localRotation;
        this.pitchSegmentStartRotation = this.pitchSegment.localRotation;
        InvokeRepeating("UpdateTarget", 1.0f, .3f);
        InvokeRepeating("BeFishy", 0.0f, activityFrequency);
    }

    private void FixedUpdate() {
        FriendlyFishFixedUpdate();

        // if there is a target and fish isnt hungry and isnt dead and target is alive
        if(target != null && !hungry && !dead && !target.GetComponent<Fish>().dead) 
        {
            AttackTarget();
        }
        else
        {
            laser.enabled = false;
        }
    }

    void AttackTarget()
    {
        laser.enabled = true;
        gm.weaponEffects.AttackWithLaser(barrel.transform.position, target.transform.position, laser);
        target.GetComponent<Fish>().TakeDamage(damageDealtPerFrame);
        
        this.pitchSegment.LookAt(this.target.transform.position);
        Debug.DrawLine(this.pitchSegment.position, this.target.transform.position, Color.red);
        Debug.DrawRay(this.pitchSegment.position, this.pitchSegment.forward * (this.target.transform.position - this.pitchSegment.position).magnitude, Color.green);
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
