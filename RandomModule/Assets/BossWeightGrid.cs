using System;

public class BossWeightGrid
{
    public int sizeX => m_sX;
    public int sizeY => m_sY;

    private int m_sX;
    private int m_sY;

    private BossAiGrid m_aiGrid;
    private int[] m_ws;

    public BossWeightGrid(BossAiGrid aiGrid)
    {
        if(aiGrid == null)
            throw new ArgumentNullException("AI Grid info is null.");

        m_sX = aiGrid.sizeX;
        m_sY = aiGrid.sizeY;

        m_aiGrid = aiGrid;
        m_ws = new int[m_sX * m_sY];
    }

    public void SetWeight(int weight, int gridX, int gridY)
    {
        if(weight < 0)
            throw new ArgumentException("Weight cannot be negative.");

        int idx = m_sX * gridY + gridX;
        m_ws[idx] = weight;
    }

    public int GetWeight(int gridX, int gridY)
    {
        int idx = m_sX * gridY + gridX;

        return m_ws[idx];
    }
}