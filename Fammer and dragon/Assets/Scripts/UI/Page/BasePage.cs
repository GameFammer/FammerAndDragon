using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RDUI
{
    public enum PageType
    {
        HUD,//游戏内UI，如血量
        Page,//页面UI，如设置界面
        Float,//浮动消息,如Boss技能提示
        PopUp//弹出窗口，如MessageBox
    }
    public enum PageStatus
    {
        Active,//正在显示
        DeActive//未显示
    }
    public class BasePage : MonoBehaviour
    {
        public PageType pageType;//页面类型
        public PageStatus pageStatus = PageStatus.DeActive;//页面状态
        void Start()
        {
            Init();
        }
        //初始化
        protected virtual void Init()
        {
            if (pageStatus == PageStatus.DeActive)
            {
                gameObject.SetActive(false);
            }
        }
        //打开
        public virtual void Open()
        {
            pageStatus = PageStatus.Active;
            gameObject.SetActive(true);
        }
        //关闭
        public virtual void Close()
        {
            pageStatus = PageStatus.DeActive;
            gameObject.SetActive(false);
        }
        public virtual bool IsOpen()
        {
            if (pageStatus == PageStatus.Active)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}

