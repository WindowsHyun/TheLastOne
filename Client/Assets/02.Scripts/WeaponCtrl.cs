using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponCtrl : MonoBehaviour {

    //  주무기 상태 정보가 있는 Enumerable 변수 선언
    public enum WeaponState { AK47, M4A1 };

    // 캐릭터의 주무기 정보를 저장할 Enum 변수
    //public WeaponState weaponState = WeaponState.AK47;

   
    public GameObject AK47;

    public bool showItem1;
    

    void Start()
    {
        showItem1 = false;  
    }

    void Update()
    {
        if (showItem1 == true)
            AK47.GetComponent<Renderer>().enabled = true;
        else if (showItem1 == false)
            AK47.GetComponent<Renderer>().enabled = false;


        if (Input.GetKeyDown(KeyCode.Space))
        {
            if(showItem1 == true)
                showItem1 = false;
            else if(showItem1 == false)
                showItem1 = true;

            Debug.Log("1번 누름");
        }
    }



    //   GameObject player;
    //   GameObject playerEquipPoint;
    //   PlayerCtrl playerLogic;

    //   Vector3 forceDirection;
    //   bool isPlayerEnter;


    //   void Awake()
    //   {
    //       player = GameObject.FindGameObjectWithTag("Player");
    //       playerEquipPoint = GameObject.FindGameObjectWithTag("EquipPoint");

    //       playerLogic = player.GetComponent<PlayerCtrl>();
    //   }

    //   void OnTriggerEnter(Collider other)
    //   {
    //       if(other.gameObject == player)
    //       {
    //           isPlayerEnter = true;
    //       }
    //   }

    //   void OnTriggerExit(Collider other)
    //   {
    //       if(other.gameObject == player)
    //       {
    //           isPlayerEnter = false;
    //       }
    //   }


    //   void Start () {

    //}

    //void Update () {

    //}
}
