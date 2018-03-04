using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletCtrl : MonoBehaviour {
    // 총알의 파괴력
    public int damage = 20;

    // 총알의 발사 속도
    public float speed = 4000.0f;

    // 총알의 수명(초 단위)
    public float lifeTime = 2.0f;

    // 총알이 활성화된 뒤 경과시간을 계산하기 위한 변수
    public float _elapsedTime = 0.0f;

    void Start()
    {
        GetComponent<Rigidbody>().AddForce(transform.forward * speed);
    }

    void Update()
    {
        if(GetTimer() > lifeTime)
        {
            SetTimer();
            Destroy(gameObject);
        }
    }

    float GetTimer()
    {
        return (_elapsedTime += Time.deltaTime);
    }

    void SetTimer()
    {
        _elapsedTime = 0.0f;
    }
}
