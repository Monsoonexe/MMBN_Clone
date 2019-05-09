using UnityEngine;

[CreateAssetMenu(menuName = "GatherTargetsBehavior/", fileName = "GatherTargetsBehavior")]
public abstract class GatherTargetsBehavior : ScriptableObject
{
    public abstract NaviController_Battle[] GatherTargets();

}
