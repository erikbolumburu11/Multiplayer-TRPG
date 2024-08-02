using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class TurnManager : NetworkBehaviour
{
    public NetworkVariable<ulong> currentPlayerClientId = new();
    public NetworkVariable<int> currentTurn = new();

    void Awake(){
        currentPlayerClientId.Value = 0;
        currentTurn.Value = 1;
        if(!IsServer) return;
    }

    void Update(){
        SetSelectedUnitRpc();
    }

    [Rpc(SendTo.Server)]
    public void NextTurnRpc(){
        currentPlayerClientId.Value++;
        currentTurn.Value++;
        int connectedPlayerCount = NetworkManager.Singleton.ConnectedClientsList.Count;
        if((int)currentPlayerClientId.Value >= connectedPlayerCount) currentPlayerClientId.Value = 0;
    }

    [Rpc(SendTo.Everyone)]
    public void SetSelectedUnitRpc(RpcParams rpcParams = default){
        SetSelectedUnit();
    }

    public void SetSelectedUnit(){
        UnitManager unitManager = GameManager.Instance.unitManager;
        ulong currentPlayerClientId = GameManager.Instance.turnManager.currentPlayerClientId.Value;
        int currentTurn = GameManager.Instance.turnManager.currentTurn.Value;

        if(unitManager.playerUnitMap[currentPlayerClientId].Count != 0) 
            unitManager.selectedUnit = unitManager.playerUnitMap[currentPlayerClientId]
                [currentTurn % unitManager.playerUnitMap[currentPlayerClientId].Count];
    }

    public static bool IsMyTurn(){
        if(GameManager.Instance.turnManager.currentPlayerClientId.Value == NetworkManager.Singleton.LocalClientId)
            return true;
        else
            return false;
    }

    public static UnitBehaviour GetCurrentTurnsUnit(){
        return GameManager.Instance.unitManager.selectedUnit.GetComponent<UnitBehaviour>();
    }
}
