using UnityEngine;

[CreateAssetMenu(fileName = "New Basic Raycast Target", menuName = "Targeting Behaviors / Immediately in Front")]
public class ImmediatelyInFront : GatherTargetsBehavior
{

    public override NaviController_Battle[] GatherTargets(NaviController_Battle user)
    {
        NaviController_Battle[] targetList = new NaviController_Battle[1];//this type of attack can only ever hit one target

        //this attack looks at the panel matrix
        if (!panelArray)
        {
            FindPanelArray();
        }
        
        //where is the user?
        user.GetCurrentPanelCoordinates(out int attackerX, out int attackerY);

        //ask the panel array what object is right in front of this user's coordinates
        var occupantOnTargetPanel = panelArray.GetOccupantAtCoordinates(attackerX + user.GetOrientation(), attackerY);

        if (occupantOnTargetPanel)
        {//if something exists on this spot...
            targetList[0] = occupantOnTargetPanel.GetComponent<NaviController_Battle>() as NaviController_Battle;
        }
                    
        return targetList;
    }
}
