using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraMove : MonoBehaviour
{
    private float _positionY = 9f;
    private float _positionZ = 31.5f;
    private float _rotateX = 41.5f;
    private Transform _cameraTransform;

    private void Awake()
    {
        _cameraTransform = transform;
    }

    public void BiasCamera()
    {
        _positionY += 2.2f;
        _positionZ -= 2.2f;
        _rotateX += 0.15f;
        
        _cameraTransform.localPosition = new Vector3(40,_positionY,_positionZ);
        _cameraTransform.rotation = Quaternion.Euler(_rotateX,0f,0f);
    }

    public void TowerDepth()
    {
       var camPos = _cameraTransform.position;
       _positionY -= 0.05f;
       _cameraTransform.position = new Vector3(camPos.x,camPos.y-0.05f,camPos.z);
    }
}
