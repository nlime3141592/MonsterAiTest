using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleModule : MonoBehaviour
{
    public const int MAX_DETECT_COUNT = 48;
    private Collider2D[] m_dctCols;
    private int m_dctColCnt;
    private BattleModule[] m_dctMods;
    private int m_dctModCnt;

    [Header("Module Properties")]
    public BattleStat stat;
    public BattleData bdat;
    
    // Detect options
    public LTRB areaD2;
    public LTRB areaD1;
    public LayerMask targetLayer;
    public List<string> targetTags;

    public Entity entity => m_entity;
    private Entity m_entity;

    // Event Handlers
    private delegate void BattleEventHandler<T>(BattleModule pubModule, T evdat);
    private BattleEventHandler<DieEventData> m_onDie;
    private BattleEventHandler<DamageEventData> m_onDamage;
    private BattleEventHandler<HealEventData> m_onHeal;
    private BattleEventHandler<ExpenseEventData> m_onExpense;
    private BattleEventHandler<ChargeEventData> m_onCharge;

    private void RegisterEvents()
    {
        m_onDamage += m_entity.OnDamage;
        m_onDie +=  m_entity.OnDie;
    }

    #region Unity Event Functions
    private void Start()
    {
        m_dctCols = new Collider2D[MAX_DETECT_COUNT];
        m_dctMods = new BattleModule[MAX_DETECT_COUNT];
        m_entity = GetComponent<Entity>();

        RegisterEvents();

        if(randomizeLTRB)
        {
            System.Random prng = new System.Random();
            areaD2.Randomize(prng, 3.0f, 5.0f);
            areaD1.Randomize(prng, 1.0f, 3.0f);
        }
    }

    private void FixedUpdate()
    {
        if(AutoCheckDie())
            return;

        m_Detect();
        mods = new List<BattleModule>(m_dctMods);

        AutoAttack();
    }
    #endregion

    #region Automatic Executor
    private void AutoAttack()
    {
        if(bdat.attackTimes == 0)
            bdat.attackFrameCur = 0;
        else if(bdat.attackTimes < 0) // NOTE: 오류 방지
            bdat.attackTimes = 0;
        else if(bdat.attackFrameCur > 0)
            bdat.attackFrameCur--;
        else
        {
            int i;
            float damage = stat.maxPhysicalPower; // TODO: 현재는 max로 설정했지만 후에 min과 max 사이 랜덤한 값을 불러올 수 있도록 해야 함.

            DamageEventData evdat = new DamageEventData();
            evdat.damage = damage;
            evdat.attacker = this;

            for(i = 0; i < m_dctModCnt; i++)
                m_dctMods[i].m_Damage(evdat);

            bdat.attackFrameCur = bdat.attackFrame;
            bdat.attackTimes--;
        }
    }

    private bool AutoCheckDie()
    {
        bool curDie = stat.health <= 0;

        if(!bdat.isDie && curDie)
        {
            DieEventData evdat = new DieEventData();

            m_Die(evdat);

            bdat.isDie = true;
        }

        return curDie;
    }
    #endregion

    #region Stat Change Event Functions
    private void m_Detect()
    {
        float px = transform.position.x;
        float py = transform.position.y;
        Vector2 vc = new Vector2(px + areaD2.dx, py + areaD2.dy);
        Vector2 vs = new Vector2(areaD2.sx, areaD2.sy);

        int dctColCnt = Physics2D.OverlapBoxNonAlloc(vc, vs, 0.0f, m_dctCols, targetLayer);
        int dctModCnt = FilterBattleModule(m_dctCols, dctColCnt, m_dctMods);

        m_dctColCnt = dctColCnt;
        m_dctModCnt = dctModCnt;
    }

    private void m_Damage(DamageEventData evdat)
    {
        evdat.baseHealth = stat.health;
        stat.ChangeHealth(-evdat.damage);
        evdat.currentHealth = stat.health;
        evdat.deltaHealth = evdat.currentHealth - evdat.baseHealth;
        evdat.victim = this;

        if(m_onDamage != null)
            m_onDamage(this, evdat);
    }

    private void m_Heal(HealEventData evdat)
    {
        evdat.baseHealth = stat.health;
        stat.ChangeHealth(evdat.healAmount);
        evdat.currentHealth = stat.health;
        evdat.deltaHealth = evdat.currentHealth - evdat.baseHealth;
        evdat.healee = this;

        if(m_onHeal != null)
            m_onHeal(this, evdat);
    }

    private void m_Expense(ExpenseEventData evdat)
    {
        evdat.baseMana = stat.mana;
        stat.ChangeMana(-evdat.expenseAmount);
        evdat.currentMana = stat.mana;
        evdat.deltaMana = evdat.currentMana - evdat.baseMana;
        evdat.customer = this;

        if(m_onExpense != null)
            m_onExpense(this, evdat);
    }

    private void m_Charge(ChargeEventData evdat)
    {
        evdat.baseMana = stat.mana;
        stat.ChangeMana(evdat.chargeAmount);
        evdat.currentMana = stat.mana;
        evdat.deltaMana = evdat.currentMana - evdat.baseMana;
        evdat.chargee = this;

        if(m_onCharge != null)
            m_onCharge(this, evdat);
    }

    private void m_Die(DieEventData evdat)
    {
        if(m_onDie != null)
            m_onDie(this, evdat);
    }
    #endregion

    #region Sub Algorithms
    // NOTE: For Detect() Function.
    private int FilterBattleModule(Collider2D[] dctCols, int dctColCnt, BattleModule[] dctMods)
    {
        GameObject obj;
        BattleModule mod;
        int i, j;
        bool cmpFound;
        bool contains;
        int dctModCnt = 0;

        for(i = 0; i < MAX_DETECT_COUNT; i++)
        {
            dctMods[i] = null;

            if(i >= dctColCnt)
                continue;

            obj = dctCols[i].gameObject;
            cmpFound = obj.TryGetComponent<BattleModule>(out mod);

            if(!targetTags.Contains(obj.tag) || !cmpFound)
                continue;

            contains = false;

            for(j = 0; j < dctModCnt && !contains; j++)
                contains = (dctMods[j] == mod);

            if(!contains)
                dctMods[dctModCnt++] = mod;
        }

        return dctModCnt;
    }
    #endregion

    #region Predefined Exceptions
    private void CheckNegativeException(float value)
    {
        if(value < 0.0f)
            throw new ArgumentException("Value cannot be negative.");
    }
    #endregion

    #region Debug Options
    [Header("Debug Options")]
    public bool randomizeLTRB;
    public List<BattleModule> mods;

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        BattleGizmos.DrawAreaLTRB(transform.position, areaD2, Color.cyan);
        BattleGizmos.DrawAreaLTRB(transform.position, areaD1, Color.red);
    }
#endif
    #endregion
}