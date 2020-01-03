using UnityEngine;

[CreateAssetMenu(fileName = "ImmediatelyInFront_Behavior", menuName = "Targeting Behaviors / Immediately in Front")]
public class ImmediatelyInFront : GatherTargetsBehavior
{
    [SerializeField]
    protected int squaresAhead = 1;

    public override Panel[] GatherTargetPanels(NaviController_Battle user)
    {
        var panelList = new Panel[1];//this type of attack only hits one target
        //where is the user?
        user.GetCurrentPanelCoordinates(out int attackerX, out int attackerY);

        //ask the panel array what object is right in front of this user's coordinates
        panelList[0] = panelArray.GetPanel(attackerX + user.TargetingOrientation * squaresAhead, attackerY);

        return panelList;
    }

    public override NaviController_Battle[] GatherTargets(NaviController_Battle user)
    {
        var targetList = new NaviController_Battle[1];//this type of attack can only ever hit one target
                
        //where is the user?
        user.GetCurrentPanelCoordinates(out int attackerX, out int attackerY);

        //ask the panel array what object is right in front of this user's coordinates
        var occupantOnTargetPanel = panelArray.GetOccupantAtCoordinates(attackerX + user.TargetingOrientation * squaresAhead, attackerY);

        if (occupantOnTargetPanel)
        {//if something exists on this spot...
            targetList[0] = occupantOnTargetPanel.GetComponent<NaviController_Battle>() as NaviController_Battle;
        }
                    
        return targetList;
    }
}
