using System;
using System.Collections.Generic;
using UnityEngine;

public static class CirclePowerCalculator
{
    public static float GetAverageValue(Vector2 position, List<CircleOption> options)
    {
        int count = 0;
        float sum = 0.0f;
        float distance;

        foreach(CircleOption op in options)
        {
            distance = Vector2.Distance(position, op.center);

            if(distance <= op.radius)
            {
                sum += (distance / op.radius);
                count++;
            }
        }

        if(count == 0)
            return 0.0f;
        else
            return 1.0f - (sum / (float)count);
    }

    public static float GetMaxValue(Vector2 position, List<CircleOption> options)
    {
        float min = 1.0f;
        float distance;
        float ratio;

        foreach(CircleOption op in options)
        {
            distance = Vector2.Distance(position, op.center);

            if(distance <= op.radius)
            {
                ratio = distance / op.radius;

                if(ratio < min)
                    min = ratio;
            }
        }

        return 1.0f - min;
    }
}