﻿using UnityEngine;

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
        //gimme every panel in my row
        //of the panels in front of me,
        //do any of these panels have any occupants? (that aren't me)
        //Fuck that occupant up!



        //look at everythign in your row
        //if found something,
        //do damage to it
        //play noise
        //show animation on thing hit

        Debug.Log("LAZERS!!! PEW PEW BANG!");
    }
}
