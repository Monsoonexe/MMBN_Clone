using UnityEngine;

public abstract class GatherTargetsBehavior : ScriptableObject
{
    /// <summary>
    /// for getting targets coordinates
    /// </summary>
    protected static PanelArray panelArray;

    /// <summary>
    /// Describe to the developer how the behavior works.
    /// </summary>
    [SerializeField]
    [TextArea]
    protected string developerDescription;
    
    public abstract NaviController_Battle[] GatherTargets(NaviController_Battle user);

    /// <summary>
    /// Get a list of the panels that this behavior is targeting (the panels on which the targets are standing)
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    public abstract Panel[] GatherTargetPanels(NaviController_Battle user);

    /// <summary>
    /// Fills static references for this class needed by all instances.
    /// </summary>
    public static void InitStaticReferences()
    {
        FindPanelArray();
    }

    /// <summary>
    /// Get a handle on the panel array script for easy access by all instances.
    /// </summary>
    protected static void FindPanelArray()
    {
        if (!panelArray)
        {
            panelArray = GameObject.FindGameObjectWithTag("PanelArray").GetComponent<PanelArray>() as PanelArray;
        }
    }

}
