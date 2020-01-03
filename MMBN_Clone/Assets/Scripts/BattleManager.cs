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
	void Start () 
    {
        if (panelArray == null)
        {
            panelArray = GameObject.FindGameObjectWithTag("PanelArray").GetComponent<PanelArray>() as PanelArray;
        }
        if (customGaugeManager == null)
        {
            customGaugeManager = GameObject.FindGameObjectWithTag("CustomGauge").GetComponent<CustomGaugeManager>() as CustomGaugeManager;
        }

        PlaceCombatants();
        
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (!atCustomScreen)
        {
            customGaugeManager.IncrementCustomGauge();
        }
        //stop time if needs be
        //

    }//end Update()

    private void PlaceCombatants()
    {
        NaviController_Battle naviController;

        naviController = combatants[0].GetComponent<NaviController_Battle>();
        naviController.MoveNavi(1, 1);

        naviController = combatants[1].GetComponent<NaviController_Battle>();
        naviController.MoveNavi(4, 1);
    }

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

}    
