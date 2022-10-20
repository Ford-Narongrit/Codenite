using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ConfirmUIController : MonoBehaviour
{
    public static ConfirmUIController Instance { get; private set; }
    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private Button yesBtn;
    [SerializeField] private Button noBtn;

    private void Awake() {
        Instance = this;
        Hide();
    }

    public void showQuestion(string _title, Action yesAction, Action onAction)
    {
        gameObject.SetActive(true);
        yesBtn.onClick.RemoveAllListeners();
        noBtn.onClick.RemoveAllListeners();

        title.text = _title;
        yesBtn.onClick.AddListener(() =>
        {
            Hide();
            yesAction();
        });

        noBtn.onClick.AddListener(() => 
        {
            Hide();
            onAction();
        });
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
