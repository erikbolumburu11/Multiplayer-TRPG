using System;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class UnitManager : NetworkBehaviour
{
    public Dictionary<ulong, List<GameObject>> playerUnitMap;
    public GameObject selectedUnit;

    void Awake(){
        playerUnitMap = new();
    }

    [Rpc(SendTo.Server)]
    public void SpawnUnitRpc(string prefabResourceDir, Vector2Int spawnGridPos, RpcParams rpcParams = default){
        ulong clientId = rpcParams.Receive.SenderClientId;
        GridTile tile = GameManager.Instance.gridManager.tiles[spawnGridPos.x, spawnGridPos.y];

        GameObject unit;
        GameObject unitPrefab = Resources.Load(prefabResourceDir, typeof(GameObject)) as GameObject;
        unit = Instantiate(unitPrefab, tile.worldPosition, Quaternion.identity);

        NetworkObject unitNetworkObject = unit.GetComponent<NetworkObject>();
        unitNetworkObject.Spawn();

        unit.GetComponent<UnitBehaviour>().ownerClientId.Value = clientId;
        unit.GetComponent<UnitBehaviour>().occupyingTile.Value = tile.gridPosition;

        AddUnitToPlayerUnitMapRpc(unit, clientId);

        GameManager.Instance.turnManager.NextTurnRpc();
    }

    [Rpc(SendTo.Everyone)]
    public void AddUnitToPlayerUnitMapRpc(NetworkObjectReference networkObjectReference, ulong clientId, RpcParams rpcParams = default){
        GameManager.Instance.unitManager.playerUnitMap[clientId].Add(networkObjectReference);
    }

    [Rpc(SendTo.Server)]
    public void MoveSelectedUnitRpc(Vector2Int targetTilePos, RpcParams rpcParams = default) {
        UnitBehaviour unitBehaviour = TurnManager.GetCurrentTurnsUnit().GetComponent<UnitBehaviour>();
        GridManager gridManager = GameManager.Instance.gridManager;

        Vector2Int startTilePos = unitBehaviour.occupyingTile.Value;
        GridTile startTile = gridManager.tiles[startTilePos.x, startTilePos.y];

        Stack<GridTile> path = GameManager.Instance.pathfinder.FindPath(startTile, GameManager.Instance.gridManager.tiles[targetTilePos.x, targetTilePos.y]);
        unitBehaviour.path = path;
        Turn turn = GameManager.Instance.turnManager.turn;
        if(path != null) GameManager.Instance.turnManager.SetTurnRpc(new Turn(){
            hasMoved = true,
            hasAttacked = turn.hasAttacked
        });
    }

    [Rpc(SendTo.Server)]
    public void BasicAttackRpc(Vector2Int targetTilePos, RpcParams rpcParams = default)
    {
        if(GridManager.GetTilesOccupyingObject(targetTilePos) == null) return;
        if(GameManager.Instance.turnManager.turn.hasAttacked) return;

        GameObject objOnTile = GridManager.GetTilesOccupyingObject(targetTilePos);

        if(objOnTile.TryGetComponent(out UnitBehaviour unitBehaviour)){
            GameObject selectedUnit = GameManager.Instance.unitManager.selectedUnit;

            // Check if friendly fire
            if(selectedUnit.GetComponent<UnitBehaviour>().ownerClientId.Value == unitBehaviour.ownerClientId.Value) return;

            unitBehaviour.isAttacking.Value = true;

            selectedUnit.GetComponent<ClientAuthNetworkAnimator>().SetTrigger("BasicAttack");
            selectedUnit.transform.LookAt(objOnTile.transform);
            unitBehaviour.unitStats.health.Value -= 100; // Damage

            Turn turn = GameManager.Instance.turnManager.turn;
            GameManager.Instance.turnManager.SetTurnRpc(new Turn(){
                hasMoved = turn.hasMoved,
                hasAttacked = true
            });

            ActionOnTimer.GetTimer(gameObject, "BasicAttack_Timer").SetTimer(1, () => {
                unitBehaviour.isAttacking.Value = false;
            });
            
        }
    }

}