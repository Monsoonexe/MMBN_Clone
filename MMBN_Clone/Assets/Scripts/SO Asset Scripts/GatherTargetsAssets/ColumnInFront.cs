using UnityEngine;

/// <summary>
/// Hits a target column of targets (max 3) in the column immediately in front of user.
/// </summary>
[CreateAssetMenu(fileName = "ColumnInFront_Behavior", menuName = "Targeting Behaviors / ColumnInFront")]
public class ColumnInFront : GatherTargetsBehavior
{
    public override NaviController_Battle[] GatherTargets(NaviController_Battle user)
    {
        var targetList = new NaviController_Battle[3];//this type of attack

        user.GetCurrentPanelCoordinates(out int x, out int y);
        x += user.GetOrientation();//get one column in front of user

        targetList[0] = panelArray.GetOccupantAtCoordinates(x, y + 1);//one above
        targetList[1] = panelArray.GetOccupantAtCoordinates(x, y);//on same row
        targetList[2] = panelArray.GetOccupantAtCoordinates(x, y - 1);//one below

        return targetList;
    }
}
