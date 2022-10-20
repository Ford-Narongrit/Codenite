using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;

public class MenuPanelController : MonoBehaviourPunCallbacks
{
    public void returnToStartMenu()
    {
        ConfirmUIController.Instance.showQuestion("Do you want to leave this game ?",
        () =>
        {
            PhotonNetwork.LeaveRoom();
        },
        () =>
        {
            // Do nothing
        }
    );

    }
    public override void OnLeftRoom()
    {
        SceneManager.LoadScene(SceneConfig.Home);
    }
}
