using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponCtrl : MonoBehaviour
{

    //  주무기 상태 정보가 있는 Enumerable 변수 선언
    public enum WeaponType { AK47, M16 };
    
    public WeaponType type;

        
    void OnTriggerEnter(Collider coll)
    {
        if (coll.gameObject.tag == "Player")
        {
            PlayerCtrl playerCtrl = GameObject.Find("Player").GetComponent<PlayerCtrl>();
            playerCtrl.weaponEatPossible = true;

            if (type == WeaponType.AK47)
            {
                playerCtrl.ak47Set = true;
                playerCtrl.m16Set = false;
            }
            else if (type == WeaponType.M16)
            {
                playerCtrl.m16Set = true;
                playerCtrl.ak47Set = false;
            }
        }
    }

    void OnTriggerExit(Collider coll)
    {
        PlayerCtrl playerCtrl = GameObject.Find("Player").GetComponent<PlayerCtrl>();
        playerCtrl.weaponEatPossible = false;
    }
}
