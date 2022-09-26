using System;
using System.Collections.Generic;
using UnityEngine;

public class Bocc : MonoBehaviour
{
    [Header("Settings")]
    public List<GridPoint> gPoints;
    private int stateCount = 20;
    public int avgFrame;
    public int rngFrame;
    public LTRB ltrb;

    [Header("For Debug")]
    public SpriteRenderer DEBUG_SPRITE_RENDERER;
    public int DEBUG_DETECT_COUNT;
    public bool DEBUG_IS_CAPTURED;
    public Vector2 DEBUG_GRID_INDEX;

    private System.Random m_prng;
    private GridAI m_ai;
    private Collider2D[] m_detects;
    private Collider2D m_capturedEntity;
    private Vector2 vCenter;
    private Vector2 vSize;

    private void Start()
    {
        m_prng = new System.Random();
        m_ai = new GridAI(m_prng, gPoints.ToArray(), stateCount, avgFrame, rngFrame);

        // 가중치, 열, 행, 상태 번호(레이어 번호)
        m_ai.SetGridWeight(20, 0, 3, 1);
        m_ai.SetGridWeight(20, 0, 4, 1);
        m_ai.SetGridWeight(20, 0, 5, 1);
        m_ai.SetGridWeight(20, 1, 3, 1);
        m_ai.SetGridWeight(20, 1, 4, 1);
        m_ai.SetGridWeight(20, 1, 5, 1);
        m_ai.SetGridWeight(20, 2, 3, 1);
        m_ai.SetGridWeight(20, 2, 4, 1);
        m_ai.SetGridWeight(20, 2, 5, 1);
        m_ai.SetGridWeight(10, 4, 3, 1);
        m_ai.SetGridWeight(10, 4, 4, 1);
        m_ai.SetGridWeight(70, 5, 3, 1);
        m_ai.SetGridWeight(70, 5, 4, 1);
        m_ai.SetGridWeight(70, 5, 5, 1);

        m_ai.SetGridWeight(20, 3, 3, 2);
        m_ai.SetGridWeight(20, 3, 4, 2);
        m_ai.SetGridWeight(60, 3, 5, 2);
        m_ai.SetGridWeight(20, 4, 3, 2);
        m_ai.SetGridWeight(20, 4, 4, 2);
        m_ai.SetGridWeight(50, 4, 5, 2);
        m_ai.SetGridWeight(30, 5, 3, 2);
        m_ai.SetGridWeight(30, 5, 4, 2);
        m_ai.SetGridWeight(30, 5, 5, 2);

        m_ai.SetGridWeight(50, 3, 3, 3);
        m_ai.SetGridWeight(70, 3, 4, 3);
        m_ai.SetGridWeight(40, 3, 5, 3);
        m_ai.SetGridWeight(20, 4, 3, 3);
        m_ai.SetGridWeight(30, 4, 4, 3);
        m_ai.SetGridWeight(50, 4, 5, 3);

        m_ai.SetGridWeight(30, 3, 3, 4);
        m_ai.SetGridWeight(10, 3, 4, 4);
        m_ai.SetGridWeight(50, 4, 3, 4);
        m_ai.SetGridWeight(40, 4, 4, 4);

        m_ai.SetGridWeight(80, 0, 3, 5);
        m_ai.SetGridWeight(80, 0, 4, 5);
        m_ai.SetGridWeight(80, 0, 5, 5);
        m_ai.SetGridWeight(80, 1, 3, 5);
        m_ai.SetGridWeight(80, 1, 4, 5);
        m_ai.SetGridWeight(80, 1, 5, 5);
        m_ai.SetGridWeight(80, 2, 3, 5);
        m_ai.SetGridWeight(80, 2, 4, 5);
        m_ai.SetGridWeight(80, 2, 5, 5);

        int proportion = 0;
        m_ai.SetNormalWeight(proportion, 0);
        m_ai.SetNormalWeight(proportion, 1);
        m_ai.SetNormalWeight(proportion, 2);
        m_ai.SetNormalWeight(proportion, 3);
        m_ai.SetNormalWeight(proportion, 4);
        m_ai.SetNormalWeight(proportion, 5);
    }

    private void FixedUpdate()
    {
        // 기타 필요한 업데이트
        if(DEBUG_SPRITE_RENDERER != null)
            DEBUG_SPRITE_RENDERER.transform.localScale = new Vector3(ltrb.l + ltrb.r, ltrb.t + ltrb.b, 1.0f);

        // Detect Entity
        float cx = transform.position.x + (ltrb.r - ltrb.l) * 0.5f;
        float cy = transform.position.y + (ltrb.t - ltrb.b) * 0.5f;
        float sx = ltrb.r + ltrb.l;
        float sy = ltrb.t + ltrb.b;

        vCenter.Set(cx, cy);
        vSize.Set(sx, sy);
        m_detects = Physics2D.OverlapBoxAll(vCenter, vSize, 0.0f, 1 << LayerMask.NameToLayer("Entity"));

        // Capture Entity
        bool found = false;

        foreach(Collider2D col in m_detects)
        {
            GameObject obj = col.gameObject;

            if(obj.tag == "Player")
            {
                found = true;
                float px = obj.transform.position.x - transform.position.x;
                float py = obj.transform.position.y - transform.position.y;
                m_ai.Capture(px, py);
            }
        }

        if(!found)
            m_ai.Uncapture();

        DEBUG_DETECT_COUNT = m_detects.Length;
        DEBUG_IS_CAPTURED = m_ai.isCaptured;
        DEBUG_GRID_INDEX.Set(m_ai.m_gx, m_ai.m_gy);

        if(m_ai.isCaptured) Debug.Log("AI Captured Player.");
        else Debug.Log("AI Not Captured Player.");

        // Update AI
        m_ai.UpdateLogic();

        // Check AI pulsed
        if(m_ai.isPulsed) Debug.Log("AI Pulsed.");
        // if(m_ai.pulsedState > -1) Debug.Log(string.Format("Pulsed Value: {0}", m_ai.pulsedState));
        Debug.Log(string.Format("Pulsed Value: {0}", m_ai.pulsedState));
    }
}