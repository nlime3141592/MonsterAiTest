using System;

public class BossAiGrid
{
    // NOTE: 구간이 n개면 분할점은 n-1개이기 때문에 +1 해줌
    public int sizeX => m_sX + 1;
    public int sizeY => m_sY + 1;

    // 분할점의 정보
    private int m_sX;
    private int m_sY;
    private float[] m_rangeXs;
    private float[] m_rangeYs;

    public BossAiGrid(float[] rangeXs, float[] rangeYs)
    {
        if(rangeXs == null || rangeYs == null)
            throw new ArgumentNullException("Argument is null.");
        if(rangeXs.Length == 0 || rangeYs.Length == 0)
            throw new ArgumentException("Empty array.");

        int i;
        int sX = rangeXs.Length;
        int sY = rangeYs.Length;

        m_sX = sX * 2 + 1;
        m_sY = sY * 2 + 1;

        m_rangeXs = new float[m_sX];
        m_rangeYs = new float[m_sY];

        m_rangeXs[sX] = 0.0f;
        m_rangeYs[sY] = 0.0f;

        for(i = 0; i < sX; i++)
        {
            m_rangeXs[i] = -rangeXs[sX - i - 1];
            m_rangeXs[sX + i + 1] = rangeXs[i];
        }

        for(i = 0; i < sY; i++)
        {
            m_rangeYs[i] = -rangeYs[sY - i - 1];
            m_rangeYs[sY + i + 1] = rangeYs[i];
        }
    }

    public int GetRX(float px)
    {
        return GetR(px, m_rangeXs, m_sX);
    }

    public int GetRY(float py)
    {
        return GetR(py, m_rangeYs, m_sY);
    }

    private int GetR(float p, float[] ranges, int s)
    {
        int i = 0;

        while(i < s && p >= ranges[i])
            i++;

        return i;
    }
}
