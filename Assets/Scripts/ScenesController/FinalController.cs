using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class FinalController : MonoBehaviourPunCallbacks
{
    public void OnCilckDisconnect()
    {
        Debug.Log("back to waitting room . . .");
        PhotonNetwork.LeaveRoom();
    }
    public override void OnLeftRoom()
    {
        SceneManager.LoadScene(SceneConfig.Home);
    }
}
