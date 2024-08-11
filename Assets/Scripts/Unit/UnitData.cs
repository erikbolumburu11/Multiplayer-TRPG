using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Default Unit Data", menuName = "Unit Data")]
public class UnitData : ScriptableObject
{
    public string unitName;
    public int maxHealth;
    public int moveRange;
    public int basicAttackRange;
    public float speed;
    public string prefabResourceDir;
    public List<Ability> abilities;
}
