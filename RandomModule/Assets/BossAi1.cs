using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class BossAi1
{
    public float rangeX1;
    public float rangeX2;
    public float rangeY1;
    public float rangeY2;

    public int rX {get; private set;}
    public int rY {get; private set;}
    public int rIdx {get; private set;}

    public int leftIdleFrame;

    private System.Random rand;

    public BossAi1(System.Random randomModule, float rx1, float rx2, float ry1, float ry2)
    {
        rand = randomModule;

        rangeX1 = rx1;
        rangeX2 = rx2;
        rangeY1 = ry1;
        rangeY2 = ry2;
    }

    public int GetRandomAction(int max)
    {
        return rand.Next(0, max);
    }

    public void FixedUpdate()
    {
        if(leftIdleFrame > 0)
            leftIdleFrame--;
    }

    public void CaptureTarget(Vector2 pos_target, Vector2 pos_ai, int lookDir_ai)
    {
        rX = GetRX(pos_target.x - pos_ai.x, lookDir_ai);
        rY = GetRY(pos_target.y - pos_ai.y);
        rIdx = 6 * rY + rX;
    }

    private int GetRX(float px, int lookDir)
    {
        px *= lookDir;

        if(px < -rangeX2)
            return 0;
        else if(px < -rangeX1)
            return 1;
        else if(px < 0.0f)
            return 2;
        else if(px < rangeX1)
            return 3;
        else if(px < rangeX2)
            return 4;
        else
            return 5;
    }

    private int GetRY(float py)
    {
        if(py < -rangeY2)
            return 0;
        else if(py < -rangeY1)
            return 1;
        else if(py < 0.0f)
            return 2;
        else if(py < rangeY1)
            return 3;
        else if(py < rangeY2)
            return 4;
        else
            return 5;
    }
}