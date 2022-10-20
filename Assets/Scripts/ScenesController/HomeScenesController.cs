using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

public class HomeScenesController : MonoBehaviourPunCallbacks
{
    public TextMeshProUGUI userInfoText;
    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.EnableCloseConnection = true;

        userInfoText.text = LocalUserData.name;
        PhotonNetwork.LocalPlayer.NickName = LocalUserData.name;
    }

    public void onClickPlay()
    {
        SceneManager.LoadScene(SceneConfig.SelectGame);
    }

    public void onClickTutorial()
    {
        CreateTutorialRoom();
    }

    public void CreateTutorialRoom()
    {
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 1;
        roomOptions.CustomRoomProperties = new ExitGames.Client.Photon.Hashtable();
        roomOptions.CustomRoomProperties.Add(RoomProperty.Pvp, 0);

        string randomRoomCode = RandomRoomCode(6) + LocalUserData.name;
        PhotonNetwork.CreateRoom(randomRoomCode, roomOptions);
    }
    public override void OnJoinedRoom()
    {
        SceneManager.LoadScene(SceneConfig.Tutorial);
    }

    public void onClickLogout()
    {
        LocalUserData.LogOut();
        SceneManager.LoadScene(SceneConfig.Login);
    }

    private string RandomRoomCode(int range)
    {
        string characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        string roomCode = "";

        for (int i = 0; i < range; i++)
        {
            roomCode += characters[Random.Range(0, characters.Length)];
        }
        return roomCode;
    }
}
