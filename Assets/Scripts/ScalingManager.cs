using UnityEngine;

public class ScalingManager : MonoBehaviour
{
    public float exponentialScalar = 1.18f;

    void Start()
    {
        
    }
// FISH
    // the amount of passive income a fish gives
    public int ScaleFishPassiveIncome(int level)
    {
        return level * 15;
    }
    // set price of new fish based on fish in tank. 
    public int ScaleFishPrice(int friendlyFishCount)
    {
        return 100*friendlyFishCount;
        //(int)(100*(Mathf.Pow(2, friendlyFishCount)));
    }
// FOOD
    public int ScaleFoodMaxPrice(int currentFoodMax)
    {
        return currentFoodMax * 10;
    }
    public int ScaleFoodHPGain(int currentFoodLevel)
    {
        return 10 + 2*currentFoodLevel;
    }
    // price for next food level
    public int ScaleFoodLevelPrice(int currentFoodLevel)
    {
        return 50*currentFoodLevel;
    }
// FEEDER
    // seconds between drops
    public float ScaleFeederDropRate(int feederLevel)
    {
        return 60f/((float)feederLevel);
    }
    public int ScaleFeederSpeedUpgradePrice(int feederLevel)
    {
        return feederLevel * 50;
    }

    
}
