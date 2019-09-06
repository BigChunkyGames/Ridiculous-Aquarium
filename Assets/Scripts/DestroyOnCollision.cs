using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOnCollision : MonoBehaviour
{
    void OnCollisionEnter(Collision col){
        if (col.gameObject.layer == LayerMask.NameToLayer("Boundary")){
            Destroy(gameObject); // destory if dropable hits ground or boundary
        }
    }
}
