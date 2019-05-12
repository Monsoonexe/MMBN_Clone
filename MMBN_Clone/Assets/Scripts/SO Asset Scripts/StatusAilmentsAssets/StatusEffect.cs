[System.Serializable]
public struct StatusEffect {

    public string statusName;
    public StatusAilmentType type;
    public float duration;
    public float effectEndTime { get; }

    public StatusEffect(string statusName, StatusAilmentType type, float duration, float effectEndTime)
    {
        this.statusName = statusName;
        this.type = type;
        this.duration = duration;
        this.effectEndTime = effectEndTime;
    }
}
