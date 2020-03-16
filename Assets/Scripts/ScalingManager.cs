using UnityEngine;

public class ScalingManager : MonoBehaviour
{
    public float exponentialScalar = 1.18f;

    void Start()
    {
        
    }
// FISH
    // the amount of passive income a fish gives
    public int ScaleFishPassiveIncome(int level, bool laserFish=false)
    {
        int income = level * 15;
        if(laserFish) income/=3;
        return income;
    }
    public float ScaleLaserFishDPF(int lasersEaten)
    {
        return .1f + (lasersEaten-1) * 0.025f;
    }
    // set price of new fish based on fish in tank. 
    public int ScaleFishPrice(int friendlyFishCount)
    {
        return (int)(100*(Mathf.Pow(exponentialScalar, friendlyFishCount)));
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
        return 60f/((float)feederLevel * 2);
    }
    public int ScaleFeederSpeedUpgradePrice(int feederLevel)
    {
        return feederLevel * 50;
    }
// MISC
    public float ScaleTreasureWorth(int treasureLevel)
    {
        return 15 * Mathf.Pow(2, treasureLevel);
    }
    
}
