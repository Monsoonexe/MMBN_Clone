using UnityEngine;

public abstract class BaseAttack : ScriptableObject{
    public string attackName;
    public AttackType attackType;
    public int basePower; //0-255 i guess
    public int baseCritRate; //0-100

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
