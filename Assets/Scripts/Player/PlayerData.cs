using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerData : INetworkSerializable {
    public ulong clientId;
    public string name;

    public PlayerData(ulong clientId){
        name = $"Player {clientId}";
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref clientId);
        serializer.SerializeValue(ref name);
    }
}