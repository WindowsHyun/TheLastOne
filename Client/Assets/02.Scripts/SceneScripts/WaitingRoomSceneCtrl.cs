using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WaitingRoomSceneCtrl : MonoBehaviour {

    public float gameStartTime = 1.0f;

    public Text gameStartTimeText;


    void Start()
    {
        StartCoroutine("HpCoolTimeCounter"); // 대기방 씬 시작 -> 코루틴 시작
        //UseItemCollTime();
    }

    public void NextInGameScene()
    {
        SceneManager.LoadScene("InGameScene");
    }


    //남은 쿨타임을 계산할 코르틴을 만들어줍니다.
    IEnumerator HpCoolTimeCounter()
    {
        while (gameStartTime > 0)  // 0 초가 될때까지 while문 진행
        {
            yield return new WaitForSeconds(1.0f); // 1초 딜레이

            gameStartTime -= 1.0f;  // 1초 감소
            gameStartTimeText.text = "Start the game in " + gameStartTime; // text 출력
           
        }
        if(gameStartTime == 0)
        {
            NextInGameScene();
        }
        yield break;
    }
}
