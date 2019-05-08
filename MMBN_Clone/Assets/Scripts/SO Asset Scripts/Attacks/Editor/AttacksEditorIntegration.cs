
using UnityEditor;
static class AttacksEditorIntegration
{

    [MenuItem("Assets/Create/Attack/Laser")]
    public static void CreateLaserAttackSO()
    {
        ScriptableObjectUtility2.CreateAsset<LaserAttack>();
    }

    [MenuItem("Assets/Create/Attack/Projectile")]
    public static void CreateProjectileAttackSO()
    {
        ScriptableObjectUtility2.CreateAsset<ProjectileAttack>();
    }

    [MenuItem("Assets/Create/Attack/Attack_Buster")]
    public static void CreateAttack_BusterAttackSO()
    {
        ScriptableObjectUtility2.CreateAsset<Attack_Buster>();
    }
}
