using System;
using System.Collections.Generic;

public class BossStateSampler
{
    private List<BossWeightGrid> m_weights;
    private Random m_prng;

    public BossStateSampler(Random prng)
    {
        if(prng == null)
            throw new ArgumentNullException("PRNG cannot be null.");

        m_weights = new List<BossWeightGrid>();
        m_prng = prng;
    }

    public void AddWeight(BossWeightGrid weight)
    {
        m_weights.Add(weight);
    }

    public int GetNextState(int rX, int rY)
    {
        int i, rndv;
        int si = -1;
        int sum = 0;
        int cnt = m_weights.Count;

        for(i = 0; i < cnt; i++)
        {
            int weight = m_weights[i].GetWeight(rX, rY);

            if(weight > 0)
            {
                sum += weight;
                rndv = m_prng.Next(0, sum);
                if(rndv < weight) si = i;
            }
        }

        return si;
    }
}