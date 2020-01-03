using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerInfoController : MonoBehaviour
{
    [Header("---Scene Refs---")]
    [SerializeField]
    private NaviController_Battle targetNavi;

    [SerializeField]
    private TextMeshProUGUI healthTextTMP;

    [SerializeField]
    private Image healthBackImage;

    [SerializeField]
    private Image characterPortrait;

    [Header("---Source Data---")]
    [SerializeField]
    private HealthColors healthColorsAsset;

    [SerializeField]
    private bool healthFlashing = false;

    // Start is called before the first frame update
    void Start()
    {
        SubscribeToEvents();
        UpdateVisuals();
    }
    
    private void SubscribeToEvents()
    {
        targetNavi.takeDamage.AddListener(UpdateVisuals);
        targetNavi.die.AddListener(UpdateVisuals);
        targetNavi.recoverHealth.AddListener(UpdateVisuals);
    }

    public void UpdateVisuals()
    {
        if (!healthTextTMP)
        {
            Debug.LogError("ERROR! No Health Text!");
        }

        if (!targetNavi)
        {
            Debug.Log("ERROR! No Navi Asset!");
        }

        healthTextTMP.text = targetNavi.CurrentHealth.ToString();
        healthColorsAsset.SetHealthColor(targetNavi.CurrentHealth, targetNavi.MaxHealth, healthTextTMP);
    }

}
