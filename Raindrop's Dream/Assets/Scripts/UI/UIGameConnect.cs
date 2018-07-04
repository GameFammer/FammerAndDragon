using System;
using System.Collections.Generic;
namespace RDUI
{
    public enum UIMessageType
    {
        Updata_HealthUI
    }
    public delegate void UIMessageDelegate(string[] _args);
    public class UIGameConnect
    {
        public static Dictionary<UIMessageType, UIMessageDelegate> messageDelegates = new Dictionary<UIMessageType, UIMessageDelegate>();
        public static void NotifyUI(UIMessageType _messageType,int _value) 
        {
            if (messageDelegates[_messageType] != null)
            {
                messageDelegates[_messageType](new string[] { _value.ToString() });
            }      
        }
        private static void AddListener(UIMessageType _messageType, UIMessageDelegate _handler)
        {
            if(!messageDelegates.ContainsKey(_messageType))
            {
                messageDelegates.Add(_messageType, null);
            }
            messageDelegates[_messageType] += _handler;
        }
        public static void RemoveListener(UIMessageType _messageType, UIMessageDelegate _handler)
        {
            if(messageDelegates.ContainsKey(_messageType))
            {
                messageDelegates[_messageType] -= _handler;
            }
        }
        public static void RemoveAllListener()
        {
            messageDelegates.Clear();
        }
    }

}
