using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FDUI
{
    public class UIManager : MonoBehaviour
    {

        public static UIManager instance = null;
        private Dictionary<string, BasePage> pages;//缓存Page,根据场景加载:{"页面名"，"页面对象"}
        //不同层级对应父对象
        private Transform canvas;
        private Transform uiHUD;
        private Transform uiPage;
        private Transform uiFloat;
        private Transform uiPopUp;
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

            canvas = GameObject.Find("Canvas").transform;
            uiHUD = canvas.transform.Find("HUD");
            uiPage = canvas.transform.Find("Page");
            uiFloat = canvas.transform.Find("Float");
            uiPopUp = canvas.transform.Find("PopUP");
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
        //获取Page
        public BasePage GetPage(string _pageName)
        {
            return pages[_pageName];
        }
    }
}

