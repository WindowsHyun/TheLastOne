using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemCtrl : MonoBehaviour {
    public enum TYPE { Ammunition, FirstAid } // 탄약, 구급상자

    public TYPE type;

    void OnTriggerEnter(Collider coll)
    {
        if (coll.gameObject.tag == "Player")
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
}
