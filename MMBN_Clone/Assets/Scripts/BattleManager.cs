using UnityEngine;
using UnityEngine.UI;

public class BattleManager : MonoBehaviour {
    private bool atCustomScreen = false;

    public CustomGaugeManager customGaugeManager;
    public Image battleBackground;
    
    public BattleType battleType = BattleType.DUEL;
    public GameObject[] combatants;//things that fight -- positions get set
    public GameObject[] boardObjects;// other pieces on the board -- obstacles
    public PanelArray panelArray;

    private PlayerManager playerManager;

    void Awake()
    {
        ;
    }

	// Use this for initialization
	void Start () {
        if (panelArray == null)
        {
            panelArray = GameObject.FindGameObjectWithTag("PanelArray").GetComponent<PanelArray>();
        }
        if (customGaugeManager == null)
        {
            customGaugeManager = GameObject.FindGameObjectWithTag("CustomGauge").GetComponent<CustomGaugeManager>();
        }


        //TODO
        // gather combatants if not set in inspector
        // Configure Panels
        // place objects / hazards on field
        // handle battle music
        // handle background
        PlaceCombatants(combatants);

	}
	
	// Update is called once per frame
	void Update () {
        if (!atCustomScreen)
        {
            customGaugeManager.IncrementCustomGauge();
        }
        //stop time if needs be
        //
		
	}//end Update()

    public static void PlaceCombatants(GameObject[] combatants_List)
    {
        NaviController_Battle naviController;
        foreach (GameObject combatant in combatants_List)
        {
            naviController = combatant.GetComponent<NaviController_Battle>();// get the naviController
            Panel targetPanel = naviController.GetDesiredStartingPanel();//target the panel the navi wants to start at
            naviController.transform.position = targetPanel.GetPosition() + naviController.GetSpriteOffset();//move sprite to new location, offset the sprite to be at center of board
            targetPanel.OccupyPanel(naviController.gameObject);//target Panel now has this object as an occupant
            naviController.UpdateCurrentPanelCoordinates();//update coordinates of current panel
        }
    }


}    
