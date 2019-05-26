using UnityEngine;

[System.Serializable]
public class NaviAsset : ScriptableObject {
    public string naviName;
    public bool READY = false;

    [Header("Visual Stuff")]
    public Sprite emotionWindow;
    public Sprite characterSelectionPortrait;
    public AnimatorOverrideController animatorOverrideController;
    public Vector3 spriteOffset;
    public int orientation;//+1 for facing right -1 for facing left

    [Header("Sound")]
    public AudioClip chargeShotAudioClip;

    [Header("Battle Stats")]
    public float naviMoveSpeed = 1.0f;
    public int maxHealth = 9999;
    public Element element = Element.NONE;

    [Header("Attack SO")]
    public BaseAttack busterAttack;
    public BaseAttack chargedBusterAttack;
    public BaseAttack swordAttack;
    public BaseAttack chargedSwordAttack;
    public BaseAttack throwAttack;
    public BaseAttack specialAttack;

}
