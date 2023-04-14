using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tower : MonoBehaviour
{
    public float depth;
    private float _positionY;
    
    public float speedDepth = 0.5f;

    private void Update()
    {
        if (depth > _positionY) return;

        _positionY -= speedDepth * Time.deltaTime;
        transform.position = new Vector3(40,_positionY,40);
    }
}
