using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RDUI
{
    public class UIManager : MonoBehaviour
    {

        public static UIManager instance = null;
        private Dictionary<string, BasePage> pages;//缓存Page,根据场景加载
        //不同层级对应父对象
        private GameObject canvas;
        private GameObject uiHUD;
        private GameObject uiPage;
        private GameObject uiat;
        private GameObject uiPopUp;
        void Awake()
        {
            if (instance == null)
                instance = this;
            else if (instance != this)
                Destroy(gameObject);

            pages = new Dictionary<string, BasePage>();
            DontDestroyOnLoad(gameObject);
        }
        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
        //根据场景加载Page
        public void LoadPage(string _sceneName)
        {
            pages.Clear();
        }
        //打开Page
        public void OpenPage(string _pageName)
        {
            pages[_pageName].Open();
        }
        //关闭Page
        public void ClosePage(string _pageName)
        {
            pages[_pageName].Close();
        }

    }
}

