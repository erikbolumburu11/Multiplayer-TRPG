using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerData : INetworkSerializable {
    public ulong clientId;
    public Team team;
    public string name;

    public PlayerData(ulong clientId, Team team){
        this.clientId = clientId;
        name = $"Player {clientId}";
        this.team = team;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref clientId);
        serializer.SerializeValue(ref name);
        serializer.SerializeValue(ref team);
    }
}