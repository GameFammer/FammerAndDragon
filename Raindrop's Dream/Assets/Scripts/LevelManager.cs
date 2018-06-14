using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance = null;

    public GameObject levelOne;
    void Awake()
    {
        //单例
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

    }
    // Use this for initialization

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
