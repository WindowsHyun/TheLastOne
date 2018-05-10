using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TheLastOne.Game.Network;

public class DieSceneCtrl : MonoBehaviour
{
    public Text playerIdText;
    NetworkCtrl networkCtrl = new NetworkCtrl();

    private void Start()
    {
        playerIdText.text = SingletonCtrl.Instance_S.PlayerID;
    }

    public void LobbyButtonClick()
    {
        SingletonCtrl.Instance_S.NowModeNumber = -1;
        SceneManager.LoadScene("LobbyGameScene");
    }
}

