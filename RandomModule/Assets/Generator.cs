using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Generator : MonoBehaviour
{
    private List<Dot> dots;
    public Dot prefab;
    public Transform parent;
    public List<CircleOption> options;
    private int pixel_xb = 0;
    private int pixel_yb = 0;
    private float scaleb = 0.0f;
    private AiMode modeb;
    public int pixel_x = 100;
    public int pixel_y = 100;
    public float scale = 0.1f;
    public AiMode mode;

    void Start()
    {
        dots = new List<Dot>();
    }

    void FixedUpdate()
    {
        if(prefab == null)
            return;

        if(pixel_x < 1 || pixel_y < 1 || scale < 0.001f)
            return;

        if(pixel_xb != pixel_x || pixel_yb != pixel_y || scaleb != scale || modeb != mode)
        {
            OnChangeResolution();
            OnChangeScale();
            OnChangeDot();
        }

        pixel_xb = pixel_x;
        pixel_yb = pixel_y;
        scaleb = scale;
    }

    void Update()
    {
        
    }

    private void OnChangeResolution()
    {
        int count = pixel_x * pixel_y;
        int i;

        while(dots.Count < count)
            dots.Add(NewDot());

        for(i = 0; i < dots.Count; i++)
            dots[i].gameObject.SetActive(i < count);
    }

    private void OnChangeScale()
    {
        int idx1d;
        int i, j;

        Vector3 sc = new Vector3(scale, scale, 1.0f);
        Vector3 p = Vector3.zero;
        Transform ts;

        for(i = 0; i < pixel_y; i++)
        for(j = 0; j < pixel_x; j++)
        {
            idx1d = j + i * pixel_x;

            p.Set(j * scale, i * scale, 0.0f);
            ts = dots[idx1d].transform;

            ts.position = p;
            ts.localScale = sc;
        }
    }

    private void OnChangeDot()
    {
        int count = pixel_x * pixel_y;
        int i;

        for(i = 0; i < count; i++)
            dots[i].OnChange(options, mode);
    }

    private Dot NewDot()
    {
        GameObject dot = GameObject.Instantiate(prefab.gameObject);
        dot.transform.parent = parent;

        return dot.GetComponent<Dot>();
    }
}
