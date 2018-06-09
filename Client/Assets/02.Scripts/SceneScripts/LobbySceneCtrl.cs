using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

using TheLastOne.GameClass;

public class LobbySceneCtrl : MonoBehaviour
{
    Game_ProtocolClass recv_protocol = new Game_ProtocolClass();
    public float gameStartTime = 1.0f;
    public Text gameStartTimeText;
    public Image readyBtn;
    private bool readyStatus = false;

    // 추후 개발 업데이트 내용
    // 상점 - 커스텀 BOX 구매 및 개봉
    // 인벤토리 - 커스텀 아이템 장착 및 탈착
    private void Awake()
    {
        SingletonCtrl.Instance_S.PlayerStatus = recv_protocol.LobbyStatus;
        if (SingletonCtrl.Instance_S.PlayerSocket.Connected == false)
        {
            gameStartTimeText.text = "Offline Game Mode..!"; // text 출력
        }
    }

    public void NextInGameScene()
    {
        if(SingletonCtrl.Instance_S.NowMapNumber == 1)
        {
            SceneManager.LoadScene("ForestGameScene");
        }
        else if (SingletonCtrl.Instance_S.NowMapNumber == 2)
        {
            SceneManager.LoadScene("DesertGameScene");
        }

    }


    //남은 쿨타임을 계산할 코르틴을 만들어줍니다.
    IEnumerator StartGameCount()
    {
        while (gameStartTime > 0)  // 0 초가 될때까지 while문 진행
        {
            yield return new WaitForSeconds(1.0f); // 1초 딜레이

            gameStartTime -= 1.0f;  // 1초 감소
            //gameStartTimeText.text = "Start the game in " + gameStartTime; // text 출력
            gameStartTimeText.text = "Offline Game Play..!"; // text 출력

        }
        if (gameStartTime == 0)
        {
            NextInGameScene();
        }
        yield break;
    }

    IEnumerator ReadyStatus()
    {
        while (readyStatus)  // 0 초가 될때까지 while문 진행
        {
            if (SingletonCtrl.Instance_S.LobbyWaitTime == -1)
            {
                gameStartTimeText.text = "Please wait.."; // text 출력
            }
            else
            {
                gameStartTimeText.text = "Start the game in " + SingletonCtrl.Instance_S.LobbyWaitTime;
            }
            if (SingletonCtrl.Instance_S.LobbyWaitTime == 0)
            {
                NextInGameScene();
            }
            yield return new WaitForSeconds(0.2f); // 1초 딜레이
        }
        gameStartTimeText.text = "";
        yield break;
    }


    public void ForestMapCheck()
    {
        // 1번의 경우, StandardMode로 싱글톤 NowModeNumber에게 1을 넣어준다.
        SingletonCtrl.Instance_S.NowModeNumber = 1;
        SingletonCtrl.Instance_S.NowMapNumber = 1;
        Debug.Log("현재 Standard Mode를 선택하셨습니다 = " + SingletonCtrl.Instance_S.NowModeNumber);
    }

    public void DesertMapCheck()
    {
        // 2번의 경우, ZombieMode로 싱글톤 NowModeNumber에게 2을 넣어준다.
        SingletonCtrl.Instance_S.NowModeNumber = 2;
        SingletonCtrl.Instance_S.NowMapNumber = 2;
        Debug.Log("현재 Zombie Mode를 선택하셨습니다 = " + SingletonCtrl.Instance_S.NowModeNumber);
    }

    public void PlayerButtonCheck()
    {
        // 레디 상태가 아닐 경우
        if (SingletonCtrl.Instance_S.PlayerSocket.Connected == false)
        {
            // 서버와 연결이 되어있지 않을 경우...
            StartCoroutine("StartGameCount"); // 대기방 씬 시작 -> 코루틴 시작
        }
        else
        {
            // 서버와 연결이 되어있지 않을경우 레디 상태로 전환한다.
            if (readyStatus == false)
            {
                // 레디 상태가 아닐 경우
                SingletonCtrl.Instance_S.PlayerStatus = recv_protocol.ReadyStatus;
                readyBtn.color = new Color32(235, 235, 235, 255);
                readyStatus = true;
                StartCoroutine("ReadyStatus");
            }
            else
            {
                SingletonCtrl.Instance_S.PlayerStatus = recv_protocol.LobbyStatus;
                readyBtn.color = new Color32(255, 144, 0, 255);
                readyStatus = false;
            }



        }
    }
}
