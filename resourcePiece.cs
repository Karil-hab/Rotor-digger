using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class resourcePiece : MonoBehaviour
{
    public List<Transform> _resourceCollector;

    private collector _collector;
    private int _resourceCollectorNum;

    private bool _permissionMove = true;
    private void Start()
    {
        _collector = FindObjectOfType<collector>();
    }

    private void Update()
    {
        if(_permissionMove) return;
        transform.DOMove(_collector.resourceCountCollector[_resourceCollectorNum].position, 1);
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer != 8 || _collector.maxCountResource <= _collector.countResource || _collector.resourceCollector.Count < 1) return;

        _resourceCollector = _collector.resourceCollector;
        
        _collector.countResource++;
        _collector.money += 20;
        _collector.countMoney.text = _collector.money.ToString();

        float maxMinLength = 100f;
        
        for (int i = 0; i < _resourceCollector.Count; i++)
        {
            var length = Vector3.Distance(_resourceCollector[i].position ,transform.position);
            
            if (maxMinLength > length)
            {
                maxMinLength = length;
                
                _resourceCollectorNum = _resourceCollector[i].GetComponent<raiseCollector>().numberCollector;
            }
        }
        
        _collector.resourceCountCollector[_resourceCollectorNum].GetComponent<raiseCollector>().AddResource();
        
        StartCoroutine(DestroyResource());
        _permissionMove = false;
        transform.DOScale(new Vector3(0.1f,0.1f,0.1f),1);
    }
    
    IEnumerator DestroyResource()
    {
        yield return new WaitForSeconds(1.0f);
        Destroy(gameObject);
    }
}
