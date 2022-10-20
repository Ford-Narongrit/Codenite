using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class ConnectToPhoton : MonoBehaviourPunCallbacks
{
    public TextMeshProUGUI LoadingText;
    private void Start()
    {
        LoadingText.SetText("Now Loading");
        Invoke("ConnectToPhotonNetwork", 5f);
    }
    public void ConnectToPhotonNetwork()
    {
        LoadingText.SetText("Connecting to Server");
        PhotonNetwork.ConnectUsingSettings();
    }
    public override void OnConnectedToMaster()
    {
        LoadingText.SetText("Welcome to Codenite");
        SceneManager.LoadScene(SceneConfig.Login);
    }
}
