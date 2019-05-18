using UnityEngine;
using TMPro;

public class HealthColors : ScriptableObject {
    [Header("Colors")]
    public Color color_HealthHigh;
    public Color color_HealthHalf;
    public Color color_HealthLow;
    public Color color_HealthDanger;

    [Header("Limits")]
    public float limit_HealthHalf = .50f;
    public float limit_HealthLow = .25f;
    public float limit_HealthDanger = .10f;

    public void SetHealthColor(float currentHealth, float maxHealth, TextMeshProUGUI healthText)
    {
        var healthPercent = currentHealth / maxHealth; //get percent
        //Debug.Log("healthPercent: " + healthPercent.ToString());//print test
        if (healthPercent <= limit_HealthDanger)
        {
            healthText.color = color_HealthDanger;
        }
        else if (healthPercent < limit_HealthLow)
        {
            healthText.color = color_HealthLow;
        }
        else if (healthPercent < limit_HealthHalf)
        {
            healthText.color = color_HealthHalf;
        }
        else
        {
            healthText.color = color_HealthHigh;
            healthText.text = currentHealth.ToString();
            //TODO get a color between white and yellow this percent
        }
    }
}
