using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Obsolete("This should be an instance of a scriptable object, not its own class.  Refactoring...")]
public class Attack_Buster : BaseAttack
{

    public Attack_Buster()
    {
        this.Initialize();
    }

    protected override void Initialize()
    {
        //setup attack
        attackName = "Basic Buster Attack";
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


    }
}
