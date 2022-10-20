using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Zone : MonoBehaviour
{
    [SerializeField] SpanwMonster[] spanwMonsters;
    [SerializeField] TextMeshProUGUI ZoneName;

    public void initZone(string itemname)
    {
        ZoneName.text = itemname;
        foreach (SpanwMonster monster in spanwMonsters)
        {
            monster.iteminZone = itemname;
        }
    }
}
