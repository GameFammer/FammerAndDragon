using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RDPlatform
{
    public static string DATA_PATH
    {
        get
        {
            return RDPlatform.isOSXEditor ? Application.persistentDataPath : Application.streamingAssetsPath;
        }
    }
    public static bool isOSXEditor = Application.platform == RuntimePlatform.OSXEditor;

    public static bool isWindowsEditor = Application.platform == RuntimePlatform.WindowsEditor;

    //拼接路径
    public static string SplitPath(string[] paths){

        string splitCharacter = isOSXEditor ? "/" : "\\";

        string path = paths[0];

        for (var i = 1; i < paths.Length; i++){
            path += splitCharacter + paths[i];
        }

        return path;
    }
}
