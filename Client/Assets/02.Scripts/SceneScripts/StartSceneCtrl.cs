using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartSceneCtrl: MonoBehaviour {

    private bool idCehck = false;               // 아이디 입력 확인을 위함
    private bool pwdCehck = false;              // 패스워드 입력 확인을 위함


    //// Update is called once per frame
    //void Update () {

    //}

    public void InputTextID(InputField id)
    {
        if (id.text != "" && id.text != " ")
        {
            Debug.Log("당신의 ID는 " + id.text + " 입니다"); // 아이디 확인을 위한 로그
            SingletonCtrl.Instance_S.PlayerID = id.text; // 싱글톤 PlayerID에 id.text를 넘겨준다.
            Debug.Log(SingletonCtrl.Instance_S.PlayerID); // 싱글톤 PlayerID에 값이 정확하게 왔는지 체크 
            idCehck = true; // 입력 확인이 되었으므로 true로 변경
        }
    }

    public void InputTextPWD(InputField pwd)
    {
        if (pwd.text != "" && pwd.text != " ")
        {
            Debug.Log("당신의 PWD는 " + pwd.text + " 입니다"); // 아이디 확인을 위한 로그
            SingletonCtrl.Instance_S.PlayerPWD = pwd.text; // 싱글톤 PlayerPWD에 pwd.text를 넘겨준다
            Debug.Log(SingletonCtrl.Instance_S.PlayerPWD); // 싱글톤 PlayerPWD에 값이 정확하게 왔는지 체크 
            pwdCehck = true; // 입력 확인이 되었으므로 true로 변경
        }
    }


    public void NextLobbyScene()
    {
        if (idCehck == true && pwdCehck == true)  // 모두 입력 하였는지 확인
        {
            SceneManager.LoadScene("LobbyGameScene"); // 다음씬으로 넘어감
        }
    }


}
