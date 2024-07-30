using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class UnitManager : NetworkBehaviour
{
    public Dictionary<ulong, GameObject> playerUnitMap;

    void Awake(){
        playerUnitMap = new();
    }

    [Rpc(SendTo.Server)]
    public void SpawnUnitRpc(string prefabResourceDir, Vector2Int spawnGridPos, RpcParams rpcParams = default){
        ulong clientId = rpcParams.Receive.SenderClientId;
        UnitManager unitManager = GameManager.Instance.unitManager;
        GridTile tile = GameManager.Instance.gridManager.tiles[spawnGridPos.x, spawnGridPos.y];

        GameObject unit;
        unit = Instantiate(Resources.Load(prefabResourceDir, typeof(GameObject)), tile.worldPosition, Quaternion.identity) as GameObject;
        NetworkObject unitNetworkObject = unit.GetComponent<NetworkObject>();
        unitNetworkObject.Spawn();

        unitManager.playerUnitMap[clientId] = unit;
    }

}