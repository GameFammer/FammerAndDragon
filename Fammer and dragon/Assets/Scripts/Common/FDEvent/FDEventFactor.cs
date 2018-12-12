using System;
using System.Collections.Generic;

public class FDEventFactor
{
    private static Dictionary<FDEvent, Delegate> fd_EventCenter = new Dictionary<FDEvent, Delegate>();

    public static void AddListener(FDEvent fdEvent, FDDelegate fdDelegate)
    {
        if (!ValidateAdd(fdEvent, fdDelegate)) return;
        fd_EventCenter[fdEvent] = (FDDelegate)fd_EventCenter[fdEvent] + fdDelegate;
    }

    public static void AddListener<T>(FDEvent fdEvent, FDDelegate<T> fdDelegate)
    {
        if (!ValidateAdd(fdEvent, fdDelegate)) return;
        fd_EventCenter[fdEvent] = (FDDelegate<T>)fd_EventCenter[fdEvent] + fdDelegate;
    }

    public static void AddListener<T, A>(FDEvent fdEvent, FDDelegate<T, A> fdDelegate)
    {
        if (!ValidateAdd(fdEvent, fdDelegate)) return;
        fd_EventCenter[fdEvent] = (FDDelegate<T, A>)fd_EventCenter[fdEvent] + fdDelegate;
    }

    public static void AddListener<T, A, B>(FDEvent fdEvent, FDDelegate<T, A, B> fdDelegate)
    {
        if (!ValidateAdd(fdEvent, fdDelegate)) return;
        fd_EventCenter[fdEvent] = (FDDelegate<T, A, B>)fd_EventCenter[fdEvent] + fdDelegate;
    }

    public static void RemoveListener(FDEvent fdEvent, FDDelegate fdDelegate)
    {
        if (!ValidateRemove(fdEvent, fdDelegate)) return;
        fd_EventCenter[fdEvent] = (FDDelegate)fd_EventCenter[fdEvent] - fdDelegate;
        AfterRemoved(fdEvent);
    }

    public static void RemoveListener<T>(FDEvent fdEvent, FDDelegate<T> fdDelegate)
    {
        if(!ValidateRemove(fdEvent, fdDelegate)) return;
        fd_EventCenter[fdEvent] = (FDDelegate<T>)fd_EventCenter[fdEvent] - fdDelegate;
        AfterRemoved(fdEvent);
    }

    public static void RemoveListener<T, A>(FDEvent fdEvent, FDDelegate<T, A> fdDelegate)
    {
        if(!ValidateRemove(fdEvent, fdDelegate)) return;
        fd_EventCenter[fdEvent] = (FDDelegate<T, A>)fd_EventCenter[fdEvent] - fdDelegate;
        AfterRemoved(fdEvent);
    }

    public static void RemoveListener<T, A, B>(FDEvent fdEvent, FDDelegate<T, A, B> fdDelegate)
    {
        if(!ValidateRemove(fdEvent, fdDelegate)) return;
        fd_EventCenter[fdEvent] = (FDDelegate<T, A, B>)fd_EventCenter[fdEvent] - fdDelegate;
        AfterRemoved(fdEvent);
    }

    public static void Broadcast(FDEvent fdEvent)
    {
        Delegate d;
        if (fd_EventCenter.TryGetValue(fdEvent, out d))
        {
            FDDelegate fdDelegate = d as FDDelegate;
            if (fdDelegate != null)
            {
                fdDelegate();
            }
        }
    }

    public static void Broadcast<T>(FDEvent fdEvent, T arg)
    {
        Delegate d;
        if (fd_EventCenter.TryGetValue(fdEvent, out d))
        {
            FDDelegate<T> fdDelegate = d as FDDelegate<T>;
            if (fdDelegate != null)
            {
                fdDelegate(arg);
            }
        }
    }

    public static void Broadcast<T, A>(FDEvent fdEvent, T arg1, A arg2)
    {
        Delegate d;
        if (fd_EventCenter.TryGetValue(fdEvent, out d))
        {
            FDDelegate<T, A> fdDelegate = d as FDDelegate<T, A>;
            if (fdDelegate != null)
            {
                fdDelegate(arg1, arg2);
            }
        }
    }

    public static void Broadcast<T, A, B>(FDEvent fdEvent, T arg1, A arg2, B arg3)
    {
        Delegate d;
        if (fd_EventCenter.TryGetValue(fdEvent, out d))
        {
            FDDelegate<T, A, B> fdDelegate = d as FDDelegate<T, A, B>;
            if (fdDelegate != null)
            {
                fdDelegate(arg1, arg2, arg3);
            }
        }
    }

    public static void RemoveEvent(FDEvent fdEvent)
    {
        if (fd_EventCenter.ContainsKey(fdEvent))
        {
            fd_EventCenter.Remove(fdEvent);
        }
    }

    private static Boolean ValidateAdd(FDEvent fdEvent, Delegate fdDelegate)
    {
        if (!fd_EventCenter.ContainsKey(fdEvent))
        {
            fd_EventCenter.Add(fdEvent, null);
        }
        if (fd_EventCenter[fdEvent] != null && fd_EventCenter[fdEvent].GetType() != fdDelegate.GetType())
        {
            return false;
        }
        return true;
    }

    private static Boolean ValidateRemove(FDEvent fdEvent, Delegate fdDelegate)
    {
        if (fd_EventCenter.ContainsKey(fdEvent))
        {
            if (fd_EventCenter[fdEvent] != null && fd_EventCenter[fdEvent].GetType() == fdDelegate.GetType())
            {
                return true;
            }
        }
        return false;
    }

    private static void AfterRemoved(FDEvent fdEvent)
    {
        if (fd_EventCenter[fdEvent] == null)
        {
            fd_EventCenter.Remove(fdEvent);
        }
    }
}