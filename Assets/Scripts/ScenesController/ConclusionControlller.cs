using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

public class ConclusionControlller : MonoBehaviour
{
    [Header("info")]
    [SerializeField] private float nextSceneTime = 10f;

    [Header("Object")]
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private GameObject contentview;
    [SerializeField] private GameObject playerCell;
    private float currentTime;
    private bool isFinalstage = false;
    private bool isLoading = false;
    private void Start()
    {
        currentTime = nextSceneTime;
        sendRoundData();
        foreach (KeyValuePair<int, Player> player in PhotonNetwork.CurrentRoom.Players)
        {
            if (!(bool)player.Value.CustomProperties[PlayerProperty.IsSpectator])
            {
                GameObject leader = GameObject.Instantiate(playerCell, contentview.transform);
                PlayerCell cell = leader.GetComponent<PlayerCell>();
                cell.setPlayerCell(player.Value.NickName);

                if (!(bool)player.Value.CustomProperties[PlayerProperty.IsPass])
                {
                    player.Value.CustomProperties[PlayerProperty.IsEliminate] = true;
                    cell.GetComponent<Image>().color = Color.red;
                }
            }
        }
        if ((int)PhotonNetwork.CurrentRoom.CustomProperties[RoomProperty.QuestionIndex] >= LocalQuestionList.questions.Length - 1)
        {
            Debug.Log("isfinalstage");
            isFinalstage = true;
        }
        else if (isNoPlayerleft())
        {
            isFinalstage = true;
        }
        else
        {
            loadNextLevel();
        }
    }

    private void Update()
    {
        countDown();
    }
    private void countDown()
    {
        currentTime -= Time.deltaTime;
        string tempTimer = string.Format("{0:00}", currentTime);
        timerText.text = tempTimer;

        if (currentTime <= 0)
        {
            if (isFinalstage)
            {
                goToScene(SceneConfig.Final);
            }
            else
            {
                goToScene(SceneConfig.Play);
            }
        }
    }

    private void goToScene(string scene)
    {
        if (!isLoading)
        {
            isLoading = true;
            if (!PhotonNetwork.IsMasterClient)
                return;
            PhotonNetwork.LoadLevel(scene);
        }
    }

    private void loadNextLevel()
    {
        ExitGames.Client.Photon.Hashtable customRoomProperties = new ExitGames.Client.Photon.Hashtable();
        customRoomProperties[RoomProperty.QuestionIndex] = (int)PhotonNetwork.CurrentRoom.CustomProperties[RoomProperty.QuestionIndex] + 1;
        PhotonNetwork.CurrentRoom.SetCustomProperties(customRoomProperties);
    }

    public bool isNoPlayerleft()
    {
        bool isLeft = true;
        foreach (KeyValuePair<int, Player> player in PhotonNetwork.CurrentRoom.Players)
        {
            if ((bool)player.Value.CustomProperties[PlayerProperty.IsPass] || !(bool)player.Value.CustomProperties[PlayerProperty.IsEliminate])
            {
                if (!(bool)player.Value.CustomProperties[PlayerProperty.IsSpectator])
                {
                    isLeft = false;
                }
            }
        }
        return isLeft;
    }

    public void sendRoundData()
    {
        int historyID = (int)PhotonNetwork.CurrentRoom.CustomProperties[RoomProperty.GameID];
        int questID = LocalQuestionList.getQuestion((int)PhotonNetwork.CurrentRoom.CustomProperties[RoomProperty.QuestionIndex]).id;
        int score = 0;
        int score_ed = 0;
        int fom = (int)PhotonNetwork.LocalPlayer.CustomProperties[PlayerProperty.Fom];
        int success_time = (int)PhotonNetwork.LocalPlayer.CustomProperties[PlayerProperty.SuccessTime];
        StartCoroutine(APIHelper.instance.sendRoundData(
            historyID, //history id
            questID,
            score,
            score_ed,
            fom,
            success_time,
            () =>
            {
                // if post success
            },
            () =>
            {
                // if post fail
            }
        ));
    }
}
