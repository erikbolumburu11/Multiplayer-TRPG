using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AbilityTarget {
    ENEMIES,
    ALLIES,
    BOTH
}

[CreateAssetMenu(fileName = "Default Ability", menuName = "Ability")]
public class Ability : ScriptableObject
{
    public string path;
    public bool requiresTargetUnit;
    public bool canTargetCaster;
    public bool canTargetCasterTile;
    public int castRange = 5;
    public int effectRange = 1;

    public AbilityTarget target;
    public GameObject particlePrefab;

    public int damageAmount;
    public int healAmount;
}
