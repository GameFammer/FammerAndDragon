using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.ComponentModel;
using System.Linq;

public class Backpack
{
    public const int MAX_CAPACITY = 50;

    private const int INVALID_ID = -1;
    private Dictionary<int, ItemBlock> itemBlockDic = new Dictionary<int, ItemBlock>();
    private int emptyCount = MAX_CAPACITY;

    public Backpack()
    {
        for (int i = 0; i < MAX_CAPACITY; i++)
        {
            itemBlockDic.Add(i, new ItemBlock(i));
        }
    }

    public void PackingById(int _id, Item _item)
    {
        if (_id < 0 || _id >= MAX_CAPACITY)
        {
            return;
        }
        itemBlockDic[_id].Item = _item;
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
        if (emptyCount <= 0)
        {
            return;
        }
        int id = INVALID_ID;
        //能够叠加
        if (_item.MaxStackSize > 1)
        {
            id = FindAvailableItemCellId(_item);
            //存在未满&&同种物品，优先填满
            if (id != INVALID_ID)
            {
                int temp = _item.MaxStackSize - itemBlockDic[id].Size;
                //填充数量
                int fillSize = _item.stackSize > temp ? temp : _item.stackSize;
                //填充
                itemBlockDic[id].Add(fillSize);
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
        int id = FindEmptyCellId();
        if (id != INVALID_ID)
        {
            _item.stackSize -= _size;
            var copy = GameObject.Instantiate<Item>(_item);
            copy.stackSize = _size;
            //状态转换
            copy.gameObject.SetActive(false);
            itemBlockDic[id].Item = copy;
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
        if (itemBlockDic[_id].IsEmpty())
        {
            return null;
        }
        if (_size > itemBlockDic[_id].Size)
        {
            return null;
        }
        //全部取出
        if (_size == itemBlockDic[_id].Size)
        {
            //状态转换
            var temp = itemBlockDic[_id].Item;
            itemBlockDic[_id].Item = null;
            emptyCount++;
            return temp;
        }
        //部分取出
        itemBlockDic[_id].Remove(_size);
        var copy = GameObject.Instantiate<Item>(itemBlockDic[_id].Item);
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
        if (itemBlockDic[_id].IsEmpty())
        {
            return null;
        }
        var temp = itemBlockDic[_id].Item;
        itemBlockDic[_id].Item = null;
        emptyCount++;
        return temp;
    }

    public int FindAvailableItemCellId(Item _item)
    {
        KeyValuePair<int, ItemBlock>[] pairs = itemBlockDic.Where(pair => pair.Value.IsIdentical(_item) && pair.Value.Size < _item.MaxStackSize).ToArray();
        return pairs.Length > 0 ? pairs[0].Key : INVALID_ID;
    }

    public int FindEmptyCellId()
    {
        KeyValuePair<int, ItemBlock>[] pairs = itemBlockDic.Where(pair => pair.Value.IsEmpty()).ToArray();
        return pairs.Length > 0 ? pairs[0].Key : INVALID_ID;
    }
}
