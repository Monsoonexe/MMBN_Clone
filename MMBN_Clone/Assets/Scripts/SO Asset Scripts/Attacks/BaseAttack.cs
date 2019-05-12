﻿using UnityEngine;

public abstract class BaseAttack : ScriptableObject{
    public string attackName;
    /// <summary>
    /// Time in seconds that one must wait between successive shots.
    /// </summary>
    [Header("---Stats---")]
    public float delay = 0.25f;
    /// <summary>
    /// How many seconds it takes to fully charge.
    /// </summary>
    public float chargeTime = 3;//multiplier
    public int damage = 1;
    public int chargeDamage = 3;

    [SerializeField]
    protected GatherTargetsBehavior targetingBehavior;
    
    [SerializeField]
    /// <summary>
    /// What trigger to send to the animator for ability
    /// </summary>
    protected string animatorMessage;

    //member functions
    /// <summary>
    /// Used for initialization
    /// </summary>
    protected abstract void Initialize();

    /// <summary>
    /// Do the things the attack does.
    /// </summary>
    /// <param name="naviController">The navi using the ability.</param>
    public abstract void TriggerAttack(NaviController_Battle naviController);  

}
