using UnityEngine;

public abstract class BaseAttack : ScriptableObject{
    public string attackName;
    /// <summary>
    /// Time in seconds that one must wait between successive shots.
    /// </summary>
    [Header("---Stats---")]
    public float delay = 0.25f;
    [Range(0.1f, 100)]
    /// <summary>
    /// How many seconds it takes to fully charge.  != 0
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
    private string animatorMessage;
    
    [SerializeField]
    /// <summary>
    /// What trigger to send to the animator for ability
    /// </summary>
    private string animatorMessage_charged;

    //member functions
    /// <summary>
    /// Used for initialization
    /// </summary>
    protected abstract void Initialize();

    /// <summary>
    /// Do the things the attack does.
    /// </summary>
    /// <param name="naviController">The navi using the ability.</param>
    /// <param name="fullyCharged">Whether or not the attack has been charged all the way.</param>
    public abstract void TriggerAttack(NaviController_Battle naviController, bool fullyCharged = false);  

    public string GetAnimatorMessage(bool charged = false)
    {
        return charged ? animatorMessage_charged : animatorMessage;
    }

}
