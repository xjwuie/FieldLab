using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class test{

    [MenuItem("Tools/BulidAssetbundle")]
    static void Test1() {
        string outPath = Path.Combine(Application.dataPath, "StreamingAssets");
        if (Directory.Exists(outPath))
        {
            Directory.Delete(outPath, true);
            //Directory.CreateDirectory(outPath);
        }
        Directory.CreateDirectory(outPath);

        BuildPipeline.BuildAssetBundles(outPath, BuildAssetBundleOptions.ChunkBasedCompression, BuildTarget.StandaloneWindows64);
        AssetDatabase.Refresh();
    }

}
