using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class UnitManager : NetworkBehaviour
{
    public Dictionary<ulong, GameObject> playerUnitMap;

    void Awake(){
        playerUnitMap = new();
    }
}