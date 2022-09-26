using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Boss : MonoBehaviour
{
    private const int stIdleGround = 0;
    private const int stWalk = 1;
    private const int stAttack = 2;

    public float rx1 = 0.5f;
    public float rx2 = 1.0f;
    public float ry1 = 0.5f;
    public float ry2 = 1.0f;

    private System.Random rnd;
    private BossAi1 ai;

    public int lookDir;
    public Player targetPlayer;

    private void Start()
    {
        rnd = new System.Random();
        ai = new BossAi1(rnd, rx1, rx2, ry1, ry2);
    }

    private void Update()
    {

    }

    private void FixedUpdate()
    {
        
    }

    public void EnableBoss(Player target)
    {
        targetPlayer = target;
    }

    public void DisableBoss()
    {
        targetPlayer = null;
    }

    public void OnTriggerEnter2D(Collider2D collider)
    {
        GameObject obj = collider.gameObject;

        if(obj.layer == LayerMask.NameToLayer("Entity") && collider.gameObject.tag == "Player")
        {
            Debug.Log("Recognized.");
        }
    }
}