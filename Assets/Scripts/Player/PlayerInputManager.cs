using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class PlayerInputManager : NetworkBehaviour
{
    GridManager gridManager;
    [SerializeField] GameObject unitPrefab;

    void Awake(){
        gridManager = GameManager.Instance.gridManager;
    }

    public override void OnNetworkSpawn(){
        base.OnNetworkSpawn();
        InitializePlayerRpc();
        if(!IsOwner) return;
    }


    void Update(){
        if(!IsOwner) return;
        if(TurnManager.IsMyTurn()) if(Input.GetKeyDown(KeyCode.N)) GameManager.Instance.turnManager.NextTurnRpc();
        if(Input.GetKeyDown(KeyCode.B)) ChangeStateRpc();


        if(GetHoveredTile() != null){
            if(Input.GetKeyDown(KeyCode.M)){
                if(TurnManager.IsMyTurn()) MoveUnitRpc(GetHoveredTile().gridPosition);
            } 
        }
    }

    [Rpc(SendTo.Everyone)]
    public void ChangeStateRpc(){
        GameManager.Instance.gameStateManager.RequestStateChangeRpc(GameStateKey.PLAYING);
    }

    [Rpc(SendTo.NotMe)]
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

    public static GridTile GetHoveredTile(){
        GridManager gridManager = GameManager.Instance.gridManager;
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, GameManager.Instance.groundLayer)) {
            Vector2Int hoveredTilePos = new Vector2Int((int)Mathf.Floor(hit.point.x) / (int)gridManager.tileSize, (int)Mathf.Floor(hit.point.z) / (int)gridManager.tileSize);
            return gridManager.tiles[hoveredTilePos.x, hoveredTilePos.y];
        }
        else return null;
    }
}
