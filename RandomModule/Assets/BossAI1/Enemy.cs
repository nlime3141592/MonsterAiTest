using  System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Entity
{
    private const int stBorn = 0;
    private const int stIdle = 1;
    private const int stReadyIdle = 2;
    private const int stReadyMove = 3;
    private const int stTrace = 4;
    private const int stJump = 5;
    private const int stAttack = 6;
    private const int stDie = 7;

    [Header("Settings")]
    public int avgFrame;
    public int rngFrame;
    public int leftFps;

    public float moveSpeed = 2.0f;
    public int maxHealth = 30;
    public int currentHealth;

    public LTRB area1;
    public LTRB area2;

    [Header("Options")]
    public int DEBUG_CURRENT_STATE;
    public int lookDir = 1;

    public bool detect1;
    public bool detect2;

    public bool isHitGround;
    public bool isHitCeil;
    public int isHitWallB;
    public int isHitWallT;
    public int isOnLedge;

    public int jumpCoolFrame = 300;
    public int leftJumpCoolFrame;

    public int attackCoolFrame = 180;
    public int leftAttackCoolFrame;

    public float jumpSpeed = 5.0f;
    // TODO: 본프로젝트에 대입한 후 사용하기.
    // public int jumpFrame = 18;
    // private int leftJumpFrame;
    // private DiscreteGraph graph;

    public Transform capturedTransform;

    private bool isEndOfState;

    private System.Random m_prng;
    private StateMachine m_machine;
    private ElongatedHexagonCollider2D col;

    private Vector2 dir_hPos;
    private Vector2 dir_hlPos;
    private Vector2 dir_hrPos;
    private Vector2 dir_fPos;
    private Vector2 dir_flPos;
    private Vector2 dir_frPos;

    private Vector2 hPos;
    private Vector2 hlPos;
    private Vector2 hrPos;
    private Vector2 fPos;
    private Vector2 flPos;
    private Vector2 frPos;

    private void UpdatePos()
    {
        hPos = transform.position + (Vector3)dir_hPos;
        hlPos = transform.position + (Vector3)dir_hlPos;
        hrPos = transform.position + (Vector3)dir_hrPos;
        fPos = transform.position + (Vector3)dir_fPos;
        flPos = transform.position + (Vector3)dir_flPos;
        frPos = transform.position + (Vector3)dir_frPos;
    }

    private void CapturePlayer()
    {
        float cx = transform.position.x + (area2.r - area2.l) * 0.5f;
        float cy = transform.position.y + (area2.t - area2.b) * 0.5f;
        float sx = area2.r + area2.l;
        float sy = area2.t + area2.b;

        Vector2 vc = new Vector2(cx, cy);
        Vector2 vs = new Vector2(sx, sy);

        Collider2D[] cols = Physics2D.OverlapBoxAll(vc, vs, 0.0f, 1 << LayerMask.NameToLayer("Entity"));

        foreach(Collider2D c in cols)
        {
            GameObject obj = c.gameObject;

            if(obj.tag == "Player")
            {
                detect2 = true;
                capturedTransform = obj.transform;

                float dx = obj.transform.position.x - cx;
                float dy = obj.transform.position.y - cy;
                float rngx = (area1.r + area1.l) * 0.5f;
                float rngy = (area1.t + area1.b) * 0.5f;

                if(dx <= rngx && dx >= -rngx && dy <= rngy && dy >= -rngy)
                    detect1 = true;
                else
                    detect1 = false;

                return;
            }
        }

        capturedTransform = null;
        detect1 = false;
        detect2 = false;
    }

private RaycastHit2D detectedGround;
public bool isDetectedGround;
    protected void CheckGround_()
    {
        CheckGroundAll(out detectedGround, out isDetectedGround, fPos, 0.5f);

        if(!isDetectedGround)
            isHitGround = false;
        else
            isHitGround = detectedGround.distance <= 0.04f;
    }

private RaycastHit2D detectedWallB;
private RaycastHit2D detectedWallT;
    protected void CheckWall_()
    {
        Vector2 fsPos = Vector2.zero;
        Vector2 hsPos = Vector2.zero;

        if(lookDir == 1)
        {
            fsPos = frPos;
            hsPos = hrPos;
        }
        else if(lookDir == -1)
        {
            fsPos = flPos;
            hsPos = hlPos;
        }

        float detectLength = 0.04f;
        base.CheckWall(out detectedWallB, out isHitWallB, fsPos, detectLength, lookDir);
        base.CheckWall(out detectedWallT, out isHitWallT, fsPos, detectLength, lookDir);
    }

private RaycastHit2D detectedGroundLower;
public bool isDetectedLedge;
    protected void CheckLedgeLower_()
    {
        Vector2 p = Vector2.zero;

        if(lookDir == 1)
            p = frPos;
        else
            p = flPos;

        float detectLength = col.shortSize * 2 + 0.04f;

        bool _d = false;
        base.CheckGroundAll(out detectedGroundLower, out _d, p, detectLength);

        isDetectedLedge = !_d;

        if(isDetectedLedge)
            isOnLedge = lookDir;
        else
            isOnLedge = 0;
    }

    #region Unity Event Functions
    protected override void Start()
    {
        base.Start();

        currentHealth = maxHealth;

        m_prng = new System.Random();
        m_machine = new StateMachine(stIdle);
        // m_machine = new StateMachine(stBorn);
        col = GetComponent<ElongatedHexagonCollider2D>();

        m_machine.SetCallbacks(stBorn, null, null, null, null);
        m_machine.SetCallbacks(stIdle, Input_Idle, Logic_Idle, null, null);
        m_machine.SetCallbacks(stReadyIdle, Input_ReadyIdle, Logic_ReadyIdle, Enter_ReadyIdle, null);
        m_machine.SetCallbacks(stReadyMove, Input_ReadyMove, Logic_ReadyMove, Enter_ReadyMove, null);
        m_machine.SetCallbacks(stTrace, Input_Trace, Logic_Trace, null, null);
        m_machine.SetCallbacks(stJump, Input_Jump, Logic_Jump, Enter_Jump, null);
        m_machine.SetCallbacks(stAttack, Input_Attack, Logic_Attack, Enter_Attack, null);
        m_machine.SetCallbacks(stDie, null, null, Enter_Die, null);

        Bounds hBounds = headBox.bounds;
        Bounds fBounds = feetBox.bounds;
        Bounds cBounds = bodyBox.bounds;
        Vector2 pPos = transform.position;

        feetBox.usedByComposite = false;
        headBox.usedByComposite = false;
        bodyBox.usedByComposite = false;

        dir_hPos.Set(hBounds.center.x - pPos.x, hBounds.max.y - pPos.y);
        dir_hlPos.Set(hBounds.min.x - pPos.x, hBounds.center.y - pPos.y);
        dir_hrPos.Set(hBounds.max.x - pPos.x, hBounds.center.y - pPos.y);
        dir_fPos.Set(fBounds.center.x - pPos.x, fBounds.min.y - pPos.y);
        dir_flPos.Set(fBounds.min.x - pPos.x, fBounds.center.y - pPos.y);
        dir_frPos.Set(fBounds.max.x - pPos.x, fBounds.center.y - pPos.y);

        feetBox.usedByComposite = true;
        headBox.usedByComposite = true;
        bodyBox.usedByComposite = true;
    }

    protected override void Update()
    {
        base.Update();

        m_machine.UpdateInput();
        DEBUG_CURRENT_STATE = m_machine.state;
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        UpdatePos();
        CapturePlayer();

        CheckGround_();
        CheckWall_();
        CheckLedgeLower_();

        m_machine.UpdateLogic();

        if(leftAttackCoolFrame > 0)
            leftAttackCoolFrame--;
        if(leftJumpCoolFrame > 0)
            leftJumpCoolFrame--;
    }
    #endregion

    #region Implement State; stBorn
    #endregion

    #region Implement State; stIdle
    private void Input_Idle()
    {
        if(currentHealth <= 0)
            m_machine.ChangeState(stDie);
        else if(detect2)
            m_machine.ChangeState(stTrace);
        else if(currentHealth < maxHealth)
        {
            int rndv = m_prng.Next(0, 2);

            if(rndv == 0)
                m_machine.ChangeState(stReadyIdle);
            else
                m_machine.ChangeState(stReadyMove);
        }
    }

    private void Logic_Idle()
    {
        float vx = 0.0f;

        SetVelocityX(vx);

        if(!isHitGround)
        {
            float vy = rigid.velocity.y;
            SetVelocityY(vy);
        }
    }
    #endregion

    #region Implement State; stReadyIdle
    private void Enter_ReadyIdle()
    {
        leftFps = (int)m_prng.RangeNextDouble(avgFrame, rngFrame);
    }

    private void Input_ReadyIdle()
    {
        if(currentHealth <= 0)
            m_machine.ChangeState(stDie);
        else if(detect2)
            m_machine.ChangeState(stTrace);
        else if(leftFps == 0)
            m_machine.ChangeState(stReadyMove);
    }

    private void Logic_ReadyIdle()
    {
        if(leftFps > 0)
        {
            leftFps--;

            float vx = 0.0f;
            float vy = 0.0f;

            if(!isHitGround)
                vy = rigid.velocity.y;

            SetVelocityXY(vx, vy);
        }
    }
    #endregion

private bool isCancelOfAction;
    #region Implement State; stReadyMove
    private void Enter_ReadyMove()
    {
        leftFps = (int)m_prng.RangeNextDouble(avgFrame, rngFrame);
        isCancelOfAction = false;

        if(isHitWallB == 0 && isHitWallT == 0 && isOnLedge == 0)
            lookDir = m_prng.Next(0, 100) < 70 ? -lookDir : lookDir;
        else if(isHitWallB != 0 && isHitWallB == lookDir)
            lookDir = -isHitWallB;
        else if(isHitWallT != 0 && isHitWallT == lookDir)
            lookDir = - isHitWallT;
        else
            lookDir = -isOnLedge;
    }

    private void Input_ReadyMove()
    {
        if(currentHealth <= 0)
            m_machine.ChangeState(stDie);
        else if(detect2)
            m_machine.ChangeState(stTrace);
        else if(isCancelOfAction ||  leftFps == 0)
            m_machine.ChangeState(stReadyIdle);
    }

    private void Logic_ReadyMove()
    {
        if(leftFps > 0)
        {
            if(
                (isOnLedge != 0 && isOnLedge == lookDir) ||
                (isHitWallB != 0 && isHitWallB == lookDir) ||
                (isHitWallT != 0 && isHitWallT == lookDir)
                )
                isCancelOfAction = true;
            else
            {
                leftFps--;
                Logic_MoveOnGround(Vector2.right, moveSpeed, lookDir);
                Debug.Log("ready move");
            }
        }
    }
    #endregion

    #region Implement State; stTrace
    private void Input_Trace()
    {
        if(currentHealth <= 0)
            m_machine.ChangeState(stDie);
        else if(detect1 && leftAttackCoolFrame == 0)
            m_machine.ChangeState(stAttack);
        else if(!detect1 && isOnLedge != 0 && leftJumpCoolFrame == 0)
            m_machine.ChangeState(stJump);
        else if(!detect2)
        {
            int rndv = m_prng.Next(0, 2);

            if(rndv == 0)
                m_machine.ChangeState(stReadyIdle);
            else
                m_machine.ChangeState(stReadyMove);
        }
    }

    private void Logic_Trace()
    {
        if(capturedTransform != null)
        {
            float dx = capturedTransform.position.x - transform.position.x;
            lookDir = dx < 0 ? -1 : 1;

            if(detect1)
                SetVelocityX(0.0f);
            else if(isOnLedge != 0 && leftJumpCoolFrame > 0)
                SetVelocityX(0.0f);
            else if(isDetectedGround)
                Logic_MoveOnGround(Vector2.Perpendicular(-detectedGround.normal).normalized, moveSpeed, lookDir);
        }
    }
    #endregion

    #region Implement State; stJump
    private void Enter_Jump()
    {
        rigid.AddForce(Vector2.up * jumpSpeed, ForceMode2D.Impulse);
        leftJumpCoolFrame = jumpCoolFrame;
    }

    private void Input_Jump()
    {
        if(currentHealth <= 0)
            m_machine.ChangeState(stDie);
        if(rigid.velocity.y <= 0.0f && isHitGround)
            m_machine.ChangeState(stTrace);
    }

    private void Logic_Jump()
    {
        Debug.Log("jump");

        // TODO:
        // x축 이동 캔슬 로직
        // 점프 처음 시작한 바닥에서 더 높은 위치의 땅을 탐지했을 때 x축 이동 캔슬
        // 의논이 필요함.

        if(isHitWallB == 0 && isHitWallT == 0)
            SetVelocityX(moveSpeed * lookDir);
    }
    #endregion

    #region Implement State; stAttack
    private void Enter_Attack()
    {
        isEndOfState = false;
        leftAttackCoolFrame = attackCoolFrame;
    }

    private void Input_Attack()
    {
        if(currentHealth <= 0)
            m_machine.ChangeState(stDie);
        else if(isEndOfState)
            m_machine.ChangeState(stTrace);
    }

    private void Logic_Attack()
    {
        Debug.Log("attack");
        isEndOfState = true;
    }

    private void End_Attack()
    {
        isEndOfState = false;
    }
    #endregion

    #region Implement State; stDie
    private void Enter_Die()
    {
        Debug.Log("die");
    }
    #endregion
}
