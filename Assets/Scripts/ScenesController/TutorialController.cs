using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Photon.Pun;
using Photon.Realtime;

public class TutorialController : MonoBehaviour
{
    [Header("Player spawner")]
    [SerializeField] private GameObject playerPrefabs;
    [SerializeField] private Transform playerSpawnpoint;

    [Header("Monster spawner")]
    [SerializeField] private GameObject MonsterPreFab;
    [SerializeField] private Transform monsterSpawnPoint;

    private void Start()
    {
        initPlayer();

        GameObject monster = PhotonNetwork.Instantiate(MonsterPreFab.name, monsterSpawnPoint.position, monsterSpawnPoint.rotation);
        monster.GetComponent<MonsterController>().carryItem = "Hello world";

        string[] dialogs = { "Use [W] [A] [S] [D] to move", "Use mouse1 to fire arrow.", "Use [C] to open code panel", "Goodluck have fun." };
        DialogueController.Instance.showDialogue("HOW to play", dialogs);
    }

    public void initPlayer()
    {
        //create player
        GameObject player = PhotonNetwork.Instantiate(playerPrefabs.name, playerSpawnpoint.position, playerSpawnpoint.rotation);

        //set player camera
        CinemachineVirtualCamera cv = GameObject.FindGameObjectWithTag("PlayerFollowCamera").GetComponent<CinemachineVirtualCamera>();
        cv.Follow = player.transform;

        //set player ui
        PlayerUIController uIController = (PlayerUIController)GameObject.FindObjectOfType(typeof(PlayerUIController));
        uIController.player = player.GetComponent<PlayerController>();

        //set codepanel
        CodePanelController codePanel = uIController.codePanel.GetComponent<CodePanelController>();
        codePanel.player = player.GetComponent<PlayerController>();

        List<string> answer = new List<string>();
        answer.Add("Hello world");
        codePanel.setProblemInfo(
            "int main()\n{\n\tprint(<color=red>[1]</color>);\n}",
            "Hello world",
            answer
        );
    }
}
