using UnityEngine;

public abstract class GatherTargetsBehavior : ScriptableObject
{
    public abstract NaviController_Battle[] GatherTargets();

}
