using UnityEngine;

public static class BattleGizmos
{
    public static void DrawAreaLTRB(Vector2 position, LTRB ltrb, Color color)
    {
        Vector2 vc = new Vector2(position.x + ltrb.dx, position.y + ltrb.dy);
        Vector2 vs = new Vector2(ltrb.sx, ltrb.sy);
        Color clr = Gizmos.color;

        Gizmos.color = color;
        Gizmos.DrawWireCube(vc, vs);
        Gizmos.color = clr;
    }
}