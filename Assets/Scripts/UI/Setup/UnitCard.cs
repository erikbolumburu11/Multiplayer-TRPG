using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UnitCard : MonoBehaviour
{
    public UnitData unitData;
    public TMP_Text unitNameObject;

    void Update(){
        if(unitData == null) return;
        unitNameObject.text = unitData.unitName;
    }
}
