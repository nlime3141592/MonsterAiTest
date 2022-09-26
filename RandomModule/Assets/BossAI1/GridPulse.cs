using System;

public class GridPulse
{
    public bool isEnabled => m_enable;

    private Random m_prng;
    private int m_avg;
    private int m_rng;
    private int m_leftDelay;
    private bool m_enable;

    public GridPulse(Random prng, int average, int range)
    {
        m_prng = prng;
        m_avg = average;
        m_rng = range;
        m_leftDelay = 0;
    }

    public void UpdateLogic()
    {
        if(m_leftDelay > 0)
        {
            m_enable = false;
            m_leftDelay--;
        }
        else
        {
            m_enable = true;
            m_leftDelay = (int)m_prng.RangeNextDouble(m_avg, m_rng);
        }
    }
}