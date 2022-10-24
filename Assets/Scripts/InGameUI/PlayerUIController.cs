using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class PlayerUIController : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GaugeBar smileBar;
    [SerializeField] public GameObject worldMap;
    [SerializeField] public GameObject menu;
    [SerializeField] public GameObject codePanel;
    public CinemachineVirtualCamera cv;
    public int spectateIndex = 0;
    public bool IsSpectator = false;
    public PlayerController player { get; set; }
    private void Awake()
    {
        smileBar.gameObject.SetActive(!ENVConfig.useSmile);
        worldMap.SetActive(false);
        menu.SetActive(false);
        codePanel.SetActive(false);
        cv = GameObject.FindGameObjectWithTag("PlayerFollowCamera").GetComponent<CinemachineVirtualCamera>();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.M) || Input.GetKeyDown(KeyCode.Tab))
        {
            worldMap.SetActive(true);
        }
        if (Input.GetKeyUp(KeyCode.M) || Input.GetKeyUp(KeyCode.Tab))
        {
            worldMap.SetActive(false);
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            codePanel.SetActive(!codePanel.activeSelf);
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            menu.SetActive(!menu.activeSelf);
        }

        if (Input.GetKeyDown(KeyCode.E) && IsSpectator)
        {
            spectateNextPlayer();
        }

        if (Input.GetKeyDown(KeyCode.Q) && IsSpectator)
        {
            spectatePrevPlayer();
        }

        if (player != null)
        {
            if (worldMap.activeSelf || menu.activeSelf || codePanel.activeSelf)
            {
                player.interact = false;
            }
            else
            {
                player.interact = true;
            }

            smileBar.SetMaxValue(player.chargeToFireTime);
            smileBar.SetValue(player.currentChargeTime);
        }
    }

    public void spectateNextPlayer()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        spectateIndex++;
        if (spectateIndex > (players.Length - 1))
        {
            spectateIndex = 0;
        }
        cv.Follow = players[spectateIndex].transform ?? null;
        codePanel.GetComponent<CodePanelController>().player = players[spectateIndex].GetComponent<PlayerController>();
    }

    public void spectatePrevPlayer()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        spectateIndex--;
        if (spectateIndex < 0)
        {
            spectateIndex = players.Length - 1;
        }
        cv.Follow = players[spectateIndex].transform;
        codePanel.GetComponent<CodePanelController>().player = players[spectateIndex].GetComponent<PlayerController>();
    }

    public void toggleCodePanel(bool toggle)
    {
        codePanel.SetActive(toggle);
    }
}
