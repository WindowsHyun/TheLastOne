using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponCtrl : MonoBehaviour
{

    //  주무기 상태 정보가 있는 Enumerable 변수 선언
    public enum WeaponState { AK47 };

    // 캐릭터의 주무기 정보를 저장할 Enum 변수
    //public WeaponState weaponState = WeaponState.AK47;

    void OnTriggerEnter(Collider coll)
    {
        if(coll.gameObject.tag == "Player")
        {
            PlayerCtrl playerCtrl = GameObject.Find("Player").GetComponent<PlayerCtrl>();
            playerCtrl.itemEatPossible = true;
        }
    }

    void OnTriggerExit(Collider coll)
    {
        PlayerCtrl playerCtrl = GameObject.Find("Player").GetComponent<PlayerCtrl>();
        playerCtrl.itemEatPossible = false;
    }

    //void OnTriggerEnter(Collider coll)
    //{
    //    if (coll.gameObject.tag == "Player")
    //    {
    //        if (Input.GetKeyDown(KeyCode.G))
    //        {
    //            Debug.Log("오김옴?");
    //            PlayerCtrl playerCtrl = GameObject.Find("Player").GetComponent<PlayerCtrl>();
    //            playerCtrl.itemEat = true;

    //            Destroy(gameObject);
    //        }
    //    }
    //}



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
}
