using System;

public class Utils
{
    // Test function
    public static float doubleNumber(float input)
    {
        float doubledAmount = input * 2;

        return doubledAmount;

    }


    // Convert an input number within a range into another range
    public static float convertToNewRange(float oldRangeMin, float oldRangeMax,float newRangeMin, float newRangeMax, float inputNum)
    {

        float oldRange = (oldRangeMin - oldRangeMax);
        float newRange = (newRangeMin - newRangeMax);

        float newValue = ((inputNum - oldRangeMin) * newRange / oldRange) + newRangeMin;

        return newValue;

    }


}
