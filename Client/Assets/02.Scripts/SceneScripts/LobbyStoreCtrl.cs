using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyStoreCtrl : MonoBehaviour {

    public GameObject StoreUI;
    public GameObject PlayButton;
    public GameObject MapButton;

    public bool StoreOnOff;

    private void Awake()
    {
        StoreUI.SetActive(false);
        StoreOnOff = false;
    }

    public void StoreButton()
    {
        if (StoreOnOff == false)
        {
            StoreUI.SetActive(true);
            StoreOnOff = true;
        }
        else if (StoreUI == true)
        {
            StoreUI.SetActive(false);
            StoreOnOff = false;
        }
        //// 2번의 경우, ZombieMode로 싱글톤 NowModeNumber에게 2을 넣어준다.
        //SingletonCtrl.Instance_S.NowModeNumber = 2;
        //SingletonCtrl.Instance_S.NowMapNumber = 2;
        //Debug.Log("현재 Zombie Mode를 선택하셨습니다 = " + SingletonCtrl.Instance_S.NowModeNumber);
    }
}
