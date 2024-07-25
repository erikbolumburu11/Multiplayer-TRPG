using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class UnitManager : NetworkBehaviour
{
    public Dictionary<int, PlayerData> playerDatas;

    void Start(){
        playerDatas = new();
    }
}