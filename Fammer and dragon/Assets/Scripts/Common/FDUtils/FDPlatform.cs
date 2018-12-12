using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FDPlatform
{
    public static string DATA_PATH
    {
        get
        {
            return Application.streamingAssetsPath;
        }
    }

    public static string LOG_PATH
    {
        get 
        {
            return Application.streamingAssetsPath;                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                
        }
    }
    public static bool isOSXEditor
    {
        get
        {
            return Application.platform == RuntimePlatform.OSXEditor;
        }
    }

    public static bool isWindowsEditor
    {
        get
        {
            return Application.platform == RuntimePlatform.WindowsEditor;
        }
    }

    //拼接路径
    public static string SplitPath(string[] strs){

        string splitCharacter = isOSXEditor ? "/" : "\\";

        return String.Join(splitCharacter, strs);
    }
}
