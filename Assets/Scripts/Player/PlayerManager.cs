using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerManager : NetworkBehaviour
{
    public Dictionary<ulong, PlayerData> playerDatas;

    void Update(){
        if(Input.GetKeyDown(KeyCode.B)) 
            Debug.Log("Breakmpoint");
    }

    [Rpc(SendTo.Server)]
    public void RequestPlayerDataRpc(ulong clientId, RpcParams rpcParams = default){
        SendPlayerDataRpc(clientId, playerDatas[clientId], RpcTarget.Single(rpcParams.Receive.SenderClientId, RpcTargetUse.Temp));
    }

    [Rpc(SendTo.SpecifiedInParams)]
    public void SendPlayerDataRpc(ulong clientId, PlayerData playerData, RpcParams rpcParams = default){
        // if(!playerDatas.ContainsKey(clientId)) playerDatas.Add(clientId, playerData);
        // else playerDatas[clientId] = playerData;
    }

    void Awake(){
        playerDatas = new();
    }

}
