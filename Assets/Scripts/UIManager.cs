using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class UIManager : MonoBehaviour
{
    
    public GameObject moneyContainer;

    private TextMeshProUGUI  moneyText;

    private GameManager gm;

    void Start(){
        gm = (GameManager)FindObjectOfType(typeof(GameManager));
        moneyText = moneyContainer.GetComponent<TMPro.TextMeshProUGUI>();
    }

    void Update(){
        moneyText.SetText("$" + gm.money.ToString());
    }

    
}
