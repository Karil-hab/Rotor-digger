using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using TMPro;
using UnityEngine;
using Dreamteck.Splines;
using Unity.VisualScripting;

public class collector : MonoBehaviour
{
    public float speedRotation = -0.2f;
    public float openPartCollector = 1f;
    public int openPartCollectorHolder = 1;
    private int _numberCollector;

    private bool _permissionGrab = false;
    
    public int maxCountResource = 1;
    public int countResource;
    public float breakTimeGather = 0.5f;
    
    public Transform prefabCollector;

    private crasher _crasher;
    
    public List<Transform> collectorHolder;
    public List<Transform> resourceCollector;
    public List<Transform> resourceCountCollector;
    private List<float> _fixingDistance;
    
    private Transform _childCollectorHolder;
    private Transform _childCollector;

    public TextMeshProUGUI countMoney; 
    public int money = 500;

    public SplineComputer splineComputer;
    private SplineProjector _projector;
    private void Awake()
    {
        _crasher = FindObjectOfType<crasher>();

        splineComputer = transform.GetComponentInChildren<SplineComputer>();
        _projector = GetComponentInChildren<SplineProjector>();
        
        _childCollectorHolder = transform.GetChild(0);
        _childCollector = transform.GetChild(1);
        
        for (int i = openPartCollectorHolder; i < 11; i++)
        {
            collectorHolder[i].gameObject.SetActive(false);
        }
        
        countMoney.text = money.ToString();
    }

    void Update()
    {
        transform.Rotate(0f,speedRotation,0f);
        //print(projector.result.percent);
    }

    private void FixedUpdate()
    {
        if ((countResource != maxCountResource) || _permissionGrab) return;
        _permissionGrab = true;
        StartCoroutine(ClearCollector());
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer != 9) return;
        
        resourceCollector.Add(other.transform);
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer != 9) return;
        
        resourceCollector.Remove(other.transform);
    }

    IEnumerator ClearCollector()
    {
        yield return new WaitForSeconds(breakTimeGather);
        _permissionGrab = false;
        countResource = 0;
    }
    

    public void ButtonAddNewCollector()
    {
        var newCollector = Instantiate(prefabCollector, _childCollector);

        _numberCollector++;
        newCollector.GetComponent<raiseCollector>().numberCollector = _numberCollector;

        var splineCollector = newCollector.gameObject.GetComponentInChildren<SplineFollower>();
        var splineProjectorCollector = newCollector.gameObject.GetComponentInChildren<SplineProjector>();
        
        splineProjectorCollector.spline = splineComputer;
        
        splineCollector.spline = splineComputer;
        splineCollector.wrapMode = SplineFollower.Wrap.Loop;

        var meaningPositionLastCollector = resourceCountCollector.Last().gameObject.GetComponentInChildren<SplineProjector>().result.percent;
        
        var lastCollectorDistance = meaningPositionLastCollector * splineComputer.CalculateLength();
        
        var startPosition = meaningPositionLastCollector-0.92f;
        if (startPosition < 0f) startPosition += 1f;
        
        splineCollector.startPosition = startPosition;

        resourceCountCollector.Add(splineCollector.transform);
        
        transform.GetChild(0).DOPunchScale(new Vector3(0.5f, 4f, 4f), 0.5f, 1, 0);
        transform.GetChild(0).DOScale(new Vector3(1f, 1f, 1f), 0.5f);
        
        openPartCollector++;
        maxCountResource++;
    }

    public void ButtonLvlUpCollector()
    {
        for (int i = 0; i < resourceCountCollector.Count; i++) resourceCountCollector[i].DOPunchScale(new Vector3(1.8f,1.8f,1.8f),0.5f,1,0);
        for (int i = 0; i < resourceCountCollector.Count; i++) resourceCountCollector[i].DOScale(new Vector3(0.3f, 0.3f, 0.3f), 1f);
        
        speedRotation += -0.1f;
        maxCountResource += 1;
        if(breakTimeGather > 0.05f) breakTimeGather -= 0.05f;
    }

    public void PutAwayMoney(int giveMoney)
    {
        money += giveMoney;
        countMoney.text = money.ToString();
    }
}
