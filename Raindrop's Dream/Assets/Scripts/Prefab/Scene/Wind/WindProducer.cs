using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindProducer : MonoBehaviour,ISleepWakeUp
{
    public int windLength;//风的长度
    private GameObject wind;

    // Use this for initialization
    void Start()
    {
        wind = transform.GetChild(0).gameObject;
        wind.transform.localScale = new Vector3(windLength/transform.localScale.x, 1, 1);
        wind.transform.localPosition = new Vector3(wind.transform.localScale.x / 2.0f + 0.5f, 0, 0);
    }
    public void Sleep()
    {
        wind.SetActive(false);
    }

    public void WakeUp()
    {
        wind.SetActive(true);
    }
}
