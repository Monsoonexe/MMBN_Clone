using UnityEngine;

[CreateAssetMenu(fileName = "NewAttack", menuName = "Abilities/BasicAttack")]
public class BaseAttack : ScriptableObject{
    [SerializeField]
    protected string attackName;

    /// <summary>
    /// Time in seconds that one must wait between successive shots.
    /// </summary>
    [Header("---Stats---")]
    public Element element;
    public float delay = 0.25f;

    [Range(0.1f, 100)]
    /// <summary>
    /// How many seconds it takes to fully charge.  != 0
    /// </summary>
    public float chargeTime = 3;//multiplier

    public int damage = 1;

    [SerializeField]
    protected GatherTargetsBehavior targetingBehavior;

    [SerializeField]
    protected StatusEffect statusEffect;
    
    [SerializeField]
    /// <summary>
    /// What trigger to send to the animator for ability
    /// </summary>
    private string animatorMessage;
        
    /// <summary>
    /// Do the things the attack does.
    /// </summary>
    /// <param name="naviController">The navi using the ability.</param>
    public virtual void TriggerAttack(NaviController_Battle naviController)
    {
        //get targets using targeting behavior
        var targets = targetingBehavior.GatherTargets(naviController);

        foreach (var target in targets)
        {
            if (target) target.TakeDamage(damage, element);
            //TODO give status ailment if you have one
        }
    }

    public string GetAnimatorMessage()
    {
        return animatorMessage;
    }
    
}
