using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartCarCtrl : MonoBehaviour
{

    // 자동차 이동 속도
    public float speed = 3000.0f;

    public GameObject player;
    private PlayerCtrl Player_Script;

    //private bool getOff = false;
    private bool startSet = false;

    // Use this for initialization
    void Start()
    {
        player.GetComponent<PlayerCtrl>().enabled = false;
        Player_Script = GameObject.FindWithTag("Player").GetComponent<PlayerCtrl>();
        // 수송 차량에 앞으로 가도록 힘을 가함
        GetComponent<Rigidbody>().AddForce(transform.forward * speed);


        // 게임 시작후 차량 하차 시 인벤토리 창을 끈다.
        // 쿨타임 스크립트 할당
        Player_Script.cooltimeCtrl = GameObject.Find("PanelCoolTime").GetComponent<CoolTimeCtrl>();
        Player_Script.inventory.SetActive(false);
        Player_Script.cooltime.SetActive(false);
        Player_Script.VehicleUI.SetActive(false);


        // 안에 있는 플레이어도 같이 가도록 힘을 가함
        //player.GetComponent<Rigidbody>().AddForce(transform.forward * speed);
    }

    void Update()
    {
        if (startSet != true)
        {
            player.transform.position = new Vector3(this.transform.position.x, 30.0f, this.transform.position.z);
        }
        // F키 입력 시
        if (Input.GetKeyDown(KeyCode.F) && startSet == false)
        {
            // 플레이어에 가해진 힘을 0으로 만든다. - > 차량 하차
            //player.GetComponent<Rigidbody>().velocity = Vector3.zero;

            // 플레이어 스크립트 사용 (이동 때문)
            player.GetComponent<PlayerCtrl>().enabled = true;

            // 카메라 전환
            FollowCam followCam = GameObject.Find("Main Camera").GetComponent<FollowCam>();
            followCam.getOff = true;
            followCam.height = 35.0f;
            followCam.dist = 25.0f;

            // 차량에 하차 할때 차량의 위치로 플레이어를 이동시킨다.
            player.transform.position = new Vector3(this.transform.position.x, 30.0f, this.transform.position.z);

            // 차량 하차 후 true로 F키 입력 시 재하차 불가능하게 만듬
            startSet = true;

            //bug.Log("차량 하차 -> 게임 시작");
        }
    }

    private void OnTriggerEnter(Collider coll)
    {
        //충돌한 Collider가 Camchange의 CAMCHANGE(Tag값)이면 카메라 전환
        if (coll.gameObject.tag == "AllGetOff" && startSet == false)
        {
            // 플레이어에 가해진 힘을 0으로 만든다. - > 차량 하차
            player.GetComponent<Rigidbody>().velocity = Vector3.zero;
            // 플레이어 스크립트 사용 (이동 때문)
            player.GetComponent<PlayerCtrl>().enabled = true;

            // 차량에 하차 할때 차량의 위치로 플레이어를 이동시킨다.
            player.transform.position = new Vector3(this.transform.position.x, 30.0f, this.transform.position.z);

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
