using UnityEngine;
using System.Collections;

[System.Serializable]
public partial class Turret : MonoBehaviour{

    public GameObject fish;
    public GameObject barrel;
    public Transform yawSegment;
    public Transform pitchSegment;
    public float yawSpeed = 30f;
    public float pitchSpeed = 30f;
    public float yawLimit = 90f;
    public float pitchLimit = 90f;

    private GameObject target;
    private LineRenderer laser;

    private Quaternion yawSegmentStartRotation;
    private Quaternion pitchSegmentStartRotation;

    private GameManager gm;

    public virtual void Start(){
        gm = (GameManager)FindObjectOfType(typeof(GameManager));
        this.laser = gm.weaponEffects.RegisterLaser(0);
        this.yawSegmentStartRotation = this.yawSegment.localRotation;
        this.pitchSegmentStartRotation = this.pitchSegment.localRotation;
        InvokeRepeating("UpdateTarget", 1.0f, .3f);
    }

    public virtual void Update(){
        // 


        /// idfk
        // float angle = 0.0f;
        // Vector3 targetRelative = default(Vector3);
        // Quaternion targetRotation = default(Quaternion);
        // if(this.yawSegment && (this.yawLimit != 0f)){
        //     targetRelative = this.yawSegment.InverseTransformPoint(this.target);
        //     angle = Mathf.Atan2(targetRelative.x, targetRelative.z) * Mathf.Rad2Deg;
        //     if(angle >= 180f)  
        //       angle = 180f - angle;
        //     if(angle <= -180f)  
        //       angle = -180f + angle;
        //     targetRotation = this.yawSegment.rotation * Quaternion.Euler(0f, Mathf.Clamp(angle, -this.yawSpeed * Time.deltaTime, this.yawSpeed * Time.deltaTime), 0f);
        //     if((this.yawLimit < 360f) && (this.yawLimit > 0f))    
        //         this.yawSegment.rotation = Quaternion.RotateTowards(this.yawSegment.parent.rotation * this.yawSegmentStartRotation, targetRotation, this.yawLimit);
        //     else    
        //         this.yawSegment.rotation = targetRotation;
        // }
        // if(this.pitchSegment && (this.pitchLimit != 0f)){
        //     targetRelative = this.pitchSegment.InverseTransformPoint(this.target);
        //     angle = -Mathf.Atan2(targetRelative.y, targetRelative.z) * Mathf.Rad2Deg;
        //     if(angle >= 180f) 
        //        angle = 180f - angle;
        //     if(angle <= -180f) 
        //        angle = -180f + angle;
        //     targetRotation = this.pitchSegment.rotation * Quaternion.Euler(Mathf.Clamp(angle, -this.pitchSpeed * Time.deltaTime, this.pitchSpeed * Time.deltaTime), 0f, 0f);
        //     if((this.pitchLimit < 360f) && (this.pitchLimit > 0f))
        //         this.pitchSegment.rotation = Quaternion.RotateTowards(this.pitchSegment.parent.rotation * this.pitchSegmentStartRotation, targetRotation, this.pitchLimit);
        //     else
        //         this.pitchSegment.rotation = targetRotation;
        // }

        // if there is a target and fish isnt hungry
        if(target != null && !fish.GetComponent<Fish>().hungry ) 
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
        Destroy(laser.gameObject);
    }

}