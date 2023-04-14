using System;
using System.Collections;
using System.Collections.Generic;
using Dreamteck.Splines;
using Unity.VisualScripting;
using UnityEngine;

public class raiseCollector : MonoBehaviour
{
    public int countOpenChild;
    public int numberCollector;

    private Transform _resourcePiece;
    
    private void Start()
    {
        _resourcePiece = transform.GetChild(1);
        ResetCountResource();
    }

    private void FixedUpdate()
    {
        var positionCollectorSpline = transform.gameObject.GetComponent<SplineProjector>().result.percent;
        
        var absPositionCollectorSpline = Mathf.Abs(0.5f - (float)positionCollectorSpline);
        if (absPositionCollectorSpline > 0.35f) ResetCountResource();
        
        //if(positionCollectorSpline is > 0f and < 0.15f or > 0.85f and < 1) ResetCountResource();
    }
    
    public void AddResource()
    {
        if (countOpenChild >= _resourcePiece.childCount) return;
        countOpenChild++;
        _resourcePiece.GetChild(countOpenChild).gameObject.SetActive(true);
    }

    public void ResetCountResource()
    {
        countOpenChild = 0;
        for (int i = 0; i < _resourcePiece.childCount; i++)
        {
            _resourcePiece.GetChild(i).gameObject.SetActive(false);
        }
    }
}
