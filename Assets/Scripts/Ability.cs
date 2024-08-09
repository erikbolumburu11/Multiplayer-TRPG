using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AbilityTarget {
    ENEMIES,
    ALLIES,
    BOTH
}

public class Ability : ScriptableObject
{
    public int castRange = 5;
    public int effectRange = 1;

    public AbilityTarget target;

    public int damageAmount;
    public int healAmount;
}
