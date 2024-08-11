using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOnTimer : MonoBehaviour
{
    public float time;
    void Start()
    {
        ActionOnTimer.GetTimer(gameObject, "Destroy Timer").SetTimer(time, () => {
            Destroy(gameObject);
        });
    }

}
