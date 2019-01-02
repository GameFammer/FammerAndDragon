using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBlock
{
    private int id;
    private Item item;
    public Item Item
    {
        get
        {
            return item;
        }
        set
        {
            item = value;
            FDEventFactor.Broadcast<int, Item>(FDEvent.Refresh_Backpack, id, item);
        }
    }
    public int Size
    {
        get
        {
            return item != null ? item.stackSize : 0;
        }
    }
    public int MaxStackSize
    {
        get
        {
            return item != null ? item.MaxStackSize : 0;
        }
    }
    public ItemBlock(int _id)
    {
        id = _id;
        item = null;
    }
    public int Add(int _count)
    {
        if (item == null)
        {
            return 0;
        }
        item.stackSize += _count;
        FDEventFactor.Broadcast<int, Item>(FDEvent.Refresh_Backpack, id, item);
        return item.stackSize;
    }
    public int Remove(int _count)
    {
        if (item == null)
        {
            return 0;
        }
        if (_count > item.stackSize)
        {
            return item.stackSize;
        }
        item.stackSize -= _count;
        FDEventFactor.Broadcast<int, Item>(FDEvent.Refresh_Backpack, id, item);
        return item.stackSize;
    }
    public bool IsEmpty()
    {
        return item == null ? true : false;
    }
    public bool IsFull()
    {
        if (item != null)
        {
            return item.stackSize >= item.MaxStackSize ? true : false;
        }
        return false;
    }
    public bool IsIdentical(Item _item)
    {
        if (item == null || _item == null)
        {
            return false;
        }
        if (item.Name == _item.Name)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
