using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Boxx : MonoBehaviour
{
    private const int stIdle = 0;
    private const int stWalk = 1;
    private const int stBackstep = 2;
    private const int stUpSlice = 3;
    private const int stTakeDown = 4;
    private const int stRotateSlice = 5;
    private System.Random m_prng;

    public List<float> gridRangeXs;
    public List<float> gridRangeYs;
    public Transform capturedTransform;
    public bool checker;
    public int lpTimes;
    public int leftLpTimes;

    private BossAiGrid bossGrid;
    private BossWeightGrid gdIdle;
    private BossWeightGrid gdWalk;
    private BossWeightGrid gdBackstep;
    private BossWeightGrid gdUpSlice;
    private BossWeightGrid gdTakeDown;
    private BossWeightGrid gdRotateSlice;
    private BossStateSampler sampler;

    private void Start()
    {
        m_prng = new System.Random();

        SetBossAi();
    }

    private void SetBossAi()
    {
        bossGrid = new BossAiGrid(gridRangeXs.ToArray(), gridRangeYs.ToArray());

        gdIdle = new BossWeightGrid(bossGrid);
        gdWalk = new BossWeightGrid(bossGrid);
        gdBackstep = new BossWeightGrid(bossGrid);
        gdUpSlice = new BossWeightGrid(bossGrid);
        gdTakeDown = new BossWeightGrid(bossGrid);
        gdRotateSlice = new BossWeightGrid(bossGrid);

        sampler = new BossStateSampler(m_prng);
// 1
        gdWalk.SetWeight(20, 0, 3);
        gdWalk.SetWeight(20, 0, 4);
        gdWalk.SetWeight(20, 0, 5);
        gdWalk.SetWeight(20, 1, 3);
        gdWalk.SetWeight(20, 1, 4);
        gdWalk.SetWeight(20, 1, 5);
        gdWalk.SetWeight(20, 2, 3);
        gdWalk.SetWeight(20, 2, 4);
        gdWalk.SetWeight(20, 2, 5);
        gdWalk.SetWeight(10, 4, 3);
        gdWalk.SetWeight(10, 4, 4);
        gdWalk.SetWeight(70, 5, 3);
        gdWalk.SetWeight(70, 5, 4);
        gdWalk.SetWeight(70, 5, 5);
// 2
        gdBackstep.SetWeight(20, 3, 3);
        gdBackstep.SetWeight(20, 3, 4);
        gdBackstep.SetWeight(60, 3, 5);
        gdBackstep.SetWeight(20, 4, 3);
        gdBackstep.SetWeight(20, 4, 4);
        gdBackstep.SetWeight(50, 4, 5);
        gdBackstep.SetWeight(30, 5, 3);
        gdBackstep.SetWeight(30, 5, 4);
        gdBackstep.SetWeight(30, 5, 5);
// 3
        gdUpSlice.SetWeight(50, 3, 3);
        gdUpSlice.SetWeight(70, 3, 4);
        gdUpSlice.SetWeight(40, 3, 5);
        gdUpSlice.SetWeight(20, 4, 3);
        gdUpSlice.SetWeight(30, 4, 4);
        gdUpSlice.SetWeight(50, 4, 5);
// 4
        gdTakeDown.SetWeight(30, 3, 3);
        gdTakeDown.SetWeight(10, 3, 4);
        gdTakeDown.SetWeight(50, 4, 3);
        gdTakeDown.SetWeight(40, 4, 4);
// 5
        gdRotateSlice.SetWeight(80, 0, 3);
        gdRotateSlice.SetWeight(80, 0, 4);
        gdRotateSlice.SetWeight(80, 0, 5);
        gdRotateSlice.SetWeight(80, 1, 3);
        gdRotateSlice.SetWeight(80, 1, 4);
        gdRotateSlice.SetWeight(80, 1, 5);
        gdRotateSlice.SetWeight(80, 2, 3);
        gdRotateSlice.SetWeight(80, 2, 4);
        gdRotateSlice.SetWeight(80, 2, 5);

        sampler.AddWeight(gdIdle);
        sampler.AddWeight(gdWalk);
        sampler.AddWeight(gdBackstep);
        sampler.AddWeight(gdUpSlice);
        sampler.AddWeight(gdTakeDown);
        sampler.AddWeight(gdRotateSlice);
    }

    private void FixedUpdate()
    {
        if(checker)
        {
            checker = false;
            float dx = capturedTransform.position.x - transform.position.x;
            float dy = capturedTransform.position.y - transform.position.y;
            int rX = bossGrid.GetRX(dx);
            int rY = bossGrid.GetRY(dy);

            Debug.Log(string.Format("(rX, rY) = ({0}, {1})", rX, rY));

            while(leftLpTimes > 0)
            {
                leftLpTimes--;
                Debug.Log(string.Format("Current State: {0}", SwitchState(sampler.GetNextState(rX, rY))));
            }
        }
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Return))
        {
            if(leftLpTimes == 0)
            {
                leftLpTimes = lpTimes;
                checker = true;
            }
        }
    }

    private string SwitchState(int state)
    {
        switch(state)
        {
            case 0:
                return "정지";
            case 1:
                return "걷기";
            case 2:
                return "뒷걸음질";
            case 3:
                return "올려베기";
            case 4:
                return "찍기";
            case 5:
                return "방향회전베기";
            default:
                return "ERROR";
        }
    }
}