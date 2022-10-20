using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class TeamCell : MonoBehaviourPunCallbacks
{
    [Header("UI")]
    [SerializeField] public TextMeshProUGUI teamNameText;
    [SerializeField] public Image teamColorImage;
    [SerializeField] public Transform playerContent;
    [SerializeField] public Button joinBtn;
    [SerializeField] public PlayerCell playerCellPrefab;

    [Header("info")]
    public string teamName;
    public Color teamColor;
    public int playerInTeam = 1;
    private List<PlayerCell> playerList = new List<PlayerCell>();

    public void OnClickJoin()
    {
        PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable() {
            { PlayerProperty.Team, teamName },
            { PlayerProperty.Color, ColorString.GetStringFromColor(teamColor) },
            { PlayerProperty.IsSpectator, false },
            { PlayerProperty.IsReady, true },
        });
    }

    public void setTeamInfo(string _teamName, Color _color, int _playerInTeam)
    {
        teamColor = _color;
        teamColorImage.color = _color;

        playerInTeam = _playerInTeam;

        teamName = _teamName;
        teamNameText.text = _teamName;
    }

    private void FixedUpdate()
    {
        if (playerList.Count >= playerInTeam)
        {
            joinBtn.interactable = false;
        }
        else
        {
            joinBtn.interactable = true;
        }
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        base.OnPlayerPropertiesUpdate(targetPlayer, changedProps);
        updateTeam();
    }

    private void updateTeam()
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
            if (player.Value.CustomProperties.ContainsKey(PlayerProperty.Team))
            {
                if ((string)player.Value.CustomProperties[PlayerProperty.Team] == teamName)
                {
                    PlayerCell newPlayer = Instantiate(playerCellPrefab, playerContent);
                    newPlayer.setPlayerCell(player.Value.NickName);
                    playerList.Add(newPlayer);
                }
            }
        }
    }
}
