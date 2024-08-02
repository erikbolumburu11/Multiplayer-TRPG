using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public abstract class GameState : NetworkBehaviour
{
    public string stateName;
    public GameStateKey key;
    public GameStateUI stateUI;

    protected abstract void Initialize();

    public virtual void EnterState(){
        Initialize();
        stateUI.EnterState();
    }

    public virtual void Update(){
        if(NetworkManager.Singleton.IsConnectedClient) stateUI.Update();
    }

    public virtual void ExitState(){
        stateUI.ExitState();
    }
}
