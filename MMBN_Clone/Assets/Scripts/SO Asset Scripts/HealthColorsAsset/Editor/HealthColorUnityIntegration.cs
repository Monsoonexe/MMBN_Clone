using UnityEngine;
using UnityEditor;

static class HealthColorUnityIntegration
{

    [MenuItem("Assets/Create/HealthColors")]
    public static void CreateScriptableObject()
    {
        ScriptableObjectUtility2.CreateAsset<HealthColors>();
    }

}
