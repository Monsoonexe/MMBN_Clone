using UnityEngine;

public abstract class BaseAttack : ScriptableObject{
    public string attackName;
    public AttackType attackType;
    public int basePower; //0-255 i guess
    public int baseCritRate; //0-100
    public float magnitude; //magnitude of over 1 will stagger, 2 will knockback

    //member functions
    protected abstract void Initialize();
    public abstract void TriggerAttack(NaviController_Battle naviController);  

}
