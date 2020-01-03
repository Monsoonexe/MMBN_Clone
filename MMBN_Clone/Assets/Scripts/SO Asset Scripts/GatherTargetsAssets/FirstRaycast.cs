using UnityEngine;

[CreateAssetMenu(fileName = "FirstRaycast_Behavior", menuName = "Targeting Behaviors / RaycastTarget")]
public class FirstRaycast : GatherTargetsBehavior
{
    /// <summary>
    /// MAX TARGETS 1
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    public override Panel[] GatherTargetPanels(NaviController_Battle user)
    {
        var targets = GatherTargets(user);
        var targetPanels = new Panel[1];//this type of attack only ever hits one target

        targetPanels[0] = targets[0].GetCurrentPanel();
        
        return targetPanels;
    }

    /// <summary>
    /// MAX TARGETS 1
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    public override NaviController_Battle[] GatherTargets(NaviController_Battle user)
    {
        var targetList = new NaviController_Battle[1];//this type of attack can only ever hit one target
        
        var userXform = user.transform;

        var raycastHitInfo = Physics2D.Raycast(user.transform.position, user.transform.right * user.TargetingOrientation, PanelArray.globalScale);

        if (raycastHitInfo)
        {
            //Debug.Log("ZAP! I shot and hit: " + raycastHitInfo.collider.name);
            targetList[0] = raycastHitInfo.collider.gameObject.GetComponent<NaviController_Battle>() as NaviController_Battle; 
        }

        else
        {
            //Debug.Log("ZAP! I shot but missed.");
        }
        
        return targetList;
    }

}
