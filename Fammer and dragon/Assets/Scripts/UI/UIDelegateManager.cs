using System;
using System.Collections.Generic;
namespace FDUI
{
    public enum UIMessageType
    {
        Updata_Hp,
        Updata_HpMax
    }
    public delegate void UIMessageDelegate<T>(T _args);

    public static class UIDelegateManager
    {
        private static Dictionary<UIMessageType, Delegate> messageDelegates = new Dictionary<UIMessageType, Delegate>();
        public static void NotifyUI<T>(UIMessageType _messageType, T _value)
        {
            if (messageDelegates.ContainsKey(_messageType))
            {
                if (messageDelegates[_messageType] != null)
                {
                    ((UIMessageDelegate<T>)messageDelegates[_messageType])(_value);
                }
            }
        }
        public static void AddObserver<T>(UIMessageType _messageType, UIMessageDelegate<T> _handler)
        {
            if (!messageDelegates.ContainsKey(_messageType))
            {
                messageDelegates.Add(_messageType, null);
            }
            messageDelegates[_messageType] = (UIMessageDelegate<T>)messageDelegates[_messageType] + _handler;
        }
        public static void RemoveObserver<T>(UIMessageType _messageType, UIMessageDelegate<T> _handler)
        {
            if (messageDelegates.ContainsKey(_messageType))
            {
                messageDelegates[_messageType] = (UIMessageDelegate<T>)messageDelegates[_messageType] - _handler;
            }
        }
        public static void RemoveAllObserver()
        {
            messageDelegates.Clear();
        }
    }

}
