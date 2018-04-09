using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DangerLineCtrl : MonoBehaviour
{
    private bool start = false;
    private int demage = 1;
    private int angles = 0;
    private Vector3 start_pos;
    private Vector3 limit_scale = new Vector3(4500, 4500, 4500);

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
        this.transform.eulerAngles = new Vector3(angles, 0, 0);
        ++angles;
        //if (this.transform.localScale.x >= limit_scale.x && start != false)
        //{
        //    // Limit Scale 까지 계속 줄어 든다.
        //    this.transform.localScale -= new Vector3(1.0f, 1.0f, 1.0f);
        //    this.transform.eulerAngles = new Vector3(angles, 0, 0);
        //    ++angles;
        //}
        //else if (this.transform.localScale.x <= limit_scale.x && start != false)
        //{
        //    Sendbyte = sF.DangerLine_End();
        //    Send_Packet(Sendbyte);
        //}


        //if(this.transform.localScale.x >= 0.0f)
        //{
        //    this.transform.localScale -= new Vector3(1.0f, 1.0f, 1.0f);
        //}
        //else
        //{
        //    gameObject.SetActive(false);
        //}
    }


}
