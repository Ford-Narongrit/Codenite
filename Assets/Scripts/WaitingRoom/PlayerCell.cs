using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerCell : MonoBehaviour
{
    [SerializeField] public TextMeshProUGUI playerName;

    public void setPlayerCell(string _player)
    {
        playerName.text = _player;
    }
}
