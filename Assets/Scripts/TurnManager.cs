using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public struct Turn : INetworkSerializable {
    public bool hasMoved;
    public bool hasPerformedAction;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref hasMoved);
        serializer.SerializeValue(ref hasPerformedAction);
    }
}

public class TurnManager : NetworkBehaviour
{
    public NetworkVariable<ulong> currentPlayerClientId = new();
    public NetworkVariable<int> currentTurn = new();
    public Turn turn;
    public Dictionary<ulong, int> playersCurrentUnitMap;

    void Awake(){
        playersCurrentUnitMap = new();
        currentPlayerClientId.Value = 0;
        currentTurn.Value = 1;
        if(!IsServer) return;
        turn = new Turn();
    }


    [Rpc(SendTo.Server)]
    public void NextTurnRpc(){
        int connectedPlayerCount = NetworkManager.Singleton.ConnectedClientsList.Count;
        if((int)currentPlayerClientId.Value + 1 != connectedPlayerCount) currentPlayerClientId.Value++;
        else currentPlayerClientId.Value = 0;

        currentTurn.Value++;

        SetTurnRpc(new Turn());
        SetSelectedUnit();
    }

    [Rpc(SendTo.Everyone)]
    public void SetSelectedUnitRpc(RpcParams rpcParams = default){
    }

    [Rpc(SendTo.Everyone)]
    public void SetTurnRpc(Turn turn, RpcParams rpcParams = default){
        this.turn = turn;
    }

    public void SetSelectedUnit(){
        if(!GameStateManager.CompareCurrentState(GameStateKey.PLAYING)) return;

        UnitManager unitManager = GameManager.Instance.unitManager;
        ulong currentPlayerClientId = GameManager.Instance.turnManager.currentPlayerClientId.Value;
        int currentTurn = GameManager.Instance.turnManager.currentTurn.Value;

        if(playersCurrentUnitMap[currentPlayerClientId] >= unitManager.playerUnitMap[currentPlayerClientId].Count) playersCurrentUnitMap[currentPlayerClientId] = 0;
        unitManager.selectedUnit.Value = unitManager.playerUnitMap[currentPlayerClientId][playersCurrentUnitMap[currentPlayerClientId]];

        playersCurrentUnitMap[currentPlayerClientId]++;
    }

    public static bool IsMyTurn(){
        if(GameManager.Instance.turnManager.currentPlayerClientId.Value == NetworkManager.Singleton.LocalClientId)
            return true;
        else
            return false;
    }
}
