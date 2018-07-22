using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartSceneCtrl : MonoBehaviour
{

    private bool idCehck = false;               // 아이디 입력 확인을 위함
    private bool pwdCehck = false;              // 패스워드 입력 확인을 위함
    private bool ipCehck = false;              // 패스워드 입력 확인을 위함
    private string pwdtext = "";

    public InputField input_id;
    public InputField input_pw;
    public InputField input_ip;
    public Image buttonimage;
    public GameObject UI_Message;
    public Text UI_Text;

    private void Awake()
    {
        input_id.Select();
        UI_Message.SetActive(false);
    }

    private void Update()
    {
        if (input_id.isFocused == true)
        {
            if (Input.GetKeyUp(KeyCode.Tab))
            {
                input_pw.Select();
            }
        }

        if (input_pw.isFocused == true)
        {
            if (Input.GetKeyUp(KeyCode.Tab))
            {
                input_ip.Select();
            }
            if (Input.GetKeyUp(KeyCode.Return))
            {
                NextLobbyScene();
            }
        }
    }

    public void InputTextID(InputField id)
    {
        if (id.text != "" && id.text != " ")
        {
            //Debug.Log("당신의 ID는 " + id.text + " 입니다"); // 아이디 확인을 위한 로그
            SingletonCtrl.Instance_S.PlayerID = id.text; // 싱글톤 PlayerID에 id.text를 넘겨준다.
            //Debug.Log(SingletonCtrl.Instance_S.PlayerID); // 싱글톤 PlayerID에 값이 정확하게 왔는지 체크 
            idCehck = true; // 입력 확인이 되었으므로 true로 변경
        }
    }

    public void InputTextPWD(InputField pw)
    {
        if (pw.text != "" && pw.text != " ")
        {
            pwdtext = pw.text;  // 비밀번호는 싱글톤에 저장하지 않는다.
            pwdCehck = true;
        }
    }

    public void InputTextIP(InputField ip)
    {
        if (ip.text != "" && ip.text != " ")
        {
            //Debug.Log("당신의 IP는 " + ip.text + " 입니다"); // 아이디 확인을 위한 로그
            SingletonCtrl.Instance_S.PlayerIP = ip.text; // 싱글톤 PlayerPWD에 pwd.text를 넘겨준다
            Debug.Log(SingletonCtrl.Instance_S.PlayerIP); // 싱글톤 PlayerPWD에 값이 정확하게 왔는지 체크 
            ipCehck = true; // 입력 확인이 되었으므로 true로 변경
        }
    }

    public void UI_MessageOkay()
    {
        UI_Message.SetActive(false);
    }


    public void NextLobbyScene()
    {
        if (idCehck == true && pwdCehck == true)  // 모두 입력 하였는지 확인
        {
            // 로그인 테스트 시작!
            bool logincheck = SingletonCtrl.Instance_S.loginWebServer(SingletonCtrl.Instance_S.PlayerID, pwdtext);

            if (logincheck == true)
            {
                // 소켓 연결을 위하여
                SingletonCtrl.Instance_S.NowModeNumber = 0;
                SceneManager.LoadScene("LobbyGameScene"); // 다음씬으로 넘어감
            }
            else
            {
                // 로그인 오류 메시지를 표시 해준다.
                UI_Text.text = SingletonCtrl.Instance_S.ResultContent;
                UI_Message.SetActive(true);
            }
        }
        else
        {
            UI_Text.text = "Please enter your ID or Password.";
            UI_Message.SetActive(true);
        }
    }

    public void ExitGame()
    {
        Application.Quit();
    }


}
