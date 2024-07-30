using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

public class PlayerPartyManager : MonoBehaviour
{
    public List<UnitData> units;

    public void Awake(){
        units = new();
    }
}
