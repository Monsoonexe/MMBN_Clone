using UnityEngine;

public abstract class GatherTargetsBehavior : ScriptableObject
{
    public bool canTargetSelf = false;//can hurt self?
    public bool pierce = false;//hit more targets after first?

    /// <summary>
    /// Describe to the developer how the behavior works.
    /// </summary>
    [SerializeField]
    [TextArea]
    protected string developerDescription;
    
    public abstract NaviController_Battle[] GatherTargets(NaviController_Battle user);

}
