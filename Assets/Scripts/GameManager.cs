using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
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

    void Awake(){
        instance = this;
    }
}
