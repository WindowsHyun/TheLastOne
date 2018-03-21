using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemCtrl : MonoBehaviour {
    public enum ItemType { Ammunition762, Ammunition556, FirstAid } // 탄약, 구급상자

    public ItemType type;

    void OnTriggerEnter(Collider coll)
    {
        if (coll.gameObject.tag == "Player")
        {
            PlayerCtrl playerCtrl = GameObject.Find("Player").GetComponent<PlayerCtrl>();
            playerCtrl.itemEatPossible = true;
            if (type == ItemType.Ammunition556)
            {
                playerCtrl.bullet556Set = true;
            }
            else if (type == ItemType.Ammunition762)
            {
                playerCtrl.bullet762Set = true;
            }
        }
    }

    void OnTriggerExit(Collider coll)
    {
        PlayerCtrl playerCtrl = GameObject.Find("Player").GetComponent<PlayerCtrl>();
        playerCtrl.itemEatPossible = false;
        playerCtrl.bullet556Set = false;
        playerCtrl.bullet762Set = false;
    }
}
