using System;
using System.Collections.Generic;
namespace RDUI
{
    public enum UIMessageType
    {
        Updata_HealthUI
    }
    public delegate void UIMessageDelegate(object _args);

    public class UIDelegateBind
    {
        public static Dictionary<UIMessageType, UIMessageDelegate> messageDelegates = new Dictionary<UIMessageType, UIMessageDelegate>();
        public static void NotifyUI(UIMessageType _messageType, int _value)
        {
            if (messageDelegates.ContainsKey(_messageType))
            {
                if (messageDelegates[_messageType] != null)
                {
                    messageDelegates[_messageType](_value);
                }
            }
        }
        public void AddListener(UIMessageType _messageType, UIMessageDelegate _handler)
        {
            if (!messageDelegates.ContainsKey(_messageType))
            {
                messageDelegates.Add(_messageType, null);
            }
            messageDelegates[_messageType] += _handler;
        }
        public void RemoveListener(UIMessageType _messageType, UIMessageDelegate _handler)
        {
            if (messageDelegates.ContainsKey(_messageType))
            {
                messageDelegates[_messageType] -= _handler;
            }
        }
        public void RemoveAllListener()
        {
            messageDelegates.Clear();
        }
    }

}
