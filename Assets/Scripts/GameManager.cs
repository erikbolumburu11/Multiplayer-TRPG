using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
  private static GameManager instance;
    public static GameManager Instance {
        get {
            if(instance == null) Debug.Log("GameManager is null");
            return instance;
        }
    }

    public GridManager gridManager;
    public UnitManager unitManager;
    public Pathfinder pathfinder;
    public GameStateManager gameStateManager;
    public TurnManager turnManager;
    public PlayerManager playerManager;
    public PlayerPartyManager playerPartyManager;
    public UIElements UIElements;
    public LayerMask groundLayer;

    void Awake(){
        instance = this;
    }
}
