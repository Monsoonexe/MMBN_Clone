using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "NewAttack", menuName = "Abilities/BasicAttack")]
public class BaseAttack : ScriptableObject
{ 
    [SerializeField]
    protected string attackName;

    [SerializeField]
    /// <summary>
    /// What trigger to send to the animator for ability animation
    /// </summary>
    private string animatorMessage;

    /// <summary>
    /// The elemental type of the damage that is dealt by this attack.
    /// </summary>
    [Header("---Stats---")]
    public Element element;

    /// <summary>
    /// Once the weapon is fired, this amount of time must pass before the actor is allowed to act.
    /// </summary>
    public float delay = 0.25f;

    /// <summary>
    /// The amount of time it takes to raise the weapon and fire it.
    /// </summary>
    public float drawDelay = .05f;

    [Range(0.1f, 100)]
    /// <summary>
    /// How many seconds it takes to fully charge.  != 0
    /// </summary>
    public float chargeTime = 3;//multiplier

    /// <summary>
    /// The amount of damage dealt per hit of this attack
    /// </summary>
    public int damage = 1;

    /// <summary>
    /// The behavior that determines how and which targets are to be dealt damage.
    /// </summary>
    [SerializeField]
    protected GatherTargetsBehavior targetingBehavior;

    /// <summary>
    /// If any, an extra effect that is dealt if the attack hits or given to the user.
    /// </summary>
    [SerializeField]
    protected StatusEffect statusEffect;

    /// <summary>
    /// Do the things the attack does.
    /// </summary>
    /// <param name="naviController">The navi using the ability.</param>
    public virtual IEnumerator TriggerAttack(NaviController_Battle naviController)
    {
        yield return new WaitForSeconds(drawDelay);

        //get targets using targeting behavior
        var targets = targetingBehavior.GatherTargets(naviController);

        //handle damage
        foreach (var target in targets)
        {
            if (target)
            {
                target.TakeDamage(damage, element);
                //TODO give status ailment if you have one
            }
        }
    }

    public string GetAnimatorMessage()
    {
        return animatorMessage;
    }
    
}
