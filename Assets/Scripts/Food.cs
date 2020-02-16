using UnityEngine;

public enum FoodTypeEnum {food,laser}

public class Food : MonoBehaviour
{
    private GameManager gm;
    public float nourishment = 1f;
    public float healthGain = 10f;
    // types: default (food), laser
    public FoodTypeEnum foodType = FoodTypeEnum.food;

    void Start(){
        gm = (GameManager)FindObjectOfType(typeof(GameManager));
        gm.shop.FoodsOnScreenDisplay++;
    }

    private void OnDestroy() {
        gm.shop.FoodsOnScreenDisplay--;
    }

}
