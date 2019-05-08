using UnityEngine;
using UnityEditor;

public class StatusEffectAssetUnityIntegration{

[MenuItem("Assets/Create/StatusAilment")]
    public static void CreateScriptableObjectAsset()
    {
        ScriptableObjectUtility2.CreateAsset<StatusEffect>();
    }
}
