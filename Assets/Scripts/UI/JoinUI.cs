using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class JoinUI : MonoBehaviour
{
    [SerializeField] GameObject joinSettingsObject;
    [SerializeField] NetworkManager networkManager;

    public void HostJoin(){
        networkManager.StartHost();
        joinSettingsObject.SetActive(false);
    }

    public void ClientJoin(){
        networkManager.StartClient();
        joinSettingsObject.SetActive(false);
    }
}
