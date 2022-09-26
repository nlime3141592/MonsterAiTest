using System;

public class GridAI
{
    public bool isCaptured => m_captured;
    public bool isPulsed {get; private set;} = false;
    public int pulsedState {get; private set;} = -1;

    private Random m_prng;
    private GridRandom m_gprng;
    private GridPulse m_gp;
    private int[] m_wVector;

    private bool m_captured = false;
    private float m_px = 0.0f;
    private float m_py = 0.0f;
    public int m_gx = -1;
    public int m_gy = -1;

    public GridAI(Random prng, GridPoint[] points, int layers, int pulseAverage, int pulseRange)
    {
        m_prng = prng;
        m_gprng = new GridRandom(prng, points, layers);
        m_gp = new GridPulse(prng, pulseAverage, pulseRange);
        m_wVector = new int[layers];
    }

    public void UpdateLogic()
    {
        m_gp.UpdateLogic();

        if(!m_gp.isEnabled)
        {
            isPulsed = false;
            pulsedState = -1;
        }
        else
        {
            isPulsed = true;

            if(m_captured)
                pulsedState = m_gprng.GridSampling(m_gx, m_gy);
            else
                pulsedState = m_NormalSampling();
        }
    }

    public void Capture(float px, float py)
    {
        m_captured = true;
        m_px = px;
        m_py = py;
        m_gx = m_gprng.GetGX(px);
        m_gy = m_gprng.GetGY(py);
    }

    public void Uncapture()
    {
        m_captured = false;
        m_px = 0.0f;
        m_py = 0.0f;
        m_gx = -1;
        m_gy = -1;
    }

    public void SetGridWeight(int value, int gx, int gy, int layer)
    {
        m_gprng.SetWeight(value, gx, gy, layer);
    }

    public void SetNormalWeight(int value, int layer)
    {
        m_wVector[layer] = value;
    }

    private int m_NormalSampling()
    {
        int i, rndv;
        int si = -1;
        int sum = 0;
        int weight;

        for(i = 0; i < m_wVector.Length; i++)
        {
            weight = m_wVector[i];

            if(weight > 0)
            {
                sum += weight;
                rndv = m_prng.Next(0, sum);

                if(rndv < weight)
                    si = i;
            }
        }

        return si;
    }
}