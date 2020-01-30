using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mover : MonoBehaviour
{
    public static float Speed { set; get; }

    void Awake()
    {
        Speed = 10f;
    }

    void Update()
    {
        transform.position += -Vector3.right * Time.deltaTime * Speed;
    }
}
