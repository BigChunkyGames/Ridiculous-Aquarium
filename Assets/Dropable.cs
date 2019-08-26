using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dropable : MonoBehaviour
{
    public float worth;

    void OnCollisionEnter(Collision col){
        if (col.gameObject.layer == LayerMask.NameToLayer("Boundary")){
            Destroy(this); // destory if dropable hits ground or boundary
        }
    }
}
