using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jump : MonoBehaviour
{
    [Range(1,10)]
    public float jumpVelocity;
    
    // Update is called once per frame
    void Update()
    {
        if(Input.GetButtonDown("Jump")){
            JumpNow();
        }    
    }

    public void JumpNow(){
        GetComponent<Rigidbody>().velocity += transform.up * jumpVelocity;
    }
}
