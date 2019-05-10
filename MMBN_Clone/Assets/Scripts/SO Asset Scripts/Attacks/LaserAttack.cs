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
        attackType = AttackType.LASER;
        basePower = 10; //0-255 i guess
        baseCritRate = 1; //0-100
    }

    public override void TriggerAttack(NaviController_Battle naviController)
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
