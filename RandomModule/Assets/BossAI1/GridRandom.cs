using System;

public class GridRandom
{
    private Random m_prng;

    private GridPoint[] m_gPoints;
    private int m_gridSize;

    private int[,,] m_wFields;
    private int m_layerCount;

    public GridRandom(Random prng, GridPoint[] points, int layers)
    {
        m_prng = prng;
        m_NewGridPoints(points);
        m_NewWeightFields(layers);
    }

    public int GridSampling(int gx, int gy)
    {
        int i, rndv;
        int si = -1;
        int sum = 0;
        int weight;

        for(i = 0; i < m_layerCount; i++)
        {
            weight = m_wFields[i, gx, gy];

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

    public int GetGX(float px)
    {
        int i = 0;

        while(i < m_gridSize - 1 && px >= m_gPoints[i].x)
            i++;

        return i;
    }

    public int GetGY(float py)
    {
        int i = 0;

        while(i < m_gridSize - 1 && py >= m_gPoints[i].y)
            i++;

        return i;
    }

    public void SetWeight(int value, int gx, int gy, int layer)
    {
        m_wFields[layer, gx, gy] = value;
    }

    public int GetWeight(int gx, int gy, int layer)
    {
        return m_wFields[layer, gx, gy];
    }

    private void m_NewGridPoints(GridPoint[] points)
    {
        int i, l;
        l = points.Length;
        m_gridSize = l + 1;

        m_gPoints = new GridPoint[l];

        for(i = 0; i < l; i++)
            m_gPoints[i] = points[i];
    }

    private void m_NewWeightFields(int layers)
    {
        m_layerCount = layers;
        m_wFields = new int[layers, m_gridSize, m_gridSize];
    }
}