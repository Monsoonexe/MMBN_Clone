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

        foreach(var target in targets)
        {
            //target.DealDamage();
        }

        Debug.Log("LAZERS!!! PEW PEW BANG!");
    }
}
