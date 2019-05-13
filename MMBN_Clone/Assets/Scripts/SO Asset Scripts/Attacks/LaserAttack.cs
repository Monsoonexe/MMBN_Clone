using UnityEngine;

[CreateAssetMenu(fileName = "NewLaserAttack", menuName = "Abilities/Laser Attack")]
public class LaserAttack : BaseAttack {
    
    public override void TriggerAttack(NaviController_Battle naviController)
    {
        //get targets using targeting behavior
        var targets = targetingBehavior.GatherTargets(naviController);
        
        foreach (var target in targets)
        {
            if(target) target.TakeDamage(damage, element);
            //TODO give status ailment if you have one
        }
    }
}
