using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileAttack : BaseAttack {

    public ProjectileAttack()
    {
        this.Initialize();
    }

    public override void Initialize()
    {
        //setup attack
        attackName = "Generic Projectile Attack";
        attackType = AttackType.PROJECTILE;
        basePower = 10; //0-255 i guess
        baseCritRate = 1; //0-100
        magnitude = 0.5f;
    }

    public override void TriggerAttack(NaviController_Battle naviController)
    {
        //look at everythign in your row
        //if found something,
        //do damage to it
        //play noise
        //show animation on thing hit


    }

}
