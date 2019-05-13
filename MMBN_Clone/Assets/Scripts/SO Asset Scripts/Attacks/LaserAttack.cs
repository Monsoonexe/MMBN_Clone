using UnityEngine;

[CreateAssetMenu(fileName = "NewLaserAttack", menuName = "Abilities/Laser Attack")]
public class LaserAttack : BaseAttack {

    public LaserAttack()
    {
        this.Initialize();
    }

    protected override void Initialize()
    {
        //setup attack
        attackName = "Generic Laser Attack";
    }

    public override void TriggerAttack(NaviController_Battle naviController, bool fullyCharged = false)
    {
        //get targets using targeting behavior
        var targets = targetingBehavior.GatherTargets(naviController);
        
        if (fullyCharged)
        {
            //Debug.Log("Firing " + this.attackName + "... fully charged!");

        }
        else
        {

            //Debug.Log("Firing " + this.attackName);
        }


        foreach (var target in targets)
        {
            if(target) target.TakeDamage(fullyCharged ? chargeDamage : damage, element);
            //TODO give status ailment if you have one
        }
    }
}
