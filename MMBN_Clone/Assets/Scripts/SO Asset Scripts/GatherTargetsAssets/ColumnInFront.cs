using UnityEngine;

/// <summary>
/// Hits a target column of targets (max 3) in the column immediately in front of user.
/// </summary>
[CreateAssetMenu(fileName = "ColumnInFront_Behavior", menuName = "Targeting Behaviors / ColumnInFront")]
public class ColumnInFront : GatherTargetsBehavior
{
    private static readonly int maxTargets = 3;
    /// <summary>
    /// How many columns in front. 1 is immediately in front
    /// </summary>
    private static readonly int spacesInFront = 1;

    /// <summary>
    /// Get the panels this can target.
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    public override Panel[] GatherTargetPanels(NaviController_Battle user)
    {
        var targetList = new Panel[maxTargets];//this type of attack

        user.GetCurrentPanelCoordinates(out int x, out int y);
        x += user.TargetingOrientation * spacesInFront;//get one column in front of user

        targetList[0] = panelArray.GetPanel(x, y + 1);//one above
        targetList[1] = panelArray.GetPanel(x, y);//on same row
        targetList[2] = panelArray.GetPanel(x, y - 1);//one below

        return targetList;
    }

    public override NaviController_Battle[] GatherTargets(NaviController_Battle user)
    {
        var targetList = new NaviController_Battle[maxTargets];//this type of attack

        user.GetCurrentPanelCoordinates(out int x, out int y);
        x += user.TargetingOrientation * spacesInFront;//get one column in front of user

        targetList[0] = panelArray.GetOccupantAtCoordinates(x, y + 1);//one above
        targetList[1] = panelArray.GetOccupantAtCoordinates(x, y);//on same row
        targetList[2] = panelArray.GetOccupantAtCoordinates(x, y - 1);//one below

        return targetList;
    }
}
