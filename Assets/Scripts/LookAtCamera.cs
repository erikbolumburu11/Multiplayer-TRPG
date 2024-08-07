using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    public void Update(){
        Vector3 camPos = Camera.main.transform.position;
        Vector3 lookAtPos = new Vector3(transform.position.x, camPos.y, camPos.z);
        transform.LookAt(lookAtPos);
    }
}
