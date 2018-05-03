using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbySceneCtrl : MonoBehaviour {

    private int modeCheck = 0;

    // 추후 개발 업데이트 내용
    // 상점 - 커스텀 BOX 구매 및 개봉
    // 인벤토리 - 커스텀 아이템 장착 및 탈착 

    public void StandardModeCheck()
    {
        // 1번의 경우, StandardMode로 싱글톤 NowModeNumber에게 1을 넣어준다.
        SingletonCtrl.Instance_S.NowModeNumber = 1;
        Debug.Log("현재 Standard Mode를 선택하셨습니다 = " + SingletonCtrl.Instance_S.NowModeNumber);
    }

    public void ZombieModeCheck()
    {
        // 2번의 경우, ZombieMode로 싱글톤 NowModeNumber에게 2을 넣어준다.
        SingletonCtrl.Instance_S.NowModeNumber = 2;
        Debug.Log("현재 Zombie Mode를 선택하셨습니다 = " + SingletonCtrl.Instance_S.NowModeNumber);
    }

    public void NextWaitingRoomScene()
    {
        //waitScene = true;
        // 스탠다드 모드인지 좀비모드인지 구별해야 될 것이다.
        // 지금은 두개의 버튼 모두다 대기방씬으로 넘어간다.
        SceneManager.LoadScene("WaitingRoomScene");
    }
}
