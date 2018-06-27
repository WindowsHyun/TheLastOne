using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyStoreCtrl : MonoBehaviour {

    public GameObject storeUI;
    public GameObject costumUI;
    public GameObject playButton;
    public GameObject mapButton;

    public bool storeOnOff;

    public LobbyCostumCtrl costumCtrl;

    private void Awake()
    {
        storeUI.SetActive(false);
        storeOnOff = false;
    }

    public void StoreButton()
    {
        if (storeOnOff == false)
        {
            storeUI.SetActive(true);
            storeOnOff = true;

            costumUI.SetActive(false);
            costumCtrl.costumOnOff = false;
        }
        else if (storeUI == true)
        {
            storeUI.SetActive(false);
            storeOnOff = false;
        }
        //// 2번의 경우, ZombieMode로 싱글톤 NowModeNumber에게 2을 넣어준다.
        //SingletonCtrl.Instance_S.NowModeNumber = 2;
        //SingletonCtrl.Instance_S.NowMapNumber = 2;
        //Debug.Log("현재 Zombie Mode를 선택하셨습니다 = " + SingletonCtrl.Instance_S.NowModeNumber);
    }
}
