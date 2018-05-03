using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingletonCtrl : MonoBehaviour {

    private int nowModeNumber = 0;
    private int playerMoney = 0;    // 플레이어 돈
    private string playerID = "";   // 플레이어 아이디
    private string playerPWD = "";  // 플레이어 패스워드
 

    private static SingletonCtrl instance_S = null; // 정적 변수
   

    public static SingletonCtrl Instance_S  // 인스턴스 접근 프로퍼티
    {
        get
        {
            return instance_S;
        }
    }

    public int NowModeNumber                  // 플레이어 현재 모드 접근 프로퍼티
    {
        get
        {
            return nowModeNumber;
        }
        set
        {
            nowModeNumber = value;
        }
    }

    public int PlayerMoney                  // 플레이어 돈 접근 프로퍼티
    {
        get
        {
            return playerMoney;
        }
        set
        {
            playerMoney = value;
        }
    }

    public string PlayerID                  // 플레이어 아이디 접근 프로퍼티
    {
        get
        {
            return playerID;
        }
        set
        {
            playerID = value;
        }
    }

    public string PlayerPWD                 // 플레이어 패스워드 접근 프로퍼티
    {
        get
        {
            return playerPWD;
        }
        set
        {
            playerPWD = value;
        }
    }

    private void Awake()
    {
        if (instance_S)                     // 인스턴스가 이미 생성 되었는가?
        {
            DestroyImmediate(gameObject);   // 또 만들 필요가 없다 -> 삭제
            return;
        }
        instance_S = this;                  // 유일한 인스턴스로 만듬
        DontDestroyOnLoad(gameObject);      // 씬이 바뀌어도 계속 유지 시킨다
    }

    // Use this for initialization
    void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
