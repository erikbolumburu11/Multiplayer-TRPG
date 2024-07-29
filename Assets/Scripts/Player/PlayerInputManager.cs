using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class PlayerInputManager : NetworkBehaviour
{
    GridManager gridManager;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] GameObject unitPrefab;

    void Awake(){
        gridManager = GameManager.Instance.gridManager;
    }

    public override void OnNetworkSpawn(){
        base.OnNetworkSpawn();
        InitializePlayerRpc();
        if(!IsOwner) return;
        SpawnUnitRpc();
    }


    void Update(){
        if(!IsOwner) return;
        if(IsServer) if(Input.GetKeyDown(KeyCode.N)) GameManager.Instance.turnManager.NextTurn();


        if(GetHoveredTile() != null){
            if(Input.GetKeyDown(KeyCode.M)) MoveUnitRpc(GetHoveredTile().gridPosition);
        }
    }

    [Rpc(SendTo.Everyone)]
    public void InitializePlayerRpc(RpcParams rpcParams = default)
    {
        ulong clientId = rpcParams.Receive.SenderClientId;
        GameManager.Instance.playerManager.playerDatas.Add(clientId, new(clientId));
    }

    [Rpc(SendTo.Server)]
    void MoveUnitRpc(Vector2Int targetTilePos, RpcParams rpcParams = default) {
        UnitManager unitManager = GameManager.Instance.unitManager;
        GameObject unit = unitManager.playerUnitMap[rpcParams.Receive.SenderClientId];
        UnitBehaviour unitBehaviour = unit.GetComponent<UnitBehaviour>();
        unitBehaviour.path = GameManager.Instance.pathfinder.FindPath(unitBehaviour.occupyingTile, gridManager.tiles[targetTilePos.x, targetTilePos.y]);
    }

    [Rpc(SendTo.Server)]
    void SpawnUnitRpc(RpcParams rpcParams = default){
        ulong clientId = rpcParams.Receive.SenderClientId;
        UnitManager unitManager = GameManager.Instance.unitManager;

        GameObject unit = Instantiate(unitPrefab);
        NetworkObject unitNetworkObject = unit.GetComponent<NetworkObject>();
        unitNetworkObject.Spawn();

        unitManager.playerUnitMap[clientId] = unit;
    }

    public GridTile GetHoveredTile(){
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, groundLayer)) {
            Vector2Int hoveredTilePos = new Vector2Int((int)Mathf.Floor(hit.point.x), (int)Mathf.Floor(hit.point.z));
            return gridManager.tiles[hoveredTilePos.x, hoveredTilePos.y];
        }
        else return null;
    }
}
