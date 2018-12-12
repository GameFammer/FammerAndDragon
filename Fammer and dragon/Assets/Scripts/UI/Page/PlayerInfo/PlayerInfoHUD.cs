using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace FDUI
{
    public class PlayerInfoHUD : BasePage
    {
        [SerializeField]
        private Image HpBarImage;
        [SerializeField]
        private Image HpEmptyBarImage;

        private float hpTextureHeight;//每格Hp贴图高度，要求每格Hp贴图为正方形
        private int playerHpMax;//玩家当前血量上限
        // Use this for initialization
        protected override void Init()
        {
            base.Init();
            //获取每格血高度
            hpTextureHeight = HpBarImage.rectTransform.sizeDelta.y;
            //获取Player当前血量上限
            playerHpMax = GameObject.Find("player").GetComponent<PlayerProperties>().HpMaxValue;
            //根据Player血量初始化
            HpBarImage.fillAmount = (float)playerHpMax / (float)PlayerProperties.HP_MaxLimit_Value;
            HpEmptyBarImage.fillAmount = 0;
            HpEmptyBarImage.rectTransform.localPosition = new Vector3((playerHpMax - PlayerProperties.HP_MaxLimit_Value) * hpTextureHeight, 0, 0);
            //绑定回调
            UIDelegateManager.AddObserver(UIMessageType.Updata_HpMax, OnHpMaxChange);
            UIDelegateManager.AddObserver(UIMessageType.Updata_Hp, OnHpChange);
        }
        //生命值上限改变,自动回满血
        public void OnHpMaxChange(object _hpMaxLimit)
        {
            //记录
            playerHpMax = (int)_hpMaxLimit;
            //回满血
            HpEmptyBarImage.fillAmount = 0;
            //重置 HpEmptyBarImage位置,x:(当前血上限-最大血上限)*每格血宽度（高度）
            HpEmptyBarImage.rectTransform.localPosition = new Vector3((playerHpMax - PlayerProperties.HP_MaxLimit_Value) * hpTextureHeight,0 , 0);
            //更改血上限
            HpBarImage.fillAmount=(float)playerHpMax / (float)PlayerProperties.HP_MaxLimit_Value;

        }
        public void OnHpChange(object _hp)
        {
            //更改血量
            HpEmptyBarImage.fillAmount = (float)(playerHpMax-(int)_hp)/ (float)PlayerProperties.HP_MaxLimit_Value;
        }
    }
}

