using System;
using System.Collections.Generic;

public class FDEventFactor
{
    private static Dictionary<FDEvent, Dictionary<Type, Delegate>> fd_EventCenter = new Dictionary<FDEvent, Dictionary<Type, Delegate>>();

    public static void AddListener(FDEvent fdEvent, FDDelegate fdDelegate)
    {
        if (!ValidateAdd(fdEvent, fdDelegate)) return;
        fd_EventCenter[fdEvent][typeof(FDDelegate)] = (FDDelegate)fd_EventCenter[fdEvent][typeof(FDDelegate)] + fdDelegate;
    }

    public static void AddListener<T>(FDEvent fdEvent, FDDelegate<T> fdDelegate)
    {
        if (!ValidateAdd(fdEvent, fdDelegate)) return;
        fd_EventCenter[fdEvent][typeof(FDDelegate<T>)] = (FDDelegate<T>)fd_EventCenter[fdEvent][typeof(FDDelegate<T>)] + fdDelegate;
    }

    public static void AddListener<T, A>(FDEvent fdEvent, FDDelegate<T, A> fdDelegate)
    {
        if (!ValidateAdd(fdEvent, fdDelegate)) return;
        fd_EventCenter[fdEvent][typeof(FDDelegate<T, A>)] = (FDDelegate<T, A>)fd_EventCenter[fdEvent][typeof(FDDelegate<T, A>)] + fdDelegate;
    }

    public static void AddListener<T, A, B>(FDEvent fdEvent, FDDelegate<T, A, B> fdDelegate)
    {
        if (!ValidateAdd(fdEvent, fdDelegate)) return;
        fd_EventCenter[fdEvent][typeof(FDDelegate<T, A, B>)] = (FDDelegate<T, A, B>)fd_EventCenter[fdEvent][typeof(FDDelegate<T, A, B>)] + fdDelegate;
    }

    public static void AddListener<T, A, B, C>(FDEvent fdEvent, FDDelegate<T, A, B, C> fdDelegate)
    {
        if (!ValidateAdd(fdEvent, fdDelegate)) return;
        fd_EventCenter[fdEvent][typeof(FDDelegate<T, A, B, C>)] = (FDDelegate<T, A, B, C>)fd_EventCenter[fdEvent][typeof(FDDelegate<T, A, B, C>)] + fdDelegate;
    }

    public static void RemoveListener(FDEvent fdEvent, FDDelegate fdDelegate)
    {
        if (!ValidateRemove(fdEvent, fdDelegate)) return;
        fd_EventCenter[fdEvent][typeof(FDDelegate)] = (FDDelegate)fd_EventCenter[fdEvent][typeof(FDDelegate)] - fdDelegate;
        AfterRemoved(fdEvent);
    }

    public static void RemoveListener<T>(FDEvent fdEvent, FDDelegate<T> fdDelegate)
    {
        if (!ValidateRemove(fdEvent, fdDelegate)) return;
        fd_EventCenter[fdEvent][typeof(FDDelegate<T>)] = (FDDelegate<T>)fd_EventCenter[fdEvent][typeof(FDDelegate<T>)] - fdDelegate;
        AfterRemoved(fdEvent);
    }

    public static void RemoveListener<T, A>(FDEvent fdEvent, FDDelegate<T, A> fdDelegate)
    {
        if (!ValidateRemove(fdEvent, fdDelegate)) return;
        fd_EventCenter[fdEvent][typeof(FDDelegate<T, A>)] = (FDDelegate<T, A>)fd_EventCenter[fdEvent][typeof(FDDelegate<T, A>)] - fdDelegate;
        AfterRemoved(fdEvent);
    }

    public static void RemoveListener<T, A, B>(FDEvent fdEvent, FDDelegate<T, A, B> fdDelegate)
    {
        if (!ValidateRemove(fdEvent, fdDelegate)) return;
        fd_EventCenter[fdEvent][typeof(FDDelegate<T, A, B>)] = (FDDelegate<T, A, B>)fd_EventCenter[fdEvent][typeof(FDDelegate<T, A, B>)] - fdDelegate;
        AfterRemoved(fdEvent);
    }

    public static void RemoveListener<T, A, B, C>(FDEvent fdEvent, FDDelegate<T, A, B, C> fdDelegate)
    {
        if (!ValidateRemove(fdEvent, fdDelegate)) return;
        fd_EventCenter[fdEvent][typeof(FDDelegate<T, A, B, C>)] = (FDDelegate<T, A, B, C>)fd_EventCenter[fdEvent][typeof(FDDelegate<T, A, B, C>)] - fdDelegate;
        AfterRemoved(fdEvent);
    }

    public static void Broadcast(FDEvent fdEvent)
    {
        Dictionary<Type, Delegate> eventCenter;
        if (fd_EventCenter.TryGetValue(fdEvent, out eventCenter))
        {
            Delegate fdDelegate;
            if (eventCenter.TryGetValue(typeof(FDDelegate), out fdDelegate))
            {
                ((FDDelegate)fdDelegate)();
            }
        }
    }

    public static void Broadcast<T>(FDEvent fdEvent, T t)
    {
        Broadcast(fdEvent);
        Dictionary<Type, Delegate> eventCenter;
        if (fd_EventCenter.TryGetValue(fdEvent, out eventCenter))
        {
            Delegate fdDelegate;
            if (eventCenter.TryGetValue(typeof(FDDelegate<T>), out fdDelegate))
            {
                ((FDDelegate<T>)fdDelegate)(t);
            }
        }
    }

    public static void Broadcast<T, A>(FDEvent fdEvent, T t, A a)
    {
        Broadcast<T>(fdEvent, t);
        Dictionary<Type, Delegate> eventCenter;
        if (fd_EventCenter.TryGetValue(fdEvent, out eventCenter))
        {
            Delegate fdDelegate;
            if (eventCenter.TryGetValue(typeof(FDDelegate<T, A>), out fdDelegate))
            {
                ((FDDelegate<T, A>)fdDelegate)(t, a);
            }
        }
    }

    public static void Broadcast<T, A, B>(FDEvent fdEvent, T t, A a, B b)
    {
        Broadcast<T, A>(fdEvent, t, a);
        Dictionary<Type, Delegate> eventCenter;
        if (fd_EventCenter.TryGetValue(fdEvent, out eventCenter))
        {
            Delegate fdDelegate;
            if (eventCenter.TryGetValue(typeof(FDDelegate<T, A, B>), out fdDelegate))
            {
                ((FDDelegate<T, A, B>)fdDelegate)(t, a, b);
            }
        }
    }

    public static void Broadcast<T, A, B, C>(FDEvent fdEvent, T t, A a, B b, C c)
    {
        Broadcast<T, A, B>(fdEvent, t, a, b);
        Dictionary<Type, Delegate> eventCenter;
        if (fd_EventCenter.TryGetValue(fdEvent, out eventCenter))
        {
            Delegate fdDelegate;
            if (eventCenter.TryGetValue(typeof(FDDelegate<T, A, B, C>), out fdDelegate))
            {
                ((FDDelegate<T, A, B, C>)fdDelegate)(t, a, b, c);
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
            fd_EventCenter.Add(fdEvent, new Dictionary<Type, Delegate>());
        }
        if(!fd_EventCenter[fdEvent].ContainsKey(fdDelegate.GetType()))
        {
            fd_EventCenter[fdEvent].Add(fdDelegate.GetType(), null);
        }
        return true;
    }

    private static Boolean ValidateRemove(FDEvent fdEvent, Delegate fdDelegate)
    {
        if (fd_EventCenter.ContainsKey(fdEvent))
        {
            if (fd_EventCenter[fdEvent] != null && fd_EventCenter[fdEvent].ContainsKey(fdDelegate.GetType())
                    && fd_EventCenter[fdEvent][fdDelegate.GetType()] != null)
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