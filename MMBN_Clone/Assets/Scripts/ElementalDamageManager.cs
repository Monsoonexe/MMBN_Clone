using UnityEngine;

public static class ElementalDamageManager
{
    public static float DetermineDamageModifier(Element attackerElement, Element receiverElement)
    {
        var damageModifier = 1.0f;
        //TODO make damage chart / matrix

        Debug.LogWarning("Elemental damage not yet defined.  Modifier is always 1.0");

        return damageModifier;
    }
}
