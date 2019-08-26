using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class UIManager : MonoBehaviour
{
    
    public Text moneyText;

    private GameManager gm;

    void Start(){
        gm = (GameManager)FindObjectOfType(typeof(GameManager));
    }

    void Update(){
        moneyText.text = "$" + gm.money.ToString();
    }
}
