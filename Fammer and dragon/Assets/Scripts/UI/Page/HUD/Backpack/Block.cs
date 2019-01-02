using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
namespace FDUI
{
    public class Block : MonoBehaviour, IPointerEnterHandler, IPointerDownHandler
    {
        public Image icon;
        public Text count;

        public int Id { get; set; }

        private Backpack backpack;
        private FDCursor cursor;
        public void Init(Backpack _backpack,FDCursor _cursor)
        {
            icon.sprite = null;
            count.text = string.Empty;
            backpack = _backpack;
            cursor = _cursor;
        }
        public void SetBlock(Sprite _icon, int _count)
        {
            if (_count == 0)
            {
                icon.sprite = null;
                count.text = string.Empty;
            }
            else
            {
                icon.sprite = _icon;
                count.text = _count.ToString();
            }
        }
        public bool IsEmpty()
        {
            return count.text == string.Empty ? true : false;
        }
        public virtual void OnPointerDown(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                //空的
                if (IsEmpty())
                {
                    //有已取出物品，放置
                    if(cursor.FollowObj !=null)
                    {
                        backpack.PackingById(Id,cursor.FollowObj.GetComponent<Item>());
                        cursor.FollowObj = null;
                    }
                }
                //非空，取出
                else
                {
                    var item = backpack.TakeOutAll(Id);
                    //有已取出物品，替换
                    if (cursor.FollowObj != null)
                    {
                        backpack.PackingById(Id,cursor.FollowObj.GetComponent<Item>());
                    }                 
                    cursor.FollowObj = item.gameObject;
                }
            }
        }

        public virtual void OnPointerEnter(PointerEventData eventData)
        {
            // throw new System.NotImplementedException();
        }
    }
}

