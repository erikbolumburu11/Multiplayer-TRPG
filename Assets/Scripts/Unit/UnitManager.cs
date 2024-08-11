using System;
using System.Collections.Generic;
using Unity.Mathematics;
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
            hasPerformedAction = turn.hasPerformedAction
        });
    }

    [Rpc(SendTo.Server)]
    public void BasicAttackRpc(Vector2Int targetTilePos, RpcParams rpcParams = default)
    {
        if(GridManager.GetTilesOccupyingObject(targetTilePos) == null) return;
        if(GameManager.Instance.turnManager.turn.hasPerformedAction) return;

        GameObject objOnTile = GridManager.GetTilesOccupyingObject(targetTilePos);

        if(objOnTile.TryGetComponent(out UnitBehaviour unitBehaviour)){
            GameObject selectedUnit = GameManager.Instance.unitManager.selectedUnit;

            // Check if friendly fire
            if(selectedUnit.GetComponent<UnitBehaviour>().ownerClientId.Value == unitBehaviour.ownerClientId.Value) return;

            unitBehaviour.isPerformingAction.Value = true;

            selectedUnit.GetComponent<ClientAuthNetworkAnimator>().SetTrigger("BasicAttack");
            selectedUnit.transform.LookAt(objOnTile.transform);
            unitBehaviour.unitStats.health.Value -= 100; // Damage

            Turn turn = GameManager.Instance.turnManager.turn;
            GameManager.Instance.turnManager.SetTurnRpc(new Turn(){
                hasMoved = turn.hasMoved,
                hasPerformedAction = true
            });

            ActionOnTimer.GetTimer(gameObject, "BasicAttack_Timer").SetTimer(1, () => {
                unitBehaviour.isPerformingAction.Value = false;
            });
            
        }
    }

    [Rpc(SendTo.Server)]
    public void CastAbilityRpc(Vector2Int targetTilePos, string abilityPath, RpcParams rpcParams = default){
        Ability ability = Resources.Load(abilityPath, typeof(Ability)) as Ability;

        if(ability.requiresTargetUnit && GridManager.GetTilesOccupyingObject(targetTilePos) == null) return;
        if((!ability.canTargetCaster || !ability.canTargetCasterTile) && GridManager.GetTilesOccupyingObject(targetTilePos) == GetSelectedUnit()) return;

        if(GameManager.Instance.turnManager.turn.hasPerformedAction) return;

        List<GridTile> affectedTiles = Visibility.GetVisibleTilesFromList(
            GridManager.GetTileAtVector2Int(targetTilePos),
            RangeFinder.GetTilesInRangeFromTile(
                GridManager.GetTileAtVector2Int(targetTilePos),
                ability.effectRange,
                true
            )
        );

        GetSelectedUnitBehaviour().isPerformingAction.Value = true;

        selectedUnit.GetComponent<ClientAuthNetworkAnimator>().SetTrigger("AbilityRaise");
        selectedUnit.transform.LookAt(GridManager.GetTileAtVector2Int(targetTilePos).worldPosition);

        ActionOnTimer.GetTimer(gameObject, "CastingAbility_Timer").SetTimer(1, () => {
            foreach (GridTile tile in affectedTiles)
            {
                GameObject objOnTile = GridManager.GetTilesOccupyingObject(tile.gridPosition);
                if(objOnTile == null) continue;

                if(objOnTile.TryGetComponent(out UnitBehaviour unitBehaviour)){
                    bool targettingAlly = GetSelectedUnitBehaviour().ownerClientId.Value == unitBehaviour.ownerClientId.Value;
                    if(targettingAlly && ability.target == AbilityTarget.ENEMIES) continue;

                    unitBehaviour.unitStats.health.Value -= ability.damageAmount; // Damage
                    unitBehaviour.unitStats.health.Value += ability.healAmount; // Heal
                }

            }

            Instantiate(ability.particlePrefab, GridManager.GetTileAtVector2Int(targetTilePos).worldPosition, Quaternion.identity);

            Turn turn = GameManager.Instance.turnManager.turn;
            GameManager.Instance.turnManager.SetTurnRpc(new Turn(){
                hasMoved = turn.hasMoved,
                hasPerformedAction = true
            });

            GetSelectedUnitBehaviour().isPerformingAction.Value = false;
        });
    }

    public static GameObject GetSelectedUnit(){
        return GameManager.Instance.unitManager.selectedUnit;
    }

    public static UnitBehaviour GetSelectedUnitBehaviour(){
        return GameManager.Instance.unitManager.selectedUnit.GetComponent<UnitBehaviour>();
    }

}