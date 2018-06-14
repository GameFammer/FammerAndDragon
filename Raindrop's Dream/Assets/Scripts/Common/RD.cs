using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RD
{
    public static bool IsOSXEditor = Application.platform == RuntimePlatform.OSXEditor;

    public static bool IsWindowsEditor = Application.platform == RuntimePlatform.WindowsEditor;

    //拼接路径
    public static string SplitPath(string[] paths){

        string split_character = IsOSXEditor ? "/" : "\\";

        string path = paths[0];

        for (var i = 1; i < paths.Length; i++){
            path += split_character + paths[i];
        }

        return path;
    }
}
