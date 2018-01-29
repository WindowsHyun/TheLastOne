using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireCtrl : MonoBehaviour {

    // 총알 프리팹
    public GameObject bullet;

    // 총알 발사 좌표
    public Transform firePos;


    private Animator animator;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Fire();
        }
        //animator.SetBool("IsTrace", true);
    }

    void Fire()
    {
        // 동적으로 총알을 생성하는 함수
        CreateBullet();
    }

    void CreateBullet()
    {
        // Bullet 프리팹을 동적으로 생성
        Instantiate(bullet, firePos.position, firePos.rotation);
    }
}