using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class GameController : MonoBehaviourPunCallbacks
{
    [Header("Player spawner")]
    [SerializeField] private GameObject playerPrefabs;
    [SerializeField] private Transform playerSpawnpoint;

    [Header("Map setup")]
    [SerializeField] private Zone[] zones;

    [Header("Rule")]
    [SerializeField] private TextMeshProUGUI timer;
    [SerializeField] private TextMeshProUGUI quotaText;
    [SerializeField] private float gameTime = 300f;
    [SerializeField] private float hintTime = 60f;
    [SerializeField] private float quotaPercent;
    [SerializeField] private int minPlayerToFindWinner = 1;
    private int quota = 1;
    private int qualifiedPlayer = 0;
    public float currentTime = 0.1f;
    private bool isGameStart = false;
    private bool gameOver = false;
    private int questionIndex = 0;

    private void Start()
    {
        gameTime = (int)PhotonNetwork.CurrentRoom.CustomProperties[RoomProperty.Timelimit] * 60;
        int exercisID = (int)PhotonNetwork.CurrentRoom.CustomProperties[RoomProperty.ExerciseID];
        questionIndex = (int)PhotonNetwork.CurrentRoom.CustomProperties[RoomProperty.QuestionIndex];
        StartCoroutine(APIHelper.instance.getQuestionAnswer(exercisID,
        () =>
        {
            //success
            initLocalPlayer();
            initZone();
            initRule();
            isGameStart = true;
        },
        () =>
        {
            //fail
        }
        ));
    }
    private void initLocalPlayer()
    {
        PhotonNetwork.LocalPlayer.CustomProperties[PlayerProperty.IsPass] = false;
        PhotonNetwork.LocalPlayer.CustomProperties[PlayerProperty.Fom] = 0;
        PhotonNetwork.LocalPlayer.CustomProperties[PlayerProperty.SuccessTime] = 0;

        GameObject player = PhotonNetwork.Instantiate(playerPrefabs.name, playerSpawnpoint.position, Quaternion.identity);
        CinemachineVirtualCamera cv = GameObject.FindGameObjectWithTag("PlayerFollowCamera").GetComponent<CinemachineVirtualCamera>();
        cv.Follow = player.transform;

        //set player ui
        PlayerUIController uIController = (PlayerUIController)GameObject.FindObjectOfType(typeof(PlayerUIController));
        uIController.player = player.GetComponent<PlayerController>();

        //set codepanel
        CodePanelController codePanel = uIController.codePanel.GetComponent<CodePanelController>();
        codePanel.player = player.GetComponent<PlayerController>();

        Question question = LocalQuestionList.getQuestion(questionIndex);
        codePanel.setProblemInfo(
            question.question,
            question.result,
            question.correctAnswer.ToList()
        );

        if ((bool)PhotonNetwork.LocalPlayer.CustomProperties[PlayerProperty.IsSpectator] || (bool)PhotonNetwork.LocalPlayer.CustomProperties[PlayerProperty.IsEliminate])
        {
            player.GetPhotonView().RPC("destroy", RpcTarget.All);
            uIController.IsSpectator = true;
        }
    }
    public void initZone()
    {
        string[] answers = LocalQuestionList.getQuestion(questionIndex).answers;
        for (int i = 0; i < answers.Length; i++)
        {
            zones[i].initZone(answers[i]);
        }
    }

    private void initRule()
    {
        //init player
        int totalPlayer = qualifiedTeam();
        if (totalPlayer > minPlayerToFindWinner)
        {
            quota = (int)Mathf.Ceil(totalPlayer * (quotaPercent / 100));
        }
        else
        {
            quota = minPlayerToFindWinner;
        }

        //init time
        currentTime = gameTime;
    }

    private void LateUpdate()
    {
        if (isGameStart)
        {
            countDown();
            quotaText.text = qualifiedPlayer + " / " + quota;
        }
    }

    private void countDown()
    {
        currentTime -= Time.deltaTime;
        TimeSpan timeSpan = TimeSpan.FromSeconds(currentTime);
        string tempTimer = string.Format("{0:D2}:{1:D2}", timeSpan.Minutes, timeSpan.Seconds);
        timer.text = tempTimer;
        if (currentTime <= 0f)
        {
            if (gameOver)
                return;
            finishGame();
            currentTime = 0;
        }
        else if (currentTime <= hintTime)
        {
            PlayerUIController uIController = (PlayerUIController)GameObject.FindObjectOfType(typeof(PlayerUIController));
            uIController.codePanel.GetComponent<CodePanelController>().isShowHint = true;
        }
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        targetPlayer.CustomProperties[PlayerProperty.SuccessTime] = (int)(gameTime - currentTime);
        if ((bool)targetPlayer.CustomProperties[PlayerProperty.IsPass])
        {
            qualifiedPlayer++;
        }
        if (qualifiedPlayer == quota)
        {
            if (!gameOver)
                finishGame();
        }

    }
    private void finishGame()
    {
        gameOver = true;
        if (!PhotonNetwork.IsMasterClient)
            return;
        PhotonNetwork.LoadLevel(SceneConfig.Conclusion);
    }

    private int qualifiedTeam()
    {
        int _qualifiedPlayer = 0;
        List<string> teamNames = new List<string>();
        foreach (KeyValuePair<int, Player> player in PhotonNetwork.CurrentRoom.Players)
        {
            string teamname = (string)player.Value.CustomProperties[PlayerProperty.Team];
            if (!teamNames.Contains(teamname))
            {
                teamNames.Add(teamname);
                if (!(bool)player.Value.CustomProperties[PlayerProperty.IsEliminate])
                {
                    _qualifiedPlayer++;
                }
            }
        }
        return _qualifiedPlayer;
    }
}
