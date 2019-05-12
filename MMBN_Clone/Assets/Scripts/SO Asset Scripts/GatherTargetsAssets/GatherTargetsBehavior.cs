using UnityEngine;

public abstract class GatherTargetsBehavior : ScriptableObject
{
    public bool canTargetSelf = false;//can hurt self?
    public bool pierce = false;//hit more targets after first?
    [Range(0, 4)]
    public int pushback = 0;// 0 - none, 1 - push back 1 tile, 2 - back 2 tiles, 

    /// <summary>
    /// Describe to the developer how the behavior works.
    /// </summary>
    [SerializeField]
    protected string developerDescription;
    
    public abstract NaviController_Battle[] GatherTargets(NaviController_Battle user);

}
