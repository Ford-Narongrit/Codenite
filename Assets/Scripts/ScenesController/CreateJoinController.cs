using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class CreateJoinController : MonoBehaviourPunCallbacks
{
    [Header("UI")]
    [SerializeField] private TMP_InputField joinInputField;
    private TypedLobby custom = new TypedLobby("custom", LobbyType.Default);
    [SerializeField] private int RoomCodeLength = 6;
    private void Start()
    {
        PhotonNetwork.JoinLobby(custom);
        joinInputField.characterLimit = RoomCodeLength;
    }
    public void onClickCreate()
    {
        SceneManager.LoadScene(SceneConfig.CreateRoom);
    }
    public void OnClickJoin()
    {
        if (joinInputField.text.Length <= 0)
        {
            AlertController.Instance.showAlert("ERROR", "Please enter code room", "close", () =>
                            {
                                joinInputField.text = "";
                            });
            PhotonNetwork.JoinLobby(custom);
        }
        PhotonNetwork.JoinRoom(joinInputField.text);
    }
    public void OnClickLeave()
    {
        PhotonNetwork.LeaveLobby();
    }
    public void OnClickPaste()
    {
        TextEditor textEditor = new TextEditor();
        textEditor.multiline = true;
        textEditor.Paste();
        joinInputField.text = textEditor.text;
    }

    public void OnChangeInputJoin()
    {
        joinInputField.text = joinInputField.text.ToUpper();
    }
    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();
    }
    public override void OnLeftLobby()
    {
        base.OnLeftLobby();
        SceneManager.LoadScene(SceneConfig.Home);
    }
    public override void OnJoinedRoom()
    {
        SceneManager.LoadScene(SceneConfig.WaitingRoom);
    }
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        base.OnJoinRoomFailed(returnCode, message);
        AlertController.Instance.showAlert("ERROR", "Could not find the game you're looking for.", "close", () =>
                {
                    joinInputField.text = "";
                });
        PhotonNetwork.JoinLobby(custom);
    }
}
