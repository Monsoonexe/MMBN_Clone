using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "StepSword", menuName = "Abilities/StepSword")]
public class StepSword : BaseAttack
{
    [Header("StepSword")]
    [SerializeField]
    protected GatherTargetsBehavior stepTargetingBehavior;

    [SerializeField]
    protected float attackWaitTime = 1.5f;

    public override IEnumerator TriggerAttack(NaviController_Battle naviController)
    {        
        //immediately check to see if you can step
        var targetPanels = stepTargetingBehavior.GatherTargetPanels(naviController);
        if (!targetPanels[0]) yield break;//if there's nothing, do nothing

        var currentPanel = naviController.GetCurrentPanel();//cache current panel to return to it after attack
        //step, can even go into enemy territory
        naviController.MoveNavi(targetPanels[0], true);//can only move to single, first panel

        //wait to draw
        yield return new WaitForSeconds(drawDelay);

        //get targets using targeting behavior (ie wide sword, short sword, long sword)
        var targets = targetingBehavior.GatherTargets(naviController);

        //handle damage
        foreach (var target in targets)
        {
            if (target)
            {
                target.TakeDamage(damage, element);
                //TODO give status ailment if you have one
            }
        }

        yield return new WaitForSeconds(attackWaitTime);
        //step back no matter what
        naviController.MoveNavi(currentPanel);//can only move to single, first panel
    }
}
