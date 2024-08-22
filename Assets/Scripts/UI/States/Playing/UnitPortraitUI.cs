using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class UnitPortraitUI : NetworkBehaviour
{
    [SerializeField] Color allyBackgroundColor;
    [SerializeField] Color enemyBackgroundColor;

    [Space]

    [SerializeField] Image backgroundUIObject;
    [SerializeField] Image portraitUIObject;

    public void SetPortrait(ulong unitOwnerId, Sprite portrait){

        Team unitTeam = GameManager.Instance.playerManager.playerDatas[unitOwnerId].team;
        Team clientTeam = GameManager.Instance.playerManager.playerDatas[NetworkManager.Singleton.LocalClientId].team;

        if(unitTeam == clientTeam) backgroundUIObject.color = allyBackgroundColor;
        else backgroundUIObject.color = enemyBackgroundColor;

        portraitUIObject.sprite = portrait;
    }
}
