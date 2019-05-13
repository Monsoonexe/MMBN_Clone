using UnityEngine;

public abstract class GatherTargetsBehavior : ScriptableObject
{
    //for getting targets coordinates
    protected static PanelArray panelArray;

    /// <summary>
    /// Describe to the developer how the behavior works.
    /// </summary>
    [SerializeField]
    [TextArea]
    protected string developerDescription;
    
    public abstract NaviController_Battle[] GatherTargets(NaviController_Battle user);

    protected static void FindPanelArray()
    {
        if (!panelArray)
        {
            panelArray = GameObject.FindGameObjectWithTag("PanelArray").GetComponent<PanelArray>() as PanelArray;
        }
    }

}
