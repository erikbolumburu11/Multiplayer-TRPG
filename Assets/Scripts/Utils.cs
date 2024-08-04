using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utils : MonoBehaviour
{
    public static Vector2 LerpVec2(Vector2 a, Vector2 b, float t){
        return new Vector2( Mathf.Lerp(a.x, b.x, t),
                            Mathf.Lerp(a.y, b.y, t));
    }

    public static Vector2Int FloorVec2(Vector2 vec){
        return new Vector2Int(Mathf.FloorToInt(vec.x), Mathf.FloorToInt(vec.y));
    }
}
