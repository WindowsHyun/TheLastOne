using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartCarCtrl : MonoBehaviour
{

    // 자동차 이동 속도
    public float speed = 3000.0f;

    public GameObject player;

    //private bool getOff = false;
    private bool startSet = false;

    // Use this for initialization
    void Start()
    {
        player.GetComponent<PlayerCtrl>().enabled = false;
        // 수송 차량에 앞으로 가도록 힘을 가함
        GetComponent<Rigidbody>().AddForce(transform.forward * speed);
        
        // 안에 있는 플레이어도 같이 가도록 힘을 가함
        player.GetComponent<Rigidbody>().AddForce(transform.forward * speed);
    }

    void Update()
    {
        // F키 입력 시
        if (Input.GetKeyDown(KeyCode.F) && startSet == false)
        {
            // 플레이어에 가해진 힘을 0으로 만든다. - > 차량 하차
            player.GetComponent<Rigidbody>().velocity = Vector3.zero;

            // 플레이어 스크립트 사용 (이동 때문)
            player.GetComponent<PlayerCtrl>().enabled = true;

            // 카메라 전환
            FollowCam followCam = GameObject.Find("Main Camera").GetComponent<FollowCam>();
            followCam.getOff = true;
            followCam.height = 35.0f;
            followCam.dist = 25.0f;

            // 차량의 포지션을 내리기 전까지 플레이어 포지션과 동일하게 이동을 시킨다.
            player.transform.position = new Vector3(transform.position.x, 29.99451f, transform.position.z);

            // 차량 하차 후 true로 F키 입력 시 재하차 불가능하게 만듬
            startSet = true;

            //bug.Log("차량 하차 -> 게임 시작");
        }

    }

    private void OnTriggerEnter(Collider coll)
    {
        //충돌한 Collider가 Camchange의 CAMCHANGE(Tag값)이면 카메라 전환
        if (coll.gameObject.tag == "AllGetOff")
        {
            // 플레이어에 가해진 힘을 0으로 만든다. - > 차량 하차
            player.GetComponent<Rigidbody>().velocity = Vector3.zero;
            // 플레이어 스크립트 사용 (이동 때문)
            player.GetComponent<PlayerCtrl>().enabled = true;
            // 카메라 전환
            FollowCam followCam = GameObject.Find("Main Camera").GetComponent<FollowCam>();
            followCam.getOff = true;
            followCam.height = 35.0f;
            followCam.dist = 25.0f;

            // 차량 하차 후 true로 F키 입력 시 재하차 불가능하게 만듬
            startSet = true;

            //bug.Log("차량 하차 -> 게임 시작");
        }

        if (coll.gameObject.tag == "EndPoint")
        {
            Destroy(gameObject);
        }
    }
}
