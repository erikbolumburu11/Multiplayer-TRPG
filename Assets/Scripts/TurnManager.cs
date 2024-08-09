using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public struct Turn : INetworkSerializable {
    public bool hasMoved;
    public bool hasAttacked;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref hasMoved);
        serializer.SerializeValue(ref hasAttacked);
    }
}

public class TurnManager : NetworkBehaviour
{
    public NetworkVariable<ulong> currentPlayerClientId = new();
    public NetworkVariable<int> currentTurn = new();
    public Turn turn;
    Dictionary<ulong, int> playersCurrentUnitMap;

    void Awake(){
        playersCurrentUnitMap = new();
        currentPlayerClientId.Value = 0;
        currentTurn.Value = 1;
        if(!IsServer) return;
        turn = new Turn();
    }

    public override void OnNetworkSpawn(){
        currentTurn.OnValueChanged += SetSelectedUnit;
    }

    public override void OnNetworkDespawn(){
        currentTurn.OnValueChanged -= SetSelectedUnit;
    }


    [Rpc(SendTo.Server)]
    public void NextTurnRpc(){
        int connectedPlayerCount = NetworkManager.Singleton.ConnectedClientsList.Count;
        if((int)currentPlayerClientId.Value + 1 != connectedPlayerCount) currentPlayerClientId.Value++;
        else currentPlayerClientId.Value = 0;

        currentTurn.Value++;

        SetTurnRpc(new Turn());
    }

    [Rpc(SendTo.Everyone)]
    public void SetSelectedUnitRpc(RpcParams rpcParams = default){
    }

    [Rpc(SendTo.Everyone)]
    public void SetTurnRpc(Turn turn, RpcParams rpcParams = default){
        this.turn = turn;
    }

    public void SetSelectedUnit(int previous, int current){
        UnitManager unitManager = GameManager.Instance.unitManager;
        ulong currentPlayerClientId = GameManager.Instance.turnManager.currentPlayerClientId.Value;
        int currentTurn = GameManager.Instance.turnManager.currentTurn.Value;

        if(!playersCurrentUnitMap.ContainsKey(currentPlayerClientId)){
            playersCurrentUnitMap.Add(currentPlayerClientId, -1);
        }

        playersCurrentUnitMap[currentPlayerClientId]++;
        if(playersCurrentUnitMap[currentPlayerClientId] == unitManager.playerUnitMap[currentPlayerClientId].Count) playersCurrentUnitMap[currentPlayerClientId] = 0;
        unitManager.selectedUnit = unitManager.playerUnitMap[currentPlayerClientId][playersCurrentUnitMap[currentPlayerClientId]];

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
