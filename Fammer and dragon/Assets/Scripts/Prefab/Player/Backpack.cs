using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.ComponentModel;

public class Backpack
{
    public const int MAX_CAPACITY = 50;

    private const int INVALID_ID = -1;
    private ItemBlock[] itemBlocks;
    private int emptyCount = MAX_CAPACITY;

    public Backpack()
    {
        itemBlocks = new ItemBlock[MAX_CAPACITY];
    }
    public void Init()
    {
        //test
        for (int i = 0; i < MAX_CAPACITY; i++)
        {
            itemBlocks[i] = new ItemBlock(i);
        }
        emptyCount = 50;
    }
    //public void SetItem(int _id, Item _item)
    //{
    //    if (emptyCount > 0)
    //    {
    //        if (_id < 0 || _id >= MAX_CAPACITY)
    //        {
    //            return;
    //        }
    //        if (_item == null)
    //        {
    //            return;
    //        }
    //        //同种物品 && 可叠加->叠加
    //        if (itemBlocks[_id].IsIdentical(_item) && _item.MaxStackSize > 1)
    //        {
    //            itemBlocks[_id].Add(_item.stackSize);
    //        }
    //        else
    //        {
    //            itemBlocks[_id].Item = _item;
    //            emptyCount--;
    //        }
    //    }
    //}
    //public void Packing(ItemBlock _block)
    //{
    //    Packing(_block.Item);
    //}
    public void PackingById(int _id, Item _item)
    {
        if (_id < 0 || _id >= MAX_CAPACITY)
        {
            return;
        }
        itemBlocks[_id].Item = _item;
        if (_item != null)
        {
            //状态转换
            _item.gameObject.SetActive(false);
        }
    }
    public void Packing(Item _item)
    {
        if (_item == null)
        {
            return;
        }
        if (IsFull())
        {
            return;
        }
        int id = INVALID_ID;
        //能够叠加
        if (_item.MaxStackSize > 1)
        {
            id = FindItemNotFull(_item);
            //存在未满&&同种物品，优先填满
            if (id != INVALID_ID)
            {
                int temp = _item.MaxStackSize - itemBlocks[id].Size;
                //填充数量
                int fillSize = _item.stackSize > temp ? temp : _item.stackSize;
                //填充
                itemBlocks[id].Add(fillSize);
                //剩余数量
                _item.stackSize -= fillSize;
            }
            if (_item.stackSize > 0)
            {
                while (_item.stackSize > _item.MaxStackSize)
                {
                    id = PackingFirstEmpty(_item, _item.MaxStackSize);
                    //背包已满
                    if (id == INVALID_ID)
                    {
                        break;
                    }
                    _item.stackSize -= _item.MaxStackSize;
                }
                id = PackingFirstEmpty(_item, _item.stackSize);
            }
        }
        else
        {
            id = PackingFirstEmpty(_item, 1);
            //背包已满
            if (id == INVALID_ID)
            {

            }
        }
    }
    //放置在第一个空位置
    public int PackingFirstEmpty(Item _item, int _size)
    {
        int id = FindFirstEmpty();
        if (id != INVALID_ID)
        {
            _item.stackSize -= _size;
            var copy = GameObject.Instantiate<Item>(_item);
            copy.stackSize = _size;
            //状态转换
            copy.gameObject.SetActive(false);
            itemBlocks[id].Item = copy;
            emptyCount--;
        }
        return id;
    }
    public Item TakeOut(int _id, int _size)
    {
        if (_id < 0 || _id >= MAX_CAPACITY)
        {
            return null;
        }
        if (itemBlocks[_id].IsEmpty())
        {
            return null;
        }
        if (_size > itemBlocks[_id].Size)
        {
            return null;
        }
        //全部取出
        if (_size == itemBlocks[_id].Size)
        {
            //状态转换
            var temp = itemBlocks[_id].Item;
            itemBlocks[_id].Item = null;
            emptyCount++;
            return temp;
        }
        //部分取出
        itemBlocks[_id].Remove(_size);
        var copy = GameObject.Instantiate<Item>(itemBlocks[_id].Item);
        copy.stackSize = _size;
        //状态转换
        copy.gameObject.SetActive(false);
        return copy;
    }
    public Item TakeOutAll(int _id)
    {
        if (_id < 0 || _id >= MAX_CAPACITY)
        {
            return null;
        }
        if (itemBlocks[_id].IsEmpty())
        {
            return null;
        }
        var temp = itemBlocks[_id].Item;
        itemBlocks[_id].Item = null;
        emptyCount++;
        return temp;
    }
    public bool IsFull()
    {
        return emptyCount > 0 ? false : true;
    }
    public int FindItemNotFull(Item _item)
    {
        for (int i = 0; i < MAX_CAPACITY; i++)
        {
            if (itemBlocks[i].IsIdentical(_item) && itemBlocks[i].Size < _item.MaxStackSize)
            {
                return i;
            }
        }
        return INVALID_ID;
    }
    public int FindFirstEmpty()
    {
        if (IsFull())
        {
            return INVALID_ID;
        }
        int id = INVALID_ID;
        for (int i = 0; i < MAX_CAPACITY; i++)
        {
            if (itemBlocks[i].IsEmpty())
            {
                id = i;
                break;
            }
        }
        return id;
    }
}
