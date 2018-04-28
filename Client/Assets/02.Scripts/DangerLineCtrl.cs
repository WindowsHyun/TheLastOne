using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DangerLineCtrl : MonoBehaviour
{
    private bool start;
    private int demage;
    private Vector3 start_pos;
    private Vector3 limit_scale;
    //private Vector3 limit_scale = new Vector3(2700, 2700, 2700);

    private void Awake()
    {
        start = false;
        demage = 1;
        limit_scale = new Vector3(4500, 4500, 4500);
    }

    public void set_start(bool value)
    {
        this.start = value;
    }

    public void set_demage(int value)
    {
        this.demage = value;
    }

    public void set_pos(Vector3 pos)
    {
        this.start_pos = pos;
    }

    
    public void set_scale(Vector3 scale)
    {
        this.limit_scale = scale;
    }

    void FixedUpdate()
    {
        this.transform.localScale = limit_scale;
    }


}
