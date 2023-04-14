using System;
using System.Collections;
using System.Collections.Generic;
using Dreamteck.Splines;
using UnityEngine;
using TMPro;
using Random = UnityEngine.Random;
using DG.Tweening;

public class crasher : MonoBehaviour
{
    private float _splinePositionX = -1f;
    private float _maxLong = -2.7f;
    
    private int _openPartBoer;
    
    private float _speedRotation = -0.2f;
    public float speedMax = -0.2f;
    public float forceDamage = 0.2f;
    
    public GameObject resource;
    public int openCellResource = 1; // 1
    private int _takeOffForce = 200;
    private float _resourceSpawnPosition = -2.5f;
    private float _boerPeakSize = 0.4f;

    private cameraMove _cameraMove;
    private spawnResource _spawnResource;
    private tower _depthTower;
    private collector _collector;
    private Transform _resourceBox;
    private Transform _transporterLast;

    private bool _permission;
    private Collider _colliderPermission;
    
    public List<Transform> boers;
    public List<boer> boerScripts;

    public TextMeshProUGUI AddNewCrasherMoney;
    public TextMeshProUGUI LvlUpCrasherMoney;

    public RectTransform ButtonAddNew;
    public RectTransform ButtonLvlUp;
    
    public int priceAddNewBoer = 100;
    public int priceLvlUpBoer = 100;
    

    private void Awake()
    {
        _cameraMove = FindObjectOfType<cameraMove>();
        _spawnResource = FindObjectOfType<spawnResource>();
        _depthTower = FindObjectOfType<tower>();
        _collector = FindObjectOfType<collector>();
        
        _resourceBox = GameObject.Find("resource").transform;
        _transporterLast = GameObject.Find("transporter last").transform;
        
        AddNewCrasherMoney.text = priceAddNewBoer.ToString();
        LvlUpCrasherMoney.text = priceLvlUpBoer.ToString();

        for (int i = openCellResource; i < 11; i++)
        {
            boers[i].gameObject.SetActive(false);
        }
        
    }

    private void Update()
     {
         transform.Rotate(0f,_speedRotation,0f);

         if (_colliderPermission)
         {
             DestroySegmentResource(_colliderPermission, _permission);
             _colliderPermission = null;
         }
     }
    
    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 6) _speedRotation = 0f;
    }

    public void OnTriggerStay(Collider other)
    {
        var resourceLine = other.GetComponent<scriptResource>();
        if (other.gameObject.layer == 6)
        {
            resourceLine.resourceHP -= forceDamage;
            resourceLine.ShakingResource(openCellResource);
            foreach (var boerScript in boerScripts)
            {
                boerScript.ResourceTouch();
            }

            if (resourceLine.resourceHP > 0) return;
            foreach (var boerScript in boerScripts)
            {
                boerScript.ResourceBroken();
            }
            _permission = false;
            _colliderPermission = other;

            _speedRotation = speedMax;
        }
    }

    private void DestroySegmentResource(Collider segmentResource,bool work)
    {
        if(work) return;
        
        _permission = true;
        
        //Destroy(segmentResource.gameObject);
        segmentResource.gameObject.SetActive(false);
        
        _spawnResource.segments.Remove(segmentResource.transform);
        
        //_spawnResource.NewResourcePiece(); //Создание новых триуголок русурсов

        _depthTower.depth += -0.05f; //спуск башни
        _cameraMove.TowerDepth();
        
        for(int i = 0; i < openCellResource; i++)
        {
            for(int j = 0; j < 5; j++)
            {
                var newResource = Instantiate(resource,transform.GetChild(i),false);
                var newResourcePos = newResource.transform.position;
                newResource.transform.position = new Vector3(newResourcePos.x, newResourcePos.y, newResourcePos.z);
    
            
                newResource.GetComponent<Rigidbody>().AddForce(transform.forward * _takeOffForce * Random.Range(0.7f, 2));
                newResource.GetComponent<Rigidbody>().AddForce(transform.up * _takeOffForce * Random.Range(0.7f, 2));
                newResource.transform.SetParent(_resourceBox);
                newResource.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
            }
            _resourceSpawnPosition += -1;
        }
    }

    public void ButtonAddNewBoer()
    {
        if(priceAddNewBoer > _collector.money) return;

        _openPartBoer++;

        _collector.PutAwayMoney(-priceAddNewBoer);
        priceAddNewBoer += 500;
        
        boers[openCellResource].gameObject.SetActive(true);
        openCellResource++;
        
        for (int i = 0; i < openCellResource; i++) boers[i].DOPunchScale(new Vector3(4f,2f,4f),0.5f,1,0);
        for (int i = 0; i < openCellResource; i++) boers[i].DOScale(new Vector3(1, 0.5f, 1), 0.5f);
        ButtonAddNew.DOPunchScale(new Vector3(1f,1f,1f),0.5f,1,0);
        ButtonAddNew.DOScale(new Vector3(1, 1, 1), 0.75f);
        
        _collector.collectorHolder[_collector.openPartCollectorHolder].gameObject.SetActive(true);
        _collector.openPartCollectorHolder++;
        
        boerScripts.Add(boers[_openPartBoer].GetComponent<boer>());
        
        _transporterLast.localPosition = new Vector3(_transporterLast.localPosition.x - 1f, 0f, 0f);
        
        _maxLong += _splinePositionX;

        var averageLongUp = _maxLong / 3;
        var averageLongDown = _maxLong / 3;
        
        for (int i = 1; i <= 3; i++)
        {
            var position =_collector.splineComputer.GetPointPosition(i,SplineComputer.Space.Local);
            _collector.splineComputer.SetPointPosition(i, new Vector3(averageLongUp, 0.65f, 0f),
                SplineComputer.Space.Local);

            averageLongUp += _maxLong / 3;
        }
        for (int i = 6; i >= 4; i--)
        {
            var position =_collector.splineComputer.GetPointPosition(i,SplineComputer.Space.Local);
            _collector.splineComputer.SetPointPosition(i, new Vector3(averageLongDown, 1.15f, 0f),
                SplineComputer.Space.Local);

            averageLongDown += _maxLong / 3;
        }

        AddNewCrasherMoney.text = priceAddNewBoer.ToString();
        
        _cameraMove.BiasCamera();
        _spawnResource.ButtonAddSegment();
        _collector.ButtonAddNewCollector();
        //_collector.CollectorsPosition();
    }

    public void ButtonLvlUpBoer()
    {
        if(priceLvlUpBoer > _collector.money) return;

        _collector.PutAwayMoney(-priceLvlUpBoer);
        priceLvlUpBoer += 250;
        
        for (int i = 0; i < openCellResource; i++)
        {
            for (int j = 1; j < boers[i].GetChild(0).childCount; j++)
            {
                 boers[i].GetChild(0).GetChild(j).DOPunchScale(new Vector3(10f, 10f, 10f), 0.5f, 1, 0);
            }
            for (int j = 1; j < boers[i].GetChild(0).childCount; j++)
            {
                boers[i].GetChild(0).GetChild(j).DOScale(new Vector3(_boerPeakSize, _boerPeakSize, _boerPeakSize), 0.75f);
            }
        }
        ButtonLvlUp.DOPunchScale(new Vector3(4f,4f,4f),0.5f,1,0);
        ButtonLvlUp.DOScale(new Vector3(1, 1, 1), 1f);
        
        speedMax += -0.2f;
        forceDamage += 0.2f;
        _boerPeakSize += 0.1f;

        foreach (Transform boer in boers)
        {
            var grinder = boer.GetChild(0);
            for (int i = 1; i < grinder.childCount; i++)
            {
                grinder.GetChild(i).localScale = new Vector3(_boerPeakSize,_boerPeakSize,_boerPeakSize);
            }
        }
        
        LvlUpCrasherMoney.text = priceLvlUpBoer.ToString();
        
        _collector.ButtonLvlUpCollector();
    }
}
