using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// canvas scale is unique for each fish because fish have different scales


public class HealthBar : MonoBehaviour
{
    public GameObject healthIndication;
    public GameObject container; // width gets changed with health max
    
    void Start()
    {
        this.gameObject.SetActive(false);
    }

    public void UpdateDisplay(float healthPercent)
    {
        healthIndication.transform.localScale = new Vector3(healthPercent, healthIndication.transform.localScale.y, healthIndication.transform.localScale.z);
        if(healthPercent >= 1f || healthPercent <= 0)
        {
            this.gameObject.SetActive(false);
            return;
        } else {
            this.gameObject.SetActive(true);
        }
    }

    public void Initialize(float startHealth)
    {
        Vector3 scale = container.gameObject.GetComponent<RectTransform>().localScale;
        scale = new Vector3(startHealth/100f, scale.y, scale.z );
    }
}
