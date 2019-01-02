using System.ComponentModel;

[Description("FDEventType")]
public enum FDEvent
{
    [Description("arg1:Name, arg2:Age, arg3:Sex, arg4:Nation")]
    Example,
    [Description("arg1:backpack-id, arg2:item")]
    Refresh_Backpack
}