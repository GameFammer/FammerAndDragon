using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using UnityEngine;

public class RDLog : MonoBehaviour
{
    public int showLogNum;//显示日志条数
    private List<string> logList;
    void Awake()
    {
        logList = new List<string>();
        Application.logMessageReceived += HandleLog;
    }
    void OnDisable()
    {
        if (logList.Count != 0)
        {
        }
        Application.logMessageReceived -= HandleLog;
    }
    void HandleLog(string logString, string stackTrace, LogType type)
    {
        logList.Add(logString);
    }
    public static void Log(string _log, LogType _type = LogType.Log)
    {
        string fullLog = DateTime.Now + " [" + _type.ToString() + "] " + _log;
        switch (_type)
        {
            case LogType.Log: Debug.Log(fullLog); break;
            case LogType.Assert: Debug.LogAssertion(fullLog); break;
            case LogType.Error: Debug.LogError(fullLog); break;
            case LogType.Warning: Debug.LogWarning(fullLog); break;
            default: Debug.Log(fullLog); break;
        }
    }
    public static void Log(Exception e)
    {
        Debug.LogException(e);
    }
}
