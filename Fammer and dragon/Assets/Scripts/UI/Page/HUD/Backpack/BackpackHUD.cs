using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FDUI
{
    public class BackpackHUD : BasePage
    {
        public const int FASTBLOCK_COUNT = 10;
        public const int BLOCK_COUNT = 40;

        public FastBlock fastBlockPrefab;
        public Block blockPrefab;

        public Transform fastInventory;
        public Transform inventory;
        //物品栏
        public Block[] blocks;

        private  Backpack backpack;
        private FDCursor cursor;
        // Use this for initialization
        protected override void Init()
        {
            backpack = GameObject.Find("Player").GetComponent<PlayerProperties>().backpack;
            cursor = GameObject.Find("Cursor").GetComponent<FDCursor>();

            int count = FASTBLOCK_COUNT + BLOCK_COUNT;
            blocks = new Block[count];
            for (int i = 0; i < count; i++)
            {
                blocks[i] = i < FASTBLOCK_COUNT ? Instantiate<FastBlock>(fastBlockPrefab, fastInventory) : blocks[i] = Instantiate<Block>(blockPrefab, inventory);
                blocks[i].Id = i;
                blocks[i].Init(backpack,cursor);
            }
            base.Init();
        }
        public override void Open()
        {
            FDEventFactor.AddListener<int, Item>(FDEvent.Refresh_Backpack, Refresh);
            base.Open();
        }

        private void Refresh(int _id,Item _item)
        {
            //Test
            if(_item == null)
            {
                blocks[_id].SetBlock(null, 0);
                return;
            }
            if(_id<0 || _id >= FASTBLOCK_COUNT + BLOCK_COUNT)
            {
                return;
            }
            SpriteRenderer render=_item.GetComponent<SpriteRenderer>();
            if(render == null)
            {
                return;
            }
            blocks[_id].SetBlock(render.sprite, _item.stackSize);
        }
    }
}
