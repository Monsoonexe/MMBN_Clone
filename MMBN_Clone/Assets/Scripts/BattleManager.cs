using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class BattleManager : MonoBehaviour {
    private bool atCustomScreen = false;

    public CustomGaugeManager customGaugeManager;
    public Image battleBackground;
    
    public BattleType battleType = BattleType.DUEL;
    public GameObject[] combatants;//things that fight -- positions get set
    public GameObject[] boardObjects;// other pieces on the board -- obstacles
    public PanelArray panelArray;

    private PlayerManager playerManager;
    [SerializeField]
    private static int secondsToWaitAfterGameEnds = 5;
    private static readonly string characterSelectionSceneNameString = "Navi Select Screen";

    void Awake()
    {
         GatherTargetsBehavior.InitStaticReferences();
    }

	// Use this for initialization
	void Start () {
        if (panelArray == null)
        {
            panelArray = GameObject.FindGameObjectWithTag("PanelArray").GetComponent<PanelArray>() as PanelArray;
        }
        if (customGaugeManager == null)
        {
            customGaugeManager = GameObject.FindGameObjectWithTag("CustomGauge").GetComponent<CustomGaugeManager>() as CustomGaugeManager;
        }

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

    private static IEnumerator ReturnToCharacterSelectionScreen()
    {
        yield return new WaitForSeconds(secondsToWaitAfterGameEnds);

        SceneManager.LoadScene(characterSelectionSceneNameString);
    }

    public static void OnCombatantDeath(NaviController_Battle deadNavi)
    {
        //Display win Text

        deadNavi.StartCoroutine(ReturnToCharacterSelectionScreen());//WHY USE DEADNAVI AS OBJECT REFERENCE TO START COROUTINE? why not?
    }

    /// <summary>
    /// Find where the combatants wish to be place and place them there.
    /// </summary>
    /// <param name="combatants_List"></param>
    public static void PlaceCombatants(GameObject[] combatants_List)
    {
        NaviController_Battle naviController;
        Panel targetPanel; 
        foreach (var combatant in combatants_List)
        {
            naviController = combatant.GetComponent<NaviController_Battle>() as NaviController_Battle;// get the naviController
            targetPanel = naviController.GetDesiredStartingPanel();//target the panel the navi wants to start at
            naviController.transform.position = targetPanel.GetPosition() + naviController.NaviAsset.spriteOffset;//move sprite to new location, offset the sprite to be at center of board
            targetPanel.OccupyPanel(naviController);//target Panel now has this object as an occupant
            naviController.UpdateCurrentPanelCoordinates();//update coordinates of current panel
        }
    }
}    
