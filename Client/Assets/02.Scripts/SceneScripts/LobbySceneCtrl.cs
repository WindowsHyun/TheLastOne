using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbySceneCtrl : MonoBehaviour {

    private int modeCheck = 0;

    public float gameStartTime = 1.0f;
    public Text gameStartTimeText;

    // 추후 개발 업데이트 내용
    // 상점 - 커스텀 BOX 구매 및 개봉
    // 인벤토리 - 커스텀 아이템 장착 및 탈착


    //void Start()
    //{
    //    StartCoroutine("StartGameCount"); // 대기방 씬 시작 -> 코루틴 시작
    //    //UseItemCollTime();
    //}

    public void NextInGameScene()
    {
        SceneManager.LoadScene("InGameScene");
    }


    //남은 쿨타임을 계산할 코르틴을 만들어줍니다.
    IEnumerator StartGameCount()
    {
        while (gameStartTime > 0)  // 0 초가 될때까지 while문 진행
        {
            yield return new WaitForSeconds(1.0f); // 1초 딜레이

            gameStartTime -= 1.0f;  // 1초 감소
            gameStartTimeText.text = "Start the game in " + gameStartTime; // text 출력

        }
        if (gameStartTime == 0)
        {
            NextInGameScene();
        }
        yield break;
    }


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

    public void PlayerButtonCheck()
    {
        StartCoroutine("StartGameCount"); // 대기방 씬 시작 -> 코루틴 시작
    }
}
