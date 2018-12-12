﻿using UnityEditor;
using System.IO;
public class CreateAssetBundles
{
    [MenuItem("Assets/Build AssetBundles")]
    static void BuildAllAssetBundles()
    {
        BuildPipeline.BuildAssetBundles(FDPlatform.DATA_PATH+ "/AssestBundles", BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows);
    }
}
