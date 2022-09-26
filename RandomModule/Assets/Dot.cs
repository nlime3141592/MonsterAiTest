using System;
using System.Collections.Generic;
using UnityEngine;

public class Dot : MonoBehaviour
{
    SpriteRenderer rnd;
    Color c;

    private void OnEnable()
    {
        rnd = GetComponent<SpriteRenderer>();
        c = rnd.color;
    }

    public void OnChange(List<CircleOption> options, AiMode mode)
    {
        switch(mode)
        {
            case AiMode.Average:
                c.a = CirclePowerCalculator.GetAverageValue(transform.position, options);
                break;
            case AiMode.Max:
                c.a = CirclePowerCalculator.GetMaxValue(transform.position, options);
                break;
            default:
                c.a = CirclePowerCalculator.GetAverageValue(transform.position, options);
                break;
        }

        rnd.color = c;
    }
}