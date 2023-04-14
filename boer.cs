using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class boer : MonoBehaviour
{
    public float speedRotation = 5f;
    public ParticleSystem effectGrind;
    public ParticleSystem effectGrind2;
    public Transform effectBroken;
    public Transform effectBroken2;

    private void Awake()
    {
        effectGrind = GetComponentInChildren<ParticleSystem>();
        effectGrind2 = transform.GetChild(2).GetComponent<ParticleSystem>();
        
        effectBroken = transform.GetChild(3).GetComponent<Transform>();
        effectBroken2 = transform.GetChild(4).GetComponent<Transform>();
    }

    void Update()
    {
        transform.GetChild(0).Rotate(0f,0f,speedRotation);
    }

    public void ResourceTouch()
    {
        effectGrind.Play();
        effectGrind2.Play();
        
        effectBroken.gameObject.SetActive(false);
        effectBroken2.gameObject.SetActive(false);
    }
    public void ResourceBroken()
    {
        effectGrind.Stop();
        effectGrind2.Stop();
        
        effectBroken.gameObject.SetActive(true);
        effectBroken2.gameObject.SetActive(true);
    }
}
