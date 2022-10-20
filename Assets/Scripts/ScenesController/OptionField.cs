using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class OptionField : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] public TMP_Dropdown dropdown;
    [SerializeField] public string title;
    [SerializeField] public List<string> keys = new List<string>();
    [SerializeField] public List<int> values = new List<int>();
    public Dictionary<string, int> options = new Dictionary<string, int>();
    public KeyValuePair<string, int> currentValue { get; private set; }

    private void Awake()
    {
        titleText.text = title;
        intiDic();
        setOptions(options);

        dropdown.onValueChanged.AddListener(delegate
        {
            dropdownItemSelected(dropdown);
        });
    }

    public void setOptions(Dictionary<string, int> _options)
    {
        dropdown.options.Clear();
        options = _options;
        foreach (var item in _options)
        {
            dropdown.options.Add(new TMP_Dropdown.OptionData() { text = item.Key });
        }
        dropdownItemSelected(dropdown);
    }

    public void dropdownItemSelected(TMP_Dropdown dropdown)
    {
        if (dropdown.options.Count > 0)
        {
            string dropdown_current_text = dropdown.options[dropdown.value].text;
            dropdown.captionText.text = dropdown_current_text;
            currentValue = new KeyValuePair<string, int>(dropdown_current_text, options[dropdown_current_text]);
        }
    }

    public void intiDic()
    {
        for (int i = 0; i < Mathf.Min(keys.Count, values.Count); i++)
        {
            options.Add(keys[i], values[i]);
        }
    }
}
