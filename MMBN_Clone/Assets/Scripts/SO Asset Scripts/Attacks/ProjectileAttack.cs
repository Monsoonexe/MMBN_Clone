using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "NewProjectileAttack", menuName = "Abilities/Projectile Attack")]
public class ProjectileAttack : BaseAttack
{

    public GameObject projectilePrefab;
    public int projectileSpeed;

    private GameObject projectileInstance;
    
    public override IEnumerator TriggerAttack(NaviController_Battle naviController)
    {
        yield return new WaitForSeconds(0);
        //look at everythign in your row
        //if found something,
        //do damage to it
        //play noise
        //show animation on thing hit

    }

}
