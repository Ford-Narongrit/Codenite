using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Cinemachine;
using Photon.Pun;
using TMPro;

public class CodePanelController : MonoBehaviour
{
    [Header("ItemList info")]
    [SerializeField] public int maxInventory = 3;
    [SerializeField] public int answerSlot = 3;
    [Header("Text UI")]
    [SerializeField] private TextMeshProUGUI problem;
    [SerializeField] private TextMeshProUGUI result;
    [Header("ItemList UI")]
    [SerializeField] private GameObject answerSlotPrefab;
    [SerializeField] private GameObject itemSlotPrefab;
    [SerializeField] private GameObject itemPrefab;
    [SerializeField] private Transform inventoryTransform;
    [SerializeField] private Transform answerSlotTransform;

    private List<GameObject> inventorySlotList = new List<GameObject>();
    private List<AnswerSlot> answerList = new List<AnswerSlot>();
    private List<ItemController> currentItems = new List<ItemController>();
    private List<string> correctAnswers = new List<string>();
    public PlayerController player { get; set; }
    public bool isShowHint = false;
    private void OnEnable()
    {
        if (player != null)
        {
            List<string> items = player.itemList;
            foreach (string item in items)
            {
                createItemPrefab(item);
            }
            if (isShowHint)
            {
                showHint();
            }
        }
    }
    private void OnDisable()
    {
        clearItemPrefab();
        for (int i = 0; i < correctAnswers.Count; i++)
        {
            answerList[i].setIndexText(i + 1);
        }
    }
    private void LateUpdate()
    {
        udpateSlot();
    }

    public void setProblemInfo(string problem, string result, List<string> answers)
    {
        this.problem.text = HtmlToText.HTMLToText(problem);
        this.result.text = HtmlToText.HTMLToText(result);
        this.answerSlot = answers.Count;
        this.correctAnswers = answers;

        for (int i = 0; i < maxInventory; i++)
        {
            GameObject newSlot = Instantiate(itemSlotPrefab, inventoryTransform);
            inventorySlotList.Add(newSlot);
        }

        for (int i = 0; i < answerSlot; i++)
        {
            GameObject newSlot = Instantiate(answerSlotPrefab, answerSlotTransform);
            newSlot.GetComponentInChildren<AnswerSlot>().setIndexText(i + 1);
            answerList.Add(newSlot.GetComponentInChildren<AnswerSlot>());
        }
    }

    private void udpateSlot()
    {
        for (int i = 0; i < maxInventory; i++)
        {
            if (inventorySlotList[i].GetComponentInChildren<ItemController>() != null)
            {
                inventorySlotList[i].GetComponent<Image>().color = Color.red;
            }
            else
            {
                inventorySlotList[i].GetComponent<Image>().color = Color.white;
            }
        }
    }
    public void createItemPrefab(string item)
    {
        GameObject newItemObject = Instantiate(itemPrefab, inventorySlotList[getEmpty()].transform);
        newItemObject.GetComponent<ItemController>().setItemName(item);

        currentItems.Add(newItemObject.GetComponent<ItemController>());
        newItemObject.GetComponentInChildren<Button>().onClick.AddListener(delegate { OnClickDelete(newItemObject); });
    }
    public void clearItemPrefab()
    {
        foreach (ItemController item in currentItems)
        {
            Destroy(item.gameObject);
        }
        currentItems = new List<ItemController>();
    }
    private int getEmpty()
    {
        for (int i = 0; i < maxInventory; i++)
        {
            if (inventorySlotList[i].GetComponentInChildren<ItemController>() == null)
            {
                return i;
            }
        }
        return 0;
    }
    public void showHint()
    {
        for (int i = 0; i < correctAnswers.Count; i++)
        {
            answerList[i].showAnswer(correctAnswers[i]);
        }
    }
    public void resetItemToSlot()
    {
        foreach (ItemController item in currentItems)
        {
            item.resetPosition();
        }
    }

    private bool checkAnswer()
    {
        for (int i = 0; i < correctAnswers.Count; i++)
        {
            if (answerList[i].getItem() == null)
                return false;
            if (correctAnswers[i] != answerList[i].getItem().itemName)
            {
                return false;
            }
        }
        return true;
    }
    public void OnClickSubmit()
    {
        if (checkAnswer())
        {
            if (SceneManager.GetActiveScene().name == SceneConfig.Tutorial)
            {
                AlertController.Instance.showAlert("CONGRATULATIONS!\nMISSION COMPLETE", "Now you ready to play this game.", "Done", () =>
                {
                    // do nothing
                });
                return;
            }

            PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable() { { PlayerProperty.IsPass, true } });
            AlertController.Instance.showAlert("CONGRATULATIONS!\nMISSION COMPLETE", "Please wait of other players finish the mission.", "Spectating", () =>
                {
                    player.gameObject.GetPhotonView().RPC("destroy", RpcTarget.All);
                    ((PlayerUIController)GameObject.FindObjectOfType(typeof(PlayerUIController))).IsSpectator = true;
                });
        }
        else
        {
            PhotonNetwork.LocalPlayer.CustomProperties[PlayerProperty.Fom] = (int)PhotonNetwork.LocalPlayer.CustomProperties[PlayerProperty.Fom] + 1;
            AlertController.Instance.showAlert("Error", "Oops! the answer is not correct. Please click 'Reset' button and try again.", "Reset", () =>
                {
                    resetItemToSlot();
                });
        }

    }
    public void OnClickDelete(GameObject item)
    {
        ConfirmUIController.Instance.showQuestion("Do you want to delete this item ?" + item.GetComponent<ItemController>().itemName,
            () =>
            {
                if (player.GetComponent<PhotonView>().IsMine)
                {
                    currentItems.Remove(item.GetComponent<ItemController>());
                    player.GetComponent<PhotonView>().RPC("removeItem", RpcTarget.All, item.GetComponent<ItemController>().itemName);
                    Destroy(item);
                }
            },
            () =>
            {
                // Do nothing
            }
        );
    }
}
