using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Realtime;
using Photon.Pun;
using ExitGames.Client.Photon;

public class WaitingRoomController : MonoBehaviourPunCallbacks
{
    [Header("UI Scene")]
    [SerializeField] public TextMeshProUGUI roomCodeText;
    [SerializeField] public GameObject startBtn;

    [Header("Waiting List")]
    [SerializeField] private TextMeshProUGUI playerNumText;
    [SerializeField] private Transform waitingContent;
    [SerializeField] private PlayerCell playerCellPrefab;

    [Header("Spectator List")]
    [SerializeField] private Transform spectatorContent;

    [Header("Team/SOLO List")]
    [SerializeField] private Button notReadyBtn;
    [SerializeField] private Transform readyContent;
    [SerializeField] private TeamCell teamCellPrefab;
    private List<PlayerCell> playerList = new List<PlayerCell>();
    private void Start()
    {
        try
        {
            // init local player
            PhotonNetwork.LocalPlayer.SetCustomProperties(PlayerProperty.getInitPlayerProperty());

            // init WaitingRoom
            roomCodeText.text = PhotonNetwork.CurrentRoom.Name;
            int maxPlayer = (int)PhotonNetwork.CurrentRoom.CustomProperties[RoomProperty.MaxPlayers];
            int playerInTeam = (int)PhotonNetwork.CurrentRoom.CustomProperties[RoomProperty.PlayerInTeam];

            // int maxPlayer = 20;
            // int playerInTeam = 1;

            for (int i = 0; i < maxPlayer / playerInTeam; i++)
            {
                readyContent.GetComponent<AutoResizePanel>().column = (int)(maxPlayer / playerInTeam * 0.4);
                TeamCell newTeam = Instantiate(teamCellPrefab, readyContent);
                newTeam.setTeamInfo((i + 1) + "", ColorString.Get20DifColorInIndex(i), playerInTeam);
            }

            startBtn.SetActive(PhotonNetwork.IsMasterClient);

            if (PhotonNetwork.IsMasterClient)
            {
                PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable() {
                    { PlayerProperty.Team, null },
                    { PlayerProperty.Color, null },
                    { PlayerProperty.IsSpectator, true },
                    { PlayerProperty.IsReady, true }
                });
            }
        }
        catch (System.Exception)
        {
            throw;
        }
    }
    public void onClickJoinSpectate()
    {
        PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable() {
            { PlayerProperty.Team, null },
            { PlayerProperty.Color, null },
            { PlayerProperty.IsSpectator, true },
            { PlayerProperty.IsReady, true }
        });
    }
    public void onClickNotReady()
    {
        PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable() {
            { PlayerProperty.Team, null },
            { PlayerProperty.Color, null },
            { PlayerProperty.IsReady, false },
            { PlayerProperty.IsSpectator, false }
        });
    }
    public void OnclickCopyToClipboard()
    {
        TextEditor textEditor = new TextEditor();
        textEditor.text = PhotonNetwork.CurrentRoom.Name;
        textEditor.SelectAll();
        textEditor.Copy();
    }
    public void OnClickLeave()
    {
        PhotonNetwork.LeaveRoom();
    }
    public void OnClickStart()
    {
        bool isHaveReadyPlayer = false;
        // loop check is have ready player and host is ready
        foreach (KeyValuePair<int, Player> player in PhotonNetwork.CurrentRoom.Players)
        {
            if (((bool)player.Value.CustomProperties[PlayerProperty.IsReady] && !player.Value.IsMasterClient) &&
                (bool)PhotonNetwork.MasterClient.CustomProperties[PlayerProperty.IsReady])
            {
                isHaveReadyPlayer = true;
            }
        }

        if (isHaveReadyPlayer)
        {
            // kick other player that not ready
            foreach (KeyValuePair<int, Player> player in PhotonNetwork.CurrentRoom.Players)
            {
                if (!(bool)player.Value.CustomProperties[PlayerProperty.IsReady])
                {
                    PhotonNetwork.CloseConnection(player.Value);
                }
            }
            StartCoroutine(APIHelper.instance.createHistory(
                (int)PhotonNetwork.CurrentRoom.CustomProperties[RoomProperty.ClassID],
                (int)PhotonNetwork.CurrentRoom.CustomProperties[RoomProperty.ExerciseID],
                () =>
                {
                    PhotonNetwork.CurrentRoom.IsOpen = false; // close room
                    SceneManager.LoadScene(SceneConfig.Play);
                },
                () =>
                {
                    AlertController.Instance.showAlert("Connection fail",
                        "Please try again", "Done",
                        () =>
                        {
                            // do nothing;
                        });
                    return;
                }
            ));
        }
        else
        {
            AlertController.Instance.showAlert("Can't Start Game",
                "Can not start game, until the player and host is ready.", "Done",
                () =>
                {
                    // do nothing;
                });
            return;
        }
    }

    public override void OnLeftRoom()
    {
        SceneManager.LoadScene(SceneConfig.Home);
    }
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        updateWaitingList();
    }
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        updateWaitingList();
    }
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        base.OnPlayerPropertiesUpdate(targetPlayer, changedProps);
        updateWaitingList();
    }

    private void updateWaitingList()
    {
        foreach (PlayerCell player in playerList)
        {
            Destroy(player.gameObject);
        }
        playerList.Clear();

        if (PhotonNetwork.CurrentRoom == null)
        {
            return;
        }

        foreach (KeyValuePair<int, Player> player in PhotonNetwork.CurrentRoom.Players)
        {
            if (player.Value.CustomProperties[PlayerProperty.IsReady] != null)
            {
                if ((bool)player.Value.CustomProperties[PlayerProperty.IsSpectator] && (bool)player.Value.CustomProperties[PlayerProperty.IsReady])
                {
                    PlayerCell newplayer = Instantiate(playerCellPrefab, spectatorContent);
                    newplayer.setPlayerCell(player.Value.NickName);
                    playerList.Add(newplayer);
                }
                else if (!(bool)player.Value.CustomProperties[PlayerProperty.IsReady] && player.Value.CustomProperties[PlayerProperty.Team] == null)
                {
                    PlayerCell newplayer = Instantiate(playerCellPrefab, waitingContent);
                    newplayer.setPlayerCell(player.Value.NickName);
                    playerList.Add(newplayer);
                }
            }
        }
        notReadyBtn.interactable = (bool)PhotonNetwork.LocalPlayer.CustomProperties[PlayerProperty.IsReady];
        playerNumText.text = PhotonNetwork.CurrentRoom.Players.Count + "/" + PhotonNetwork.CurrentRoom.MaxPlayers;
    }
}
