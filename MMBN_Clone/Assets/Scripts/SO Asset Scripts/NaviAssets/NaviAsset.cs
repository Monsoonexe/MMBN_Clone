using UnityEngine;
using UnityEngine.Audio;

[System.Serializable]
public class NaviAsset : ScriptableObject {
    public string naviName;
    public bool READY = false;

    [Header("Visual Stuff")]
    public Sprite emotionWindow;
    public Sprite characterSelectionPortrait;
    public RuntimeAnimatorController runtimeAnimController;
    public Vector3 spriteOffset;
    public int orientation;//+1 for facing right -1 for facing left

    [Header("Sound")]
    public AudioClip chargeShotAudioClip;

    [Header("Battle Stats")]
    public float naviMoveSpeed = 1.0f;
    public int maxHealth = 9999;
    public Element element = Element.NONE;

    [Header("Buster Stats")]
    public float busterDelay = 0.25f;
    public float busterChargeRate = 1.0f;//multiplier
    public float busterChargeMax = 3.0f;//max power needed before charge shot
    public float busterCooldownRate = 1.0f;
    public float busterDamageMod = 1.0f;
    public float busterChargeDamageMod = 1.0f;
    public float busterDefenseMod = 1.0f;

    [Header("Sword Stats")]
    public float swordDelay = 0.45f;
    public float swordChargeRate = 0.5f;
    public float swordChargeMax = 3.0f;
    public float swordCooldownRate = 1.0f;
    public float swordDamageMod = 1.0f;
    public float swordChargeDamageMod = 1.0f;
    public float swordDefenseMod = 1.0f;

    [Header("Attacks")]
    public BaseAttack busterAttack;
    public BaseAttack chargedBusterAttack;
    public BaseAttack swordAttack;
    public BaseAttack chargedSwordAttack;
    public BaseAttack throwAttack;
    public BaseAttack specialAttack;

}
