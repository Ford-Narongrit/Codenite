using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SpanwMonster : MonoBehaviour
{
    [SerializeField] MonsterController monsterPrefab;
    GameObject monster = null;
    public string iteminZone { get; set; }

    private void LateUpdate()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if (monster == null && iteminZone != null)
            {
                monster = PhotonNetwork.InstantiateRoomObject(monsterPrefab.name, transform.position, Quaternion.identity);
                monster.GetComponent<PhotonView>().RPC("setItem", RpcTarget.All, iteminZone);
                return;
            }
        }
    }
}
