using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgramManager : MonoBehaviour
{
    public double average = 0.0;
    public double sigma = 1.0;
    public double range = 2.0;
    public int loopTime = 0;
    public int loopPerFrame = 20;
    public float graphRange = 5.0f;
    public GameObject prefab;
    private Vector2 tempPos;
    private GameObject inst;

    public List<float> rXs;
    public List<float> rYs;
    public float px;
    public float py;
    public bool checker;

    private System.Random rnd;

    void Start()
    {
        rnd = new System.Random();
    }

    void NewPoint(Transform parent, float px, float py)
    {
        tempPos.Set(px, py);

        GameObject obj = GameObject.Instantiate(prefab);
        obj.transform.position = tempPos;
        obj.transform.parent = parent;
    }

    void FixedUpdate()
    {
        if(checker)
        {
            checker = false;
            BossAiGrid grid = new BossAiGrid(rXs.ToArray(), rYs.ToArray());

            int rX = grid.GetRX(px);
            int rY = grid.GetRY(py);

            Debug.Log(string.Format("Chunk Position: ({0}, {1})", rX, rY));
        }
    }
}