using UnityEngine;

[CreateAssetMenu(fileName = "NewProjectileAttack", menuName = "Abilities/Projectile Attack")]
public class ProjectileAttack : BaseAttack {

    public GameObject projectilePrefab;
    public int projectileSpeed;

    private GameObject projectileInstance;

    public ProjectileAttack()
    {
        this.Initialize();
    }

    protected override void Initialize()
    {
        //setup attack
        attackName = "Generic Projectile Attack";
    }

    public override void TriggerAttack(NaviController_Battle naviController, bool fullyCharged = false)
    {
        //look at everythign in your row
        //if found something,
        //do damage to it
        //play noise
        //show animation on thing hit

    }

}
