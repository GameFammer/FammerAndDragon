using UnityEditor;
using System.IO;
public class CreateAssetBundles
{
    [MenuItem("Assets/Build AssetBundles")]
    static void BuildAllAssetBundles()
    {
        BuildPipeline.BuildAssetBundles(RDPlatform.DATA_PATH, BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows);
    }
}
