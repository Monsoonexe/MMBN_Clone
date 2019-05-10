using UnityEngine;

[CreateAssetMenu(fileName = "New Basic Raycast Target", menuName = "Targeting Behaviors / RaycastTarget")]
public class FirstRaycast : GatherTargetsBehavior
{
    public override NaviController_Battle[] GatherTargets(NaviController_Battle user)
    {
        NaviController_Battle[] targetList = new NaviController_Battle[1];//this type of attack can only ever hit one target

        //get forward direction
        //get start position
        //fire ray
        //if hit something
        //return

        Transform userXform = user.transform;

        RaycastHit2D raycastHitInfo = Physics2D.Raycast(user.transform.position, user.transform.forward * user.GetOrientation(), PanelArray.globalScale);

        if (raycastHitInfo)
        {
            Debug.Log("ZAP! I shot and hit: " + raycastHitInfo.collider.name);
            targetList[0] = raycastHitInfo.collider.gameObject.GetComponent<NaviController_Battle>() as NaviController_Battle; 
        }

        else
        {
            Debug.Log("ZAP! I shot but missed.");
        }
        
        return targetList;
    }
}
