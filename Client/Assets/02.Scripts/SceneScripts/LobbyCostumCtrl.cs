using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyCostumCtrl : MonoBehaviour
{

    public GameObject costumUI;
    public GameObject storeUI;
    public GameObject playButton;
    public GameObject mapButton;

    public bool costumOnOff;

    public LobbyStoreCtrl storeCtrl;


    // v 표시 체크 아이콘을 위함
    public GameObject[] checkIcon = new GameObject[18];
    public int[] checkIconNumber = new int[18];

    // 자물쇠 체크 아이콘을 위함
    public GameObject[] lockIcon = new GameObject[18];
    public int[] lockIconNumber = new int[18];


    public GameObject[] playerCostumeView = new GameObject[18];

    private void Awake()
    {
        costumUI.SetActive(false);
        costumOnOff = false;


        // 수 초기화
        for (int i = 0; i < 18; ++i)
        {
            checkIconNumber[i] = 0;
            lockIconNumber[i] = 0;
        }

        checkIcon[0].SetActive(true);
        checkIconNumber[0] = 1;
    }

    public void CostumButton()
    {
        if (costumOnOff == false)
        {
            costumUI.SetActive(true);
            costumOnOff = true;

            storeUI.SetActive(false);
            storeCtrl.storeOnOff = false;
        }
        else if (costumUI == true)
        {
            costumUI.SetActive(false);
            costumOnOff = false;
        }
        //// 2번의 경우, ZombieMode로 싱글톤 NowModeNumber에게 2을 넣어준다.
        //SingletonCtrl.Instance_S.NowModeNumber = 2;
        //SingletonCtrl.Instance_S.NowMapNumber = 2;
        //Debug.Log("현재 Zombie Mode를 선택하셨습니다 = " + SingletonCtrl.Instance_S.NowModeNumber);
    }

    public void WereCostumeButton(int number)
    {

        SingletonCtrl.Instance_S.WereCostumNumber = number;
        // checkIconNumber이 0이면 비활성화
        // checkIconNumber이 1이면 활성화

        for (int i = 0; i < 18; ++i)
        {
            if (i != number)
            {
                checkIconNumber[i] = 0;
                checkIcon[i].SetActive(false);
                
                playerCostumeView[i].SetActive(false);

            }
            else if (i == number)
            {
                checkIconNumber[i] = 1;
                checkIcon[i].SetActive(true);            

                playerCostumeView[i].SetActive(true);
            }
        }
    }
}
