using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CustomGaugeManager : MonoBehaviour {
    private int customGaugeStandardFillSpeed = 1;

    public Slider customGauge;
    public BattleManager battlemanager;
    public int customGaugeSecondsToFill = 8; //default is 8
    public int customGaugeAdjustedFillSpeed = 1;

	// Use this for initialization
	void Start () {
        if (customGauge == null)
        {
            Debug.Log("ERROR: YO! Fix the reference to the customGauge Slider!");//
        }
        customGauge.value = 0; //custom gauge starts at 0
		
	}

    void Update()
    {
        
    }//end Update()

    public void IncrementCustomGauge()
    {
        if (customGauge.value < customGauge.maxValue)
        {
            float customGaugeIncrementAmount = (customGaugeStandardFillSpeed) * Time.deltaTime * (customGauge.maxValue / customGaugeSecondsToFill);
            customGauge.value += customGaugeIncrementAmount;
            //Debug.Log("CustomGauge incremented by: " + customGaugeIncrementAmount.ToString());
        }
        
    }//end FIncrementCustomGauge()
}
