using UnityEngine;
using TMPro;

public class HelperBox : MonoBehaviour
{
    public static HelperBox Instance { get; private set; }
    [SerializeField] public TextMeshProUGUI text;

    private void Awake()
    {
        Instance = this;
        hide();
    }

    public void showAlert(string _text)
    {
        gameObject.SetActive(true);
        text.text = _text;
    }

    public void hide()
    {
        gameObject.SetActive(false);
    }
}
