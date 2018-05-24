using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DangerLineCtrl : MonoBehaviour
{
    private bool start;
    private int demage;
    private Vector3 start_pos;
    private Vector3 limit_scale;
    public Image dangerLineImage;
    public Transform minimapDangerLine;
    public bool getDangerLine = true;

    //private Vector3 limit_scale = new Vector3(2700, 2700, 2700);

    private void Awake()
    {
        start = false;
        demage = 1;
        //limit_scale = new Vector3(4500, 4500, 4500);

        // 지도 위의 자기장 이미지를 DangerLineBall 위치와 동기화
        dangerLineImage.GetComponent<RectTransform>().localPosition = new Vector2(-this.transform.position.z * 0.5f, this.transform.position.x * 0.5f);
        // 지도 위의 자기장 이미지를 DangerLineBall의 크기의 비율에 맞게 설정
        dangerLineImage.GetComponent<RectTransform>().sizeDelta = new Vector2(this.transform.localScale.x * 0.5f, this.transform.localScale.y * 0.5f);
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
        dangerLineImage.GetComponent<RectTransform>().sizeDelta = new Vector2(limit_scale.x / 2, limit_scale.z / 2);
        minimapDangerLine.localScale = new Vector3(limit_scale.x, 250, limit_scale.z);
    }


}
