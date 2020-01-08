using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate2 : MonoBehaviour
{
    public float rate = 1f;

    // Update is called once per frame
    void FixedUpdate()
    {
        gameObject.transform.Rotate(0, 0, rate, Space.World);
    }
}
