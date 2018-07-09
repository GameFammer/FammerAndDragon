using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class CreatPrefabAndAssestBundle : MonoBehaviour {

    [MenuItem("Assets/Creat Prefab And Build AssetBundles")]
    // Use this for initialization
    static void CreatPrefabAndBuildAssetBundles()
    {
        List<AssetBundleBuild> assestBuildMap=new List<AssetBundleBuild>();//AssetBundel Map
        List<string> assestPath = new List<string>();//Assest路径+文件名
        GameObject root = GameObject.Find("Root");
        foreach(Transform  levelTheme in root.transform)
        {
            //清空上一场景AssestPath
            assestPath.Clear();
            AssetBundleBuild assetBundle = new AssetBundleBuild();
            //以关卡类型命名
            assetBundle.assetBundleName = levelTheme.name;

            if (levelTheme.gameObject.activeSelf)
            {
                //路径：Assets/Prefabs/Scene/关卡类型名/prefab名.prefab
                string prefabPath = "Assets/Prefabs/Scene/" + levelTheme.name+"/";
                if (!Directory.Exists(prefabPath))
                {
                    Directory.CreateDirectory(prefabPath);
                }
                foreach (Transform tile in levelTheme)
                {
                    if (tile.gameObject.activeSelf)
                    {
                        string prefabFullPath = prefabPath + tile.name + ".prefab";
                        //创建prefab
                        Object prefab = PrefabUtility.CreatePrefab(prefabFullPath, tile.gameObject);
                        PrefabUtility.ReplacePrefab(tile.gameObject, prefab, ReplacePrefabOptions.ConnectToPrefab);
                        //增加到assestPath
                        assestPath.Add(prefabFullPath);
                    }
                }

                assetBundle.assetNames = assestPath.ToArray();
                if(assetBundle.assetNames.Length>0)
                {
                    assestBuildMap.Add(assetBundle);
                }
            }
            
        }            //AssestBundle路径:StreamingAssets/AssestBundles
        BuildPipeline.BuildAssetBundles(Application.streamingAssetsPath + "/AssestBundles", assestBuildMap.ToArray(), BuildAssetBundleOptions.None, RDPlatform.isOSXEditor ? BuildTarget.StandaloneOSX : BuildTarget.StandaloneWindows);

       

    }
}
