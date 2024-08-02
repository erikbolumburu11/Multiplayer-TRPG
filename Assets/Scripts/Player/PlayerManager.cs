using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerManager : NetworkBehaviour
{
    public Dictionary<ulong, PlayerData> playerDatas;

    void Awake(){
        playerDatas = new();
    }

}
