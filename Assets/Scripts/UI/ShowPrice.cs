using UnityEngine;

public class ShowPrice : MonoBehaviour
{
    GameManager gm;

    void Start()
    {
        gm = (GameManager)FindObjectOfType(typeof(GameManager));
    }

    void OnMouseOver()
    {
        gm.shop.ShowFoodPrice(false);
    }

    void OnMouseExit()
    {
        gm.shop.ShowFoodPrice(true);
    }

}
