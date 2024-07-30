using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class TurnManager : NetworkBehaviour
{
    public NetworkVariable<ulong> currentPlayerClientId = new();

    void Awake(){
        currentPlayerClientId.Value = 0;
        if(!IsServer) return;
    }

    [Rpc(SendTo.Server)]
    public void NextTurnRpc(){
        currentPlayerClientId.Value++;
        int connectedPlayerCount = NetworkManager.Singleton.ConnectedClientsList.Count;
        if((int)currentPlayerClientId.Value >= connectedPlayerCount) currentPlayerClientId.Value = 0;
    }

    static public bool IsMyTurn(){
        if(GameManager.Instance.turnManager.currentPlayerClientId.Value == NetworkManager.Singleton.LocalClientId)
            return true;
        else
            return false;
    }
}
