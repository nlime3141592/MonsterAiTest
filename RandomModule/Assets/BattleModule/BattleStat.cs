using System;

[Serializable]
public struct BattleStat
{
    public float maxHealth;
    public float health;

    public float maxMana;
    public float mana;

    public float minPhysicalPower;
    public float maxPhysicalPower;

    public void ChangeHealth(float h)
    {
        float th = MathM.Mid3(0.0f, health + h, maxHealth);
        health = th;
    }

    public void ChangeMana(float m)
    {
        float tm = MathM.Mid3(0.0f, mana + m, maxMana);
        mana = tm;
    }
}