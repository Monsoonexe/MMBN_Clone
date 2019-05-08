using UnityEngine;
using UnityEditor;

static class NaviAssetUnityIntegration
{

    [MenuItem("Assets/Create/NaviAsset")]
    public static void CreateYourScriptableObject()
    {
        ScriptableObjectUtility2.CreateAsset<NaviAsset>();
    }

}
