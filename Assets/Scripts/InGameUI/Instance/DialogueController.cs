using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueController : MonoBehaviour
{
    public static DialogueController Instance { get; private set; }

    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private TextMeshProUGUI content;
    [SerializeField] private Button btn;
    private int index;
    private void Awake()
    {
        Instance = this;
        Hide();
    }

    public void showDialogue(string _title, string[] sentences)
    {
        gameObject.SetActive(true);
        index = 0;
        title.text = _title;

        nextSentence(sentences);
    }
    private void nextSentence(string[] sentences)
    {
        if (sentences.Length == index + 1)
        {
            content.text = sentences[index];
            btn.onClick.RemoveAllListeners();
            btn.GetComponentInChildren<TextMeshProUGUI>().text = "End";
            btn.onClick.AddListener(() =>
            {
                Hide();
            });
        }
        else if (sentences.Length > 0)
        {
            content.text = sentences[index];
            btn.onClick.RemoveAllListeners();
            btn.GetComponentInChildren<TextMeshProUGUI>().text = "Next";
            btn.onClick.AddListener(() =>
            {
                index++;
                nextSentence(sentences);
            });
        }
        else
        {
            Hide();
        }
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
