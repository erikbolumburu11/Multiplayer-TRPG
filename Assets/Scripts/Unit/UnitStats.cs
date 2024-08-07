using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class UnitStats : NetworkBehaviour
{
    public NetworkVariable<int> health;
}
